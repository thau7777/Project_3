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

    MageProjectile,
    MageProjectileExplosion,

    TopDownEnemySlime,
    TopDownEnemyTurtle,
    TopDownEnemyBat,
    TopDownEnemyBlackKnight,
    TopDownEnemyStraightAttackVFX,
    TopDownEnemySlashDark,
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

    SlimeSlamChargeVFX,
    SlimeSlamVFX,

    BasicDecal,
    TopDownEnemyDemonKing,
    TopDownEnemyDragon,
    AttackWarningVFX,
    Charge_Fire_1,
    Projectile_Fire_1,
    Explosion_Fire_1,
    TopDownEnemyNagaWizard,
    TopDownEnemySalamander,
    SpearThrustIceVFX,
    SlashIceVFX,
    TopDownEnemyBishopKnight,
    IceExplosionVFX_3,
    TopDownEnemyGolem,
    IceSpellVFX_3,
    FlameThrower_Dragon,
    Projectile_Holy_1,
    Explosion_Holy_1,
    Laser_Holy,
    EnemyTopDownBeholder,
    Spell_Holy_1,
    EnemyTopDownEvilMage,
    EnemyTopDownSpecter,
    Slash_Holy,
    EnemyTopDownBattleBee,
    EnemyTopDownFlyingDemon,
    Projectile_Lightning_1,
    Explosion_Lightning_1,
    Slash_Lightning_1,
    EnemyTopDownWereWolf,
    EnemyTopDownChestMonster,
    EnemyTopDownCyclop,
    Cyclop_Small_Arrow,
    Cyclop_Big_Arrow,
    Explosion_Normal_1,
    EnemyTopDownOrc,
    Slash_Circle_Normal,
    EnemyTopDownCactus,
    Explosion_Poison_1,
    Explosion_Poison_2,
    EnemyTopDownMonsterPlant,
    Thrust_Poison_1,
    Slash_Circle_Poison,
    EnemyTopDownRatAssassin,
    EnemyTopDownCrab,
    Slash_Circle_Water,
    EnemyTopDownFishman,
    Slash_Water,
    Thrust_Water,
    Spell_Water_1,
    EnemyTopDownStingray,
    ChainLightning_LineRenderer,
    ChainLightning_ImpactVFX,
    BasicChargeBuff,
}
