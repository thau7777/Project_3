using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public bool isDashForward;
    public float dashForce;
    public AimType aimType;
}


[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Objects/StrategyPattern/Skill")]
public class SkillStrategy : ScriptableObject, IStrategy
{
    #region Skill Condition
    [TabGroup("Skill Condition")]
    [SerializeField]
    public bool needEnemiesInSkillIndicator = false;

    [TabGroup("Skill Condition")]
    [ShowIf("needEnemiesInSkillIndicator")]
    [SerializeField]
    protected LayerMask enemyLayer;

    [TabGroup("Skill Condition")]
    [ShowIf("needEnemiesInSkillIndicator")]
    [SerializeField]
    protected LayerMask groundLayer;
    #endregion

    [TabGroup("Skill Data For Classes")]
    [SerializeField]
    protected List<SkillDataForClass> dataForClasses;

    #region Skill Settings
    [TabGroup("Skill Settings")]
    public bool isPassiveSkill = false;
    [TabGroup("Skill Settings")]
    public Sprite skillIcon;
    [TabGroup("Skill Settings")]
    public String skillDescription = "bla bla bla";

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected FlyweightSettings _mainSkillVfxSettings;
    public FlyweightSettings MainSkillVfxSettings => _mainSkillVfxSettings;

    [TabGroup("Skill Settings")]
    [SerializeField]
    private float _manaCost = 20;
    public float ManaCost => _manaCost;

    [TabGroup("Skill Settings")]
    public VFXSpawnLocation spawnLocation;

    [TabGroup("Skill Settings")]
    [ShowIf("_isProjectile", true)]
    [SerializeField]
    private bool _setParentToUser;

    [TabGroup("Skill Settings")]
    public Vector3 positionOffset = Vector3.zero;

    [TabGroup("Skill Settings")]
    [ShowIf("_isProjectile", true)]
    public Vector3 rotationOffset = Vector3.zero;

    [TabGroup("Skill Settings")]
    public LayerMask DodgeLayers;
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
    protected int _damage = 10;
    public int Damage => _damage;
    [TabGroup("Skill Settings")]
    [SerializeField]
    protected bool _dealTrueDamage;
    public bool DealTrueDamage => _dealTrueDamage;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected float _knockbackForce = 3;
    public float KnockbackForce => _knockbackForce;

    [TabGroup("Skill Settings")]
    [SerializeField]
    public bool canRotateWhileUsingSkill;

    [TabGroup("Skill Settings")]
    [SerializeField]
    protected bool _needHoldStill;
    public bool NeedHoldStill => _needHoldStill;

    [TabGroup("Skill Settings")]
    [SerializeField]
    public bool canBeInterrupted;

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

    #endregion

    #region Skill Indicator Settings
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

    #endregion

    #region Effects Settings
    [TabGroup("Effects Settings")]
    [SerializeField]
    private bool _hasBuffOrDebuffToUser;

    [TabGroup("Effects Settings")]
    [ShowIf("_hasBuffOrDebuffToUser")]
    [SerializeField]
    private List<EffectData> _effectsToApply = new List<EffectData>();

    #endregion

    private void OnValidate()
    {
        _isProjectile = MainSkillVfxSettings is StraightProjectileSettings;
    }

    public virtual void Execute(IStrategyContext context)
    {
        var skillContext = context as SkillStrategyContext;
        if (!skillContext.chargedSkillFlyweight)
            SpawnSkillVFX(skillContext);
        else
            ExecuteChargedSkill(skillContext);
    }
    public void ApplyEffectsToUser(GameObject target)
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
        if (!needEnemiesInSkillIndicator)
            return true;
        return HasEnemiesAtMousePosition(user);
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
        // Check for enemies in sphere at mouse position
        Collider[] enemiesInRange = Physics.OverlapSphere(mouseWorldPos, indicatorWidth/2, enemyLayer);

        foreach (Collider col in enemiesInRange)
        {
            if (enemyLayer.Contains(col.gameObject.layer))
            {
                return true; // Found at least one enemy
            }
        }

        return false; // No enemies found
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
        if (MainSkillVfxSettings == null) return;
        Flyweight flyweightObj = FlyweightFactory.Spawn(MainSkillVfxSettings);
        Vector3 spawnPosition = context.spawnTransform.AddLocal(context.positionOffset.x, context.positionOffset.y, context.positionOffset.z);
        flyweightObj.FlyweightInitialize(spawnPosition, context.origin.rotation * (context.rotationOffset == Vector3.zero ? Quaternion.Euler(1, 1, 1): Quaternion.Euler(rotationOffset)));
        
        if (flyweightObj is StraightProjectile straightProjectile)
        {
            StraightProjectileSettings projectileSettings = straightProjectile.settings as StraightProjectileSettings;

            CharacterStats userStats = context.origin.GetComponent<CharacterStats>();
            bool isCrit = userStats.CriticalRate > 0 && UnityEngine.Random.Range(0,100) < userStats.CriticalRate;
            int finalDamage = isCrit ? Mathf.RoundToInt(Damage * userStats.CriticalMultiplier) : Damage;

            straightProjectile.InitializeProjectile(context.origin.gameObject, context.origin.forward, 10, Range, projectileSettings.defaultSize, isCrit, finalDamage, _knockbackForce, _dealTrueDamage, DodgeLayers);
        }
        else if (flyweightObj is OneShotVFX oneShotVFX)
        {
            OneShotVFXSettings oneShotVFXSettings = oneShotVFX.settings as OneShotVFXSettings;

            if (flyweightObj.TryGetComponent<HitBoxHandler>(out var hitboxHandler))
            {
                hitboxHandler.Setup(
                    context.origin.gameObject,
                    DodgeLayers,
                    oneShotVFXSettings.hitboxOnOffTime,
                    oneShotVFXSettings.useTriggerStays,
                    oneShotVFXSettings.triggerStayTickInterval,
                    false);
                
            }
            if (flyweightObj.TryGetComponent<DamageDealer>(out var damageDealer))
            {
                CharacterStats userStats = context.origin.GetComponent<CharacterStats>();
                bool isCrit = userStats.CriticalRate > 0 && UnityEngine.Random.Range(0, 100) < userStats.CriticalRate;
                int finalDamage = isCrit ? Mathf.RoundToInt(Damage * userStats.CriticalMultiplier) : Damage;

                damageDealer.Setup(
                    isCrit,
                    finalDamage,
                    _dealTrueDamage,
                    _knockbackForce,
                    oneShotVFXSettings.reverseKnockBackDirection,
                    oneShotVFXSettings.elementalType);
            }
            oneShotVFX.InitializeVFX(Size, (oneShotVFX.settings as OneShotVFXSettings).DefaultLifeTime);
            if (_setParentToUser)
                oneShotVFX.transform.SetParent(context.origin.transform);


        }
    }

    private void ExecuteChargedSkill(SkillStrategyContext context)
    {
        //if (context.chargedSkillFlyweight is StraightProjectile chargedSkillProjectile)
        //{
        //    context.chargedSkillFlyweight.transform.SetParent(null);
        //    chargedSkillProjectile.transform.rotation = Quaternion.identity;

        //    StraightProjectileSettings projectileSettings = chargedSkillProjectile.settings as StraightProjectileSettings;
        //    chargedSkillProjectile.InitializeProjectile(context.origin.gameObject, context.origin.forward, 10, Range, Size, Damage,_knockbackForce, _dealTrueDamage, DodgeLayers);
        //}
    }

    public void OnInterupted(Transform user)
    {
        Flyweight skillVfx = user.Find(_mainSkillVfxSettings.prefab.name)?.GetComponent<Flyweight>();
        skillVfx.ReturnToPool();
    }

    public void UpdateDamage(int newValue)
    {
        _damage = newValue;
    }
}