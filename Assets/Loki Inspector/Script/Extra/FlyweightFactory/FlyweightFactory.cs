using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlyweightFactory : PersistentSingleton<FlyweightFactory>
{

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
            settings.collectionCheck,
            settings.defaultCapacity,
            settings.maxPoolSize
        );
        pools.Add(settings.type, pool);
        return pool;
    }
    
}
public enum FlyweightType
{
    SmallSwordSlashVFX,
    ShieldBashVFX,
    SlashHitVFX,

    PurpleProjectile,
    PurpleHitVFX,
    BigPurpleProjectile,
    BigPurpleHitVFX,
    FireBallProjectile,
    FireBallExplosionVFX,

    TopDownEnemySlime,
    TopDownEnemyTurtle,
    TopDownEnemyStraightAttackVFX,
    BasicPlayerOnHitImpactVFX,

    TurnbaseSpawnPet,

    IndicatorCircleAlly,
    IndicatorCircleEnemy,
    IndicatorStraightDashAlly,
    IndicatorStraightAlly,
    IndicatorStraightDashEnemy,
    IndicatorStraightEnemy,
    IndicatorConeAlly,
    IndicatorConeEnemy,
    IndicatorRectangleAlly,
    IndicatorRectangleEnemy,

    BasicSummonerAttack,
    SummonerTargetVFX,
    DogSlash_1,
    DogSlash_2,

    BasicChargeVFX,
}
