using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New OneShotVFX Settings", menuName = "Scriptable Objects/Flyweight/OneShotVFX Settings")]
public class SimpleOneShotVFXSettings : FlyweightSettings
{
    [field: SerializeField]
    public float LifeTime { get; private set; } = 1;

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
    private SimpleOneShotVFXSettings _hitImpactVFXSetting;

    #endregion

    #region Effect Settings
    [SerializeField]
    private bool _canApplyEffect = false;

    public bool CanApplyEffect
    {
        get => _canApplyEffect;
        set
        {
            _canApplyEffect= value;
        }
    }

    [ShowIf("_canApplyEffect")]
    [SerializeField]
    [TabGroup("Effects Settings")]
    private List<Effect> _effectsToApplyList = new();

    #endregion

    #region Decal Settings
    public FlyweightSettings decalEffectSettings;

    private bool _hasDecal; 
    [ShowIf("_hasDecal")]
    public float decalSize;
    [ShowIf("_hasDecal")]
    public float decalDelayTime;
    [ShowIf("_hasDecal")]
    public float decalDuration;
    #endregion

    private bool _hasHitBox;
    public bool HasHitBox => _hasHitBox;


    [ShowIf("_hasHitBox")]
    [MinMaxSlider(0,1)]
    public Vector2 hitboxOnOffTime = new Vector2(0,0.1f);

    [ShowIf("_hasHitBox")]
    public LayerMask dodgeLayers;

    private void OnValidate()
    {
        _hasHitBox = _canDealDamage || _canApplyEffect;
        _hasDecal = decalEffectSettings;
    }
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        ParticleSystem particleSystem = prefab.GetComponentInChildren<ParticleSystem>();
        LifeTime = particleSystem != null ? particleSystem.main.duration : LifeTime;

        var flyweight = go.GetOrAdd<SimpleOneShotVFX>();
        flyweight.settings = this;

        if (HasHitBox)
        {
            var hitboxHandler = go.GetOrAdd<HitBoxHandler>();
            hitboxHandler.DodgeLayers = dodgeLayers;
            hitboxHandler.VFXLifeTime = LifeTime;
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

                if(_effectsToApplyList.Count > 0)
                    effectApplier.SetEffects(_effectsToApplyList);
            }
        }


        return flyweight;
    }

}
