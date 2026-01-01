using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{

    [SerializeField]
    private SkillStrategy[] _skillDatas = new SkillStrategy[4];
    private SkillRuntimeInstance[] _skillInstance = new SkillRuntimeInstance[4];

    [SerializeField]
    private List<Transform> _skillSpawnPoints;

    private SkillRuntimeInstance _skillToCast;
    private SkillDataForClass? _storedSkillData;
    public SkillDataForClass? StoredSkillData => _storedSkillData;
    private Flyweight _chargedSkillFlyweight;
    private Coroutine _chargeCoroutine;
    private Coroutine _lerpCoroutine;

    private SkillIndicator _skillIndicator;
    void Awake()
    {
        InitializeSkillInstance();
    }
    private void InitializeSkillInstance()
    {
        for(int i = 0; i < 4; i++)
        {
            _skillInstance[i] = new SkillRuntimeInstance(_skillDatas[i]);
        }
    }
    // always get that skill data first if return ok then we can cast it later
    public bool SetSkillData(int index, CharacterClass characterClass)
    {
        var skillData = _skillInstance[index].Definition.GetSkillDataForClass(characterClass);
        if (skillData == null) return false;
        _skillToCast = _skillInstance[index];
        if(_skillToCast.IsOnCooldown) return false;
        // store the data for the cast
        _storedSkillData = skillData;
        return true;
    }
    public void UseSkill(int index, CharacterClass characterClass,PlayerTopdownContext context, Action onCastInstantly = null)
    {
        if (!SetSkillData(index, characterClass)) return;

        context.IsNextAttackQueued = false;
        context.CastingSkill = index;
        bool isAimNeeded = _storedSkillData.Value.aimType != AimType.None;
        context.IsUseSkillByUpperBody = isAimNeeded;
        if (isAimNeeded)
        {
            // run aim anim first
            context.IsAiming = true;
            context.AimAnimName = _storedSkillData.Value.aimType.ToString();

            
        }
        if (_skillToCast.Definition.CanCharge && _skillToCast.Definition.chargingEffect)
        {
            _chargedSkillFlyweight = FlyweightFactory.Spawn(_skillToCast.Definition.chargingEffect ?? _skillToCast.Definition.FlyweightSettings);
            Transform spawnTransform = GetSkillSpawnTransform(_storedSkillData.Value.spawnLocation);

            _chargedSkillFlyweight.FlyweightInitialize(spawnTransform.position);
            var chargeEffectVFX = _chargedSkillFlyweight as OneShotVFX;
            var chargeEffectVFXSettings = chargeEffectVFX.settings as OneShotVFXSettings;
            chargeEffectVFX?.InitializeVFX(chargeEffectVFXSettings.DefaultSize, chargeEffectVFXSettings.DefaultLifeTime);

            _chargedSkillFlyweight.transform.SetParent(spawnTransform);

            //if (!_skillToCast.Definition.chargingEffect)
            //{
            //    _chargeCoroutine = StartCoroutine(ChargeSkill(
            //        _chargedSkillFlyweight, _skillToCast.Definition.chargeLevel));
            //}

        }

        if (isAimNeeded || _skillToCast.Definition.CanCharge)
        {
            if (!_skillToCast.Definition.useIndicator || !_skillToCast.Definition.skillIndicator) return;
            _skillIndicator = FlyweightFactory.Spawn(_skillToCast.Definition.skillIndicator) as SkillIndicator;
            switch (_skillToCast.Definition.skillIndicator.type)
            {
                default:
                    break;
                case FlyweightType.IndicatorStraightAlly:
                {
                    _skillIndicator.FlyweightInitialize(transform.position,transform.rotation);
                    var followedIndicator = _skillIndicator as FollowedIndicator;
                    followedIndicator.Initialize(transform, _skillToCast.Definition.Range * 12,3);
                    break;
                }
                case FlyweightType.IndicatorCircleAlly:
                {
                        var circleIndicator = _skillIndicator as CircleIndicator;
                        circleIndicator.Initialize(5);
                        break;
                }
            }

            return;
        }

        onCastInstantly?.Invoke();
        CastSkill(context);
    }

    private IEnumerator ChargeSkill(Flyweight chargingObject,int totalLevel)
    {
        int currentLevel = 0;
        while (currentLevel < totalLevel)
        {
            yield return Helpers.GetWaitForSeconds(2f);
            currentLevel++;

            Vector3 startScale = chargingObject.transform.localScale;
            Vector3 targetScale = startScale + Vector3.one * 0.3f;

            if (_lerpCoroutine != null)
                StopCoroutine(_lerpCoroutine);

            _lerpCoroutine = StartCoroutine(Helpers.LerpValue(
                startScale,
                targetScale,
                0.5f,
                Vector3.Lerp,
                value => chargingObject.transform.localScale = value
            ));


        }
        yield break;
    }
    public void CastSkill(PlayerTopdownContext context)// run the actual skill animation
    {
        if(_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;

            if(_lerpCoroutine != null)
            {
                StopCoroutine(_lerpCoroutine);
                _lerpCoroutine = null;
            }
        }
        string animName = _storedSkillData.Value.animName;
        if (animName == "Dash") context.IsDashing = true;
        context.IsInSpecialMove = true;
        context.NeedHoldStill = _skillToCast.Definition.NeedHoldStill;
        context.SkillAnimName = animName;
    }
    public void TurnOffSkillIndicator()
    {
        if (_skillIndicator)
        {
            _skillIndicator.ReturnToPool();
        }
    }
    // animation event
    public void OnSkillTrigger()
    {
        ExecuteSkill();
    }
    private void ExecuteSkill()
    {
        if (_skillToCast == null || _storedSkillData == null) return;
        //if (_chargedSkillFlyweight && !_skillToCast.Definition.chargingEffect && _chargedSkillFlyweight is StraightProjectile chargedProjectile)
        //{
        //    chargedProjectile.projectileImpactScale = _skillToCast.Definition.name switch
        //    {
        //        "Fireball" => _chargedSkillFlyweight.transform.localScale * 2f,
                
        //        _ => null
        //    };
        //}else if (_skillToCast.Definition.chargingEffect && _chargedSkillFlyweight)
        //{
        //    _chargedSkillFlyweight.ReturnToPool();
        //    _chargedSkillFlyweight = null;
        //}


        Vector3 spawnPos = GetSkillSpawnTransform(_storedSkillData.Value.spawnLocation).position;
        var ctx = new SkillStrategyContext(transform, spawnPos, _chargedSkillFlyweight);

        _skillToCast.Cast(ctx);

        ClearSkillData();
    }
    public void ClearSkillData()
    {
        _skillToCast = null;
        _storedSkillData = null;
        _chargedSkillFlyweight = null;
    }
    private Transform GetSkillSpawnTransform(VFXSpawnLocation location)
    {
        Transform skillSpawnTransform = transform;
        foreach (var sp in _skillSpawnPoints)
        {
            if (sp.name == location.ToString())
            {
                skillSpawnTransform = sp;
                break;
            }
        }
        return skillSpawnTransform;
    }
    public void AddOrReplaceSkill(int index, SkillStrategy newSkill)
    {
        if (index < 0 || index >= 4) return;
        _skillInstance[index] = new SkillRuntimeInstance(newSkill);
    }
}
