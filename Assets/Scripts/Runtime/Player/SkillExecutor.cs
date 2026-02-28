using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillExecutor : MonoBehaviour
{
    [SerializeField, TabGroup("Skills Setup")]
    private SkillStrategy[] _skillStrategies = new SkillStrategy[6];

    [SerializeField, TabGroup("References")]
    private List<Transform> _skillSpawnPoints;

    [SerializeField, TabGroup("References")]
    private CharacterControllerLayerIgnoreController _layerIgnoreController;


    private SkillRuntimeInstance[] _skillInstance = new SkillRuntimeInstance[6];
    private SkillRuntimeInstance _skillToCast;
    private SkillDataForClass? _storedSkillDataForClass;
    public SkillDataForClass? StoredSkillDataForClass => _storedSkillDataForClass;
    private Flyweight _chargedSkillFlyweight;
    private Coroutine _chargeCoroutine;
    private Coroutine _lerpCoroutine;
    private SkillIndicator _skillIndicator;


    [SerializeField, TabGroup("Stats")]
    private float _currentMana;
    public float CurrentMana => _currentMana;
    [SerializeField, TabGroup("Stats")]
    private float _maxMana;
    public float MaxMana => _maxMana;

    [TabGroup("Stats")]
    public UnityEvent<float, float> OnManaChanged;

    void Awake()
    {
        _layerIgnoreController = GetComponent<CharacterControllerLayerIgnoreController>();
        InitializeMana(100);
    }
    private void Start()
    {
        InitializeSkillInstance();
        ExecuteAllPassiveSkills();
    }
    private void InitializeMana(float maxMana)
    {
        _maxMana = maxMana;
        _currentMana = maxMana;
        OnManaChanged?.Invoke(_currentMana, _maxMana);
    }
    private void InitializeSkillInstance()
    {
        for(int i = 0; i < 6; i++)
        {
            _skillInstance[i] = new SkillRuntimeInstance(_skillStrategies[i], i);
        }
        
        EventBus<TopDownInitializeSkillsEvent>.Raise(new TopDownInitializeSkillsEvent(_skillInstance));
    }
    public void ExecuteAllPassiveSkills()
    {
        foreach (var skill in _skillStrategies)
        {
            if (skill && skill.isPassiveSkill){
                var ctx = new SkillStrategyContext(transform, transform, Vector3.zero, Quaternion.identity);
                if (skill is FireCircle)
                    (skill as FireCircle).Execute(ctx);
                else
                    skill.Execute(ctx);
            }
            
        }
    }
    public void RestoreMana(float amount)
    {
        _currentMana = Mathf.Min(_maxMana, _currentMana + amount);
        OnManaChanged?.Invoke(_currentMana, _maxMana);
    }
    public void ConsumeMana(float amount)
    {
        _currentMana = Mathf.Max(0, _currentMana - amount);
        OnManaChanged?.Invoke(_currentMana, _maxMana);
    }
    public void IncreaseMaxMana(float amount)
    {
        _maxMana += amount;
        OnManaChanged?.Invoke(_currentMana, _maxMana);
    }
    // always get that skill data first if return ok then we can cast it later
    public bool SetSkillData(int index, CharacterClass characterClass)
    {
        if(index < 0 || index > 5 || _skillStrategies[index] == null || _skillStrategies[index].isPassiveSkill) return false;
        var skillData = _skillInstance[index].Definition.GetSkillDataForClass(characterClass);
        if (skillData == null || _skillInstance[index].IsOnCooldown) return false;
        // store the data for the cast
        _skillToCast = _skillInstance[index];
        _storedSkillDataForClass = skillData;
        return true;
    }
    public void UseSkill(int index, CharacterClass characterClass,PlayerTopdownContext context, Action onCastInstantly = null)
    {
        if (!SetSkillData(index, characterClass)) return;
        if(CurrentMana < _skillToCast.Definition.ManaCost && _skillToCast.Definition.ManaCost != 0)
        {
            Debug.Log("Not enough mana to cast skill: " + _skillToCast.Definition.name);
            return;
        }
        context.IsNextAttackQueued = false;
        context.CastingSkill = index;
        bool isAimNeeded = _storedSkillDataForClass.Value.aimType != AimType.None;
        context.IsUseSkillByUpperBody = isAimNeeded;
        if (isAimNeeded)
        {
            // run aim anim first
            context.IsAiming = true;
            context.AimAnimName = _storedSkillDataForClass.Value.aimType.ToString();
        }
        if (_skillToCast.Definition.CanCharge && _skillToCast.Definition.chargingEffect)
        {
            _chargedSkillFlyweight = FlyweightFactory.Spawn(_skillToCast.Definition.chargingEffect);
            Transform chargeSpawnTransform = GetSkillSpawnTransform(_skillToCast.Definition.chargeVFXSpawnLocation);

            _chargedSkillFlyweight.FlyweightInitialize(chargeSpawnTransform.position);
            if(_chargedSkillFlyweight is OneShotVFX _chargeOneShotVfx)
            {
                var chargeEffectVFXSettings = _chargeOneShotVfx.settings as OneShotVFXSettings;
                _chargeOneShotVfx.InitializeVFX(chargeEffectVFXSettings.DefaultSize, chargeEffectVFXSettings.DefaultLifeTime);
            }

            _chargedSkillFlyweight.transform.SetParent(chargeSpawnTransform);

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
                    followedIndicator.Initialize(transform, _skillToCast.Definition.indicatorWidth, _skillToCast.Definition.indicatorLength);
                    break;
                }
                case FlyweightType.IndicatorConeAlly:
                {
                    _skillIndicator.FlyweightInitialize(transform.position,transform.rotation);
                    var followedIndicator = _skillIndicator as FollowedIndicator;
                    followedIndicator.Initialize(transform, _skillToCast.Definition.indicatorWidth, _skillToCast.Definition.indicatorLength);
                    break;
                }
                case FlyweightType.IndicatorCircleAlly:
                {
                    _skillIndicator.FlyweightInitialize(transform.position.Add(y:-1f));
                    var circleIndicator = _skillIndicator as CircleIndicator;
                    circleIndicator.Initialize(_skillToCast.Definition.indicatorWidth);

                    break;
                }
            }
            return;
        }

        onCastInstantly?.Invoke();
        CastSkill(context);
    }

    public void CastSkill(PlayerTopdownContext context)// run the actual skill animation
    {
        if (_skillToCast == null) return;
        if(_skillIndicator != null)
        {
            _skillIndicator.ReturnToPool();
        }

        if (_chargeCoroutine != null)
        {
            StopCoroutine(_chargeCoroutine);
            _chargeCoroutine = null;

            if(_lerpCoroutine != null)
            {
                StopCoroutine(_lerpCoroutine);
                _lerpCoroutine = null;
            }
        }
        
        string animName = _storedSkillDataForClass.Value.animName;
        if (animName == "Dash")
        {
            _layerIgnoreController.ApplyLayerIgnore(_skillToCast.Definition.DodgeLayers);
            context.IsDashing = true;
        }
        context.IsInSpecialMove = true;
        context.NeedHoldStillWhileExecuteWhenAiming = _skillToCast.Definition.NeedHoldStill;
        context.SkillAnimName = animName;

        ConsumeMana(_skillToCast.Definition.ManaCost);
    }
    public void OnSkillTrigger()
    {
        ExecuteSkill();
    }
    private void ExecuteSkill()
    {
        if (_skillToCast == null || _storedSkillDataForClass == null) return;
        Transform spawnTransform = GetSkillSpawnTransform(_skillToCast.Definition.spawnLocation);
        var ctx = new SkillStrategyContext(transform, spawnTransform, _skillToCast.Definition.positionOffset, _skillToCast.Definition.rotationOffset, _chargedSkillFlyweight);

        _skillToCast.Cast(ctx);

        ClearSkillData();
    }

    public void ClearSkillData()
    {
        _skillIndicator = null;
        _skillToCast = null;
        _storedSkillDataForClass = null;
        _chargedSkillFlyweight = null;
    }
    private Transform GetSkillSpawnTransform(VFXSpawnLocation location)
    {
        if (location == VFXSpawnLocation.Mouse)
            return _skillIndicator.transform;
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
    public void TurnOffSkillIndicator()
    {
        if (_skillIndicator)
        {
            _skillIndicator.ReturnToPool();
        }
        
        _storedSkillDataForClass = null;
        _chargedSkillFlyweight = null;
    }

    public void AddOrReplaceSkill(int index, SkillStrategy strategy)
    {
        if (index < 0 || index > 5) return;
        _skillInstance[index] = new SkillRuntimeInstance(strategy, index);
    }
}
