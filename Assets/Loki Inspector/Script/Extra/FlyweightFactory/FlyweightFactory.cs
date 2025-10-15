using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlyweightFactory : PersistentSingleton<FlyweightFactory>
{
    [SerializeField] bool collectionCheck = true;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxPoolSize = 100;

    readonly Dictionary<FlyweightType, IObjectPool<Flyweight>> pools = new();


    public static Flyweight Spawn(FlyweightSettings settings) => instance.GetPoolFor(settings)?.Get();
    public static void ReturnToPool(Flyweight f) => instance.GetPoolFor(f.settings)?.Release(f);

    IObjectPool<Flyweight> GetPoolFor(FlyweightSettings settings)
    {
        IObjectPool<Flyweight> pool;

        if (pools.TryGetValue(settings.type, out pool)) return pool;

        pool = new ObjectPool<Flyweight>(
            settings.Create,
            settings.OnGet,
            settings.OnRelease,
            settings.OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
        );
        pools.Add(settings.type, pool);
        return pool;
    }
    
}
public enum FlyweightType
{
    SmallSwordSlashVFX,
    ShieldBashVFX,
    PurpleProjectile,
    PurpleHitVFX,
    BigPurpleProjectile,
    BigPurpleHitVFX,
}
