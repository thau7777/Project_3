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
    [ShowIf("useAdvanceSettings")]
    public string playEventName = "Play";
    [ShowIf("useAdvanceSettings")]
    public string durationName = "Duration";
    [ShowIf("useAdvanceSettings")]
    public string sizeName = "Size";
    #endregion

    #region Damage Settings
    [SerializeField]
    private bool _canDealDamage = false;

    public bool CanDealDamage
    {
        get => _canDealDamage;
        set
        {
            _canDealDamage = value;
        }
    }

    [SerializeField]
    [ShowIf("_canDealDamage")]
    [TabGroup("Damage Settings")]
    private int _damage = 40;

    [SerializeField]
    [ShowIf("_canDealDamage")]
    [TabGroup("Damage Settings")]
    private int _knockBackForce = 10;

    [SerializeField]
    [ShowIf("_canDealDamage")]
    [TabGroup("Damage Settings")]
    private OneShotVFXSettings _hitImpactVFXSetting;

    #endregion

    #region Effect Settings
    [SerializeField]
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
    [SerializeField]
    private List<Effect> _effectsToApplyList = new();

    #endregion

    #region Decal Settings
    public DecalProjectorSettings decalSettings;

    private bool _hasDecal;
    [ShowIf("_hasDecal")]
    public Material decalMaterial;
    [ShowIf("_hasDecal")]
    public float decalSize;
    [ShowIf("_hasDecal")]
    public float decalDelayTime;
    [ShowIf("_hasDecal")]
    public float decalDuration;
    #endregion

    #region HitBox Stuff
    private bool _hasHitBox;
    public bool HasHitBox => _hasHitBox;


    [ShowIf("_hasHitBox")]
    [MinMaxSlider(0, 1)]
    public Vector2 hitboxOnOffTime = new Vector2(0, 0.1f);

    [ShowIf("_hasHitBox")]
    public LayerMask dodgeLayers;
    #endregion
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

        if (HasHitBox)
        {
            var hitboxHandler = go.GetOrAdd<HitBoxHandler>();
            hitboxHandler.DodgeLayers = dodgeLayers;
            hitboxHandler.HitboxOnOffTime = hitboxOnOffTime;
            if (CanDealDamage)
            {
                var damageDealer = go.GetOrAdd<DamageDealer>();
                damageDealer.Damage = _damage;
                damageDealer.KnockbackForce = _knockBackForce;

                if (_hitImpactVFXSetting)
                    damageDealer.SetHitImpactVFX(_hitImpactVFXSetting);
            }
            if (CanApplyEffect)
            {
                var effectApplier = go.GetOrAdd<EffectApplier>();

                if (_effectsToApplyList.Count > 0)
                    effectApplier.SetEffects(_effectsToApplyList);
            }
        }


        return flyweight;
    }

    public override void OnGet(Flyweight f)
    {
        
    }

}
