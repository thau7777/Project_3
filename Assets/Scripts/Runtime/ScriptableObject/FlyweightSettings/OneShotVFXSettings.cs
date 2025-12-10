using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New OneShotVFX Settings", menuName = "Scriptable Objects/Flyweight/OneShotVFX Settings")]
public class OneShotVFXSettings : FlyweightSettings
{
    [field:SerializeField]
    public float DespawnDelay { get; private set; }

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
    private int _damage = 40;

    [SerializeField]
    [ShowIf("_canDealDamage")]
    private int _knockBackForce = 10;




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
    private List<Effect> _effectsToApplyList = new();


    private bool _hasHitBox;
    public bool HasHitBox => _hasHitBox;


    [ShowIf("_hasHitBox")]
    [MinMaxSlider(0,1)]
    public Vector2 hitboxOnOffTime;

    [ShowIf("_hasHitBox")]
    public LayerMask dodgeLayers;

    private void OnValidate()
    {
        _hasHitBox = _canDealDamage || _canApplyEffect;
    }
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        ParticleSystem particleSystem = prefab.GetComponentInChildren<ParticleSystem>();
        DespawnDelay = particleSystem != null ? particleSystem.main.duration : 1f;

        var flyweight = go.GetOrAdd<OneShotVFX>();
        flyweight.settings = this;

        if (HasHitBox)
        {
            go.GetOrAdd<HitBoxHandler>().DodgeLayers = dodgeLayers;

            if (CanDealDamage)
            {
                var damageDealer = go.GetOrAdd<DamageDealer>();
                damageDealer.Damage = _damage;
                damageDealer.KnockbackForce = _knockBackForce;
            }
            if (CanApplyEffect)
            {
                var effectApplier = go.GetOrAdd<EffectApplier>();
                effectApplier.SetEffects(_effectsToApplyList);
            }
        }


        return flyweight;
    }

}
