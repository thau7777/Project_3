using Ami.BroAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New Straight Projectile Settings", menuName = "Scriptable Objects/Flyweight/Straight Projectile Settings")]
public class StraightProjectileSettings : FlyweightSettings
{
    public float defaultSize = 1;

    [field: SerializeField]
    public OneShotVFXSettings ProjectileImpactVFX { get; set; }

    [field: SerializeField]
    public LayerMask DodgeLayers { get; private set; }

    public float delayDurationToDespawn = 0f;

    [Header("Lifetime")]
    public bool useLifeTime = false;
    [ShowIf("useLifeTime"), TabGroup("LifeTimeSettings")] public float lifeTime = 3f;

    [ShowIf("useLifeTime"), TabGroup("LifeTimeSettings")] public bool useSpeedCurve = false;
    [ShowIf("useLifeTime"), TabGroup("LifeTimeSettings")] public AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);


    public bool canDealDamageWhileStopping = true;
    public bool despawnOnHit = true;
    public bool canDealDamageByProjectile;

    [ShowIf("canDealDamageByProjectile"), TabGroup("DamageProjectileSettings")] public ElementalType projectileDamageElementType;

    [ShowIf("canDealDamageByProjectile"), TabGroup("DamageProjectileSettings")] public bool canApplyEffects;
    [ShowIf("canDealDamageByProjectile"), TabGroup("DamageProjectileSettings")] public bool pickRandomEffectFromList;
    [ShowIf("canDealDamageByProjectile"), TabGroup("DamageProjectileSettings")] public List<EffectData> effectsToApplyList;

    public bool hasSound = false;

    [ShowIf("hasSound"), TabGroup("Sound")] public SoundID sfx;
    [ShowIf("hasSound"), TabGroup("Sound")] public float soundDelayTime;
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        var flyweight = go.GetOrAdd<StraightProjectile>();
        flyweight.settings = this;

        return flyweight;
    }
    public override void OnGet(Flyweight f)
    {

        f.transform.ForEveryChildDeep(child =>
        {
            var ps = child.GetComponent<ParticleSystem>();
            var trail = child.GetComponent<TrailRenderer>();

            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play(true);
            }

            if (trail != null)
            {
                trail.enabled = true;
                trail.Clear();
            }
        });

        var rootPS = f.GetComponent<ParticleSystem>();
        var rootTrail = f.GetComponent<TrailRenderer>();

        if (rootPS != null)
        {
            rootPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            rootPS.Play(true);
        }

        if (rootTrail != null)
        {
            rootTrail.enabled = true;
            rootTrail.Clear();
        }

        base.OnGet(f);
    }


    public override void OnRelease(Flyweight f)
    {
        base.OnRelease(f);

        f.transform.ForEveryChildDeep(child =>
        {
            var ps = child.GetComponent<ParticleSystem>();
            var trail = child.GetComponent<TrailRenderer>();

            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (trail != null)
            {
                trail.enabled = false; // IMPORTANT: disable BEFORE clearing
                trail.Clear();
            }
        });

        var rootPS = f.GetComponent<ParticleSystem>();
        var rootTrail = f.GetComponent<TrailRenderer>();

        if (rootPS != null)
            rootPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (rootTrail != null)
        {
            rootTrail.enabled = false;
            rootTrail.Clear();
        }
    }



}
