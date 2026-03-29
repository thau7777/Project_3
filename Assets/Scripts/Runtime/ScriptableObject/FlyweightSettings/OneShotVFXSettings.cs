using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New OneShotVFX Settings", menuName = "Scriptable Objects/Flyweight/OneShotVFX Settings")]
public class OneShotVFXSettings : FlyweightSettings
{
    #region Initial Settings
    [field: SerializeField]
    public float DefaultLifeTime { get; private set; } = 1;
    [field: SerializeField]
    public float DefaultSize { get; private set; } = 1;
    public bool useAdvanceSettings = false;
    [TabGroup("Advance Settings")]
    [ShowIf("useAdvanceSettings")]
    public string playEventName = "Play";

    [TabGroup("Advance Settings")]
    [ShowIf("useAdvanceSettings")]
    public string durationName = "Duration";

    [TabGroup("Advance Settings")]
    [ShowIf("useAdvanceSettings")]
    public string sizeName = "Size";
    #endregion

    #region Damage Settings
    [TabGroup("Damage Settings")]
    [SerializeField]
    private bool _canDealDamage = false;

    [ShowIf("_canDealDamage")]
    [TabGroup("Damage Settings")]
    public bool isMagicAttack = false;

    [ShowIf("_canDealDamage")]
    [TabGroup("Damage Settings")]
    public ElementalType elementalType = ElementalType.Normal;

    [SerializeField]
    [ShowIf("_canDealDamage")]
    [TabGroup("Damage Settings")]
    public OneShotVFXSettings hitImpactVFXSetting;

    #endregion

    #region Effect Settings
    [SerializeField]
    [TabGroup("Effect Settings")]
    private bool _canApplyEffect = false;

    public bool CanApplyEffect
    {
        get => _canApplyEffect;
        set
        {
            _canApplyEffect = value;
        }
    }

    [ShowIf("_canApplyEffect")]
    [TabGroup("Effect Settings")]
    public bool pickRandomEffectFromList = false;

    [ShowIf("_canApplyEffect")]
    [TabGroup("Effect Settings")]
    public List<EffectData> effectsToApplyList = new();

    #endregion

    #region Decal Settings
    [TabGroup("Decal Settings")]
    public DecalProjectorSettings decalSettings;

    private bool _hasDecal;
    [TabGroup("Decal Settings")]
    [ShowIf("_hasDecal")]
    public Material decalMaterial;
    [TabGroup("Decal Settings")]
    [ShowIf("_hasDecal")]
    public float decalSize;
    [TabGroup("Decal Settings")]
    [ShowIf("_hasDecal")]
    public float decalDelayTime;
    [TabGroup("Decal Settings")]
    [ShowIf("_hasDecal")]
    public float decalDuration;
    #endregion

    #region HitBox Stuff
    private bool _hasHitBox;
    public bool HasHitBox => _hasHitBox;

    [SerializeField]
    [ShowIf("_hasHitBox")]
    [TabGroup("HitBox Settings")]
    private bool _useParticleCollision = false;

    public bool UseParticleCollision => _useParticleCollision;

    [ShowIf("_useParticleCollision",true)]
    [TabGroup("HitBox Settings")]
    [MinMaxSlider(0, 1)]
    public Vector2 hitboxOnOffTime = new Vector2(0, 0.1f);

    [TabGroup("HitBox Settings")]
    [ShowIf("_hasHitBox")]
    public bool useTriggerStays = false;

    [ShowIf("_hasHitBox")]
    [TabGroup("HitBox Settings")]
    public bool reverseKnockBackDirection = false;

    [TabGroup("HitBox Settings")]
    [ShowIf("useTriggerStays")]
    public float triggerStayTickInterval = 0.2f;

    #endregion

    [SerializeField] private bool _addComponentsFirst = true;
    private void OnValidate()
    {
        _hasHitBox = _canDealDamage || _canApplyEffect;
        _hasDecal = decalSettings;
    }
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        var flyweight = go.GetOrAdd<OneShotVFX>();
        flyweight.settings = this;


        if (HasHitBox && _addComponentsFirst)
        {
            if (!_useParticleCollision)
                go.GetOrAdd<HitBoxHandler>();
            if (_canDealDamage)
                go.GetOrAdd<DamageDealer>();
            if (CanApplyEffect)
            {
                var effectApplier = go.GetOrAdd<EffectApplier>();

                if (effectsToApplyList.Count > 0)
                    effectApplier.SetEffects(effectsToApplyList);
            }
        }


        return flyweight;
    }

    public override void OnGet(Flyweight f)
    {
        
    }
    public override void OnRelease(Flyweight f)
    {
        if (f.transform.parent != null) 
            f.transform.SetParent(null);

        base.OnRelease(f);
    }

}
