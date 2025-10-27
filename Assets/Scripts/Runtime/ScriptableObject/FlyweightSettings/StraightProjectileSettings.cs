using UnityEngine;

[CreateAssetMenu(fileName = " New Straight Projectile Settings", menuName = "Scriptable Objects/Flyweight/Straight Projectile Settings")]
public class StraightProjectileSettings : FlyweightSettings
{
    [field: SerializeField]
    public float DespawnDelay { get; private set; } = 5f;

    [field: SerializeField]
    public OneShotVFXSettings HitVFXSettings { get; private set; }

    [field: SerializeField]
    public LayerMask DodgeLayers { get; private set; }
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
        base.OnGet(f);

        // Stop, clear and restart all ParticleSystems in this object (including children)
        f.transform.ForEveryChild(child =>
        {
            var ps = child.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play(true);
            }
        });

        // Also include root particle system if it has one
        var rootPS = f.GetComponent<ParticleSystem>();
        if (rootPS != null)
        {
            rootPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            rootPS.Play(true);
        }
    }
    public override void OnRelease(Flyweight f)
    {
        base.OnRelease(f);

        // Stop all ParticleSystems (including trails) to reset them
        f.transform.ForEveryChild(child =>
        {
            var ps = child.GetComponent<ParticleSystem>();
            var trail = child.GetComponent<TrailRenderer>();
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (trail != null)
                trail.Clear();
        });

        // Stop root PS if any
        var rootPS = f.GetComponent<ParticleSystem>();
        var trailRoot = f.GetComponent<TrailRenderer>();
        if (rootPS != null)
            rootPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (trailRoot != null)
            trailRoot.Clear();

    }

}
