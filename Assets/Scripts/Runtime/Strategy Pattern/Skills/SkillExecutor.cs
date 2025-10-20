using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    [SerializeField] 
    private List<SkillStrategy> _skillDatas;
    [SerializeField]
    private List<Transform> _skillSpawnPoints;


    private List<SkillRuntimeInstance> _skillInstance;

    private SkillRuntimeInstance _skillToCast;
    private SkillDataForClass? _storedSkillData;
    public SkillDataForClass? StoredSkillData => _storedSkillData;
    private Flyweight _chargedSkillFlyweight;
    private Coroutine _chargeCoroutine;
    private Coroutine _lerpCoroutine;

    private Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _skillInstance = new List<SkillRuntimeInstance>();
        foreach (var s in _skillDatas)
            _skillInstance.Add(new SkillRuntimeInstance(s));
    }

    // always get that skill data first if return ok then we can cast it later
    public bool SetSkillData(int index, CharacterClass characterClass)
    {
        var skillData = _skillInstance[index].Definition.GetSkillDataByClass(characterClass);
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
        if (isAimNeeded)
        {
            // run aim anim first
            context.IsUseSkillByUpperBody = true;
            context.IsStrafing = true;
            _animator.CrossFade(_storedSkillData.Value.aimType.ToString(), 0.1f, 1);

            if (_skillToCast.Definition.CanCharge)
            {
                _chargedSkillFlyweight = FlyweightFactory.Spawn(_skillToCast.Definition.FlyweightSettings);
                Transform spawnTransform = GetSkillSpawnTransform(_storedSkillData.Value.spawnLocation);
                _chargedSkillFlyweight.Initialize(spawnTransform.position, Quaternion.identity);
                
                _chargedSkillFlyweight.transform.SetParent(spawnTransform);
                
                _chargeCoroutine = StartCoroutine(ChargeSkill(
                    _chargedSkillFlyweight, _skillToCast.Definition.chargeLevel));
            }
                
            return;
        }
        context.IsUseSkillByUpperBody = false;
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
        bool isDashSkill = _storedSkillData.Value.animName == "Dash";
        if (isDashSkill)
            context.IsDashing = true;
        context.IsInSpecialMove = true;
        context.NeedHoldStill = _skillToCast.Definition.NeedHoldStill;
        bool isAimNeeded = _storedSkillData.Value.aimType != AimType.None;
        string animName = _storedSkillData.Value.animName;
        _animator.CrossFade(animName, !isDashSkill ? 0.1f : 0f, isAimNeeded ? 1 : 0);
    }

    // animation event
    public void OnSkillTrigger()
    {
        ExecuteSkill();
    }
    private void ExecuteSkill()
    {
        if (_skillToCast == null) return;
        if (_chargedSkillFlyweight != null && _chargedSkillFlyweight is StraightProjectile chargedProjectile)
        {
            chargedProjectile.projectileImpactScale = _skillToCast.Definition.name switch
            {
                "Fireball" => _chargedSkillFlyweight.transform.localScale * 2f,
                
                _ => null
            };
        }

        if (_storedSkillData == null) return;

        Vector3 spawnPos = GetSkillSpawnTransform(_storedSkillData.Value.spawnLocation).position;
        var ctx = new SkillStrategyContext(transform, spawnPos, _chargedSkillFlyweight);

        _skillToCast.Cast(ctx);

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
        if (index < 0 || index >= _skillInstance.Count) return;
        _skillInstance[index] = new SkillRuntimeInstance(newSkill);
    }
}
