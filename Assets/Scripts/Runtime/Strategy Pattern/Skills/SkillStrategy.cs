using System;
using System.Collections.Generic;
using UnityEngine;

public enum AimType
{
    None,
    Below,
    Straight,
    Above
}

[Serializable]
public struct SkillDataForClass
{
    public CharacterClass characterClass;
    public string animName;
    public VFXSpawnLocation spawnLocation;
    public Vector3 positionOffset;
    public bool isDashForward;
    public float dashForce;
    public AimType aimType;
}

public enum SkillCondition
{
    None,
    InMousePosition
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Objects/StrategyPattern/Skill")]
public class SkillStrategy : ScriptableObject, IStrategy
{
    [TabGroup("Skill Condition")]
    [SerializeField]
    protected SkillCondition _specialCondition = SkillCondition.None;

    [TabGroup("Skill Condition")]
    [ShowIfEnumValue("_specialCondition", SkillCondition.InMousePosition)]
    [SerializeField]
    protected float targetCheckRadius = 3f;

    [TabGroup("Skill Condition")]
    [ShowIfEnumValue("_specialCondition", SkillCondition.InMousePosition)]
    [SerializeField]
    protected LayerMask enemyLayer;

    [TabGroup("Skill Condition")]
    [ShowIfEnumValue("_specialCondition", SkillCondition.InMousePosition)]
    [SerializeField]
    protected LayerMask groundLayer;

    [TabGroup("Skill Data For Classes")]
    [SerializeField]
    protected List<SkillDataForClass> dataForClasses;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected FlyweightSettings _mainSkillVfxSettings;
    public FlyweightSettings FlyweightSettings => _mainSkillVfxSettings;

    [TabGroup("Skill Settings")]
    [ShowIf("_isProjectile", true)]
    [SerializeField]
    private bool _setParentToUser;

    [TabGroup("Skill Settings")]
    [SerializeField]
    private LayerMask DodgeLayers;
    protected bool _isProjectile;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected float _coolDown = 5;
    public float Cooldown => _coolDown;

    [TabGroup("Skill Settings")]
    [ShowIf("_isProjectile")]
    [SerializeField]
    protected float _speed = 10;
    public float Speed => _speed;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected float _range = 10;
    public float Range => _range;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected float _size = 1;
    public float Size => _size;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected float _damage = 10;
    public float Damage => _damage;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected bool _needHoldStill;
    public bool NeedHoldStill => _needHoldStill;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected bool _canCharge;
    public bool CanCharge { get => _canCharge; private set => _canCharge = value; }

    [TabGroup("Skill Settings")]
    [ShowIf("_canCharge")]
    public VFXSpawnLocation chargeVFXSpawnLocation;

    [TabGroup("Skill Settings")]
    [ShowIf("_canCharge")]
    public OneShotVFXSettings chargingEffect;

    [TabGroup("Skill Settings")]
    [ShowIf("_canCharge")]
    public int chargeLevel = 1;

    [TabGroup("Skill Indicator Settings")]
    public bool useIndicator;

    [TabGroup("Skill Indicator Settings")]
    [ShowIf("useIndicator")]
    public SkillIndicatorSettings skillIndicator;

    [TabGroup("Skill Indicator Settings")]
    [ShowIf("useIndicator")]
    public float indicatorWidth = 1;

    [TabGroup("Skill Indicator Settings")]
    [ShowIf("useIndicator")]
    public float indicatorLength = 1;

    [TabGroup("Buff / Debuff To User Settings")]
    [SerializeField]
    private bool _hasBuffOrDebuffToUser;

    [TabGroup("Buff / Debuff To User Settings")]
    [ShowIf("_hasBuffOrDebuffToUser")]
    [SerializeField]
    private List<EffectData> _effectsToApply = new List<EffectData>();
    private void OnValidate()
    {
        _isProjectile = FlyweightSettings is StraightProjectileSettings;
    }

    public virtual void Execute(IStrategyContext context)
    {
        var skillContext = context as SkillStrategyContext;
        if(_hasBuffOrDebuffToUser)
            ApplyBuffOrDeBuffToUser(skillContext.origin.root.gameObject);
        if (!skillContext.chargedSkillFlyweight)
            SpawnSkillVFX(skillContext);
        else
            ExecuteChargedSkill(skillContext);
    }
    private void ApplyBuffOrDeBuffToUser(GameObject target)
    {
        if (_effectsToApply == null || _effectsToApply.Count == 0)
        {
            Debug.LogWarning("No effects assigned to BuffOrDeBuffOnSingleTarget skill!");
            return;
        }
        EffectsManager manager = target.GetOrAdd<EffectsManager>();
        foreach (EffectData effect in _effectsToApply)
        {
            manager.AddEffect(effect);
        }
    }
    public bool CheckSpecialCondition(Transform user)
    {
        switch (_specialCondition)
        {
            case SkillCondition.None:
                return true;

            case SkillCondition.InMousePosition:
                return HasEnemiesAtMousePosition(user);

            default:
                return true;
        }
    }

    private bool HasEnemiesAtMousePosition(Transform user)
    {
        // Get mouse position on ground
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if (mouseWorldPos == Vector3.zero) return false;

        // Get player position from context
        Vector3 startPoint = user.position;
        Vector3 directionToMouse = mouseWorldPos - startPoint;
        directionToMouse.y = 0;
        float distanceToMouse = directionToMouse.magnitude;

        // Clamp to max range if needed
        if (distanceToMouse > Range)
        {
            mouseWorldPos = startPoint + directionToMouse.normalized * Range;
        }

        // Check for enemies in sphere at mouse position
        Collider[] enemiesInRange = Physics.OverlapSphere(mouseWorldPos, targetCheckRadius, enemyLayer);

        foreach (Collider col in enemiesInRange)
        {
            if (enemyLayer.Contains(col.gameObject.layer))
            {
                return true; // Found at least one enemy
            }
        }

        return false; // No enemies found
    }

    public SkillDataForClass? GetSkillDataForClass(CharacterClass characterClass)
    {
        bool hasThatClass = false;
        foreach (var dataForClass in dataForClasses)
        {
            if (dataForClass.characterClass == characterClass)
            {
                hasThatClass = true;
                break;
            }
        }
        if (!hasThatClass) return null;

        foreach (var dataForClass in dataForClasses)
        {
            if (dataForClass.characterClass == characterClass)
                return dataForClass;
        }
        return null;
    }

    private void SpawnSkillVFX(SkillStrategyContext context)
    {
        if (FlyweightSettings == null) return;
        Flyweight flyweightObj = FlyweightFactory.Spawn(FlyweightSettings);
        Vector3 spawnPosition = context.spawnTransform.AddLocal(context.positionOffset.x, context.positionOffset.y, context.positionOffset.z);
        flyweightObj.FlyweightInitialize(spawnPosition, context.origin.rotation);

        if (flyweightObj is StraightProjectile straightProjectile)
        {
            StraightProjectileSettings projectileSettings = straightProjectile.settings as StraightProjectileSettings;
            straightProjectile.InitializeProjectile(context.origin.forward, 10, Range, projectileSettings.defaultSize, Damage, DodgeLayers);
        }
        else if (flyweightObj is OneShotVFX oneShotVFX)
        {
            oneShotVFX.InitializeVFX(Size, (oneShotVFX.settings as OneShotVFXSettings).DefaultLifeTime);
            if (_setParentToUser)
                oneShotVFX.transform.SetParent(context.origin.transform);
        }
    }

    private void ExecuteChargedSkill(SkillStrategyContext context)
    {
        if (context.chargedSkillFlyweight is StraightProjectile chargedSkillProjectile)
        {
            context.chargedSkillFlyweight.transform.SetParent(null);
            chargedSkillProjectile.transform.rotation = Quaternion.identity;

            StraightProjectileSettings projectileSettings = chargedSkillProjectile.settings as StraightProjectileSettings;
            chargedSkillProjectile.InitializeProjectile(context.origin.forward, 10, Range, Size, Damage, DodgeLayers);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
            return hit.point;
        }

        // Fallback: calculate intersection with Y=0 plane
        if (ray.direction.y != 0)
        {
            float distance = -ray.origin.y / ray.direction.y;
            return ray.origin + ray.direction * distance;
        }

        return Vector3.zero;
    }

    public void UpdateStat()
    {

    }
}