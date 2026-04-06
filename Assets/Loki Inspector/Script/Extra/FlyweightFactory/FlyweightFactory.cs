using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlyweightFactory : Singleton<FlyweightFactory>
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
    IceExplosionVFX_Enemy_3,
    TopDownEnemyGolem,
    IceSpellVFX_Enemy_3,
    FlameThrower_Dragon,
    Projectile_Holy_Enemy_1,
    Explosion_Holy_Enemy_1,
    Laser_Holy_Enemy,
    EnemyTopDownBeholder,
    Spell_Holy_Enemy_1,
    EnemyTopDownEvilMage,
    EnemyTopDownSpecter,
    Slash_Holy_Enemy,
    EnemyTopDownBattleBee,
    EnemyTopDownFlyingDemon,
    Projectile_Lightning_Enemy_1,
    Explosion_Lightning_Enemy_1,
    Slash_Lightning_Enemy_1,
    EnemyTopDownWereWolf,
    EnemyTopDownChestMonster,
    EnemyTopDownCyclop,
    Cyclop_Small_Arrow,
    Cyclop_Big_Arrow,
    Explosion_Normal_Enemy_1,
    EnemyTopDownOrc,
    Slash_Circle_Normal_Enemy,
    EnemyTopDownCactus,
    Explosion_Poison_Enemy_1,
    Explosion_Poison_Enemy_2,
    EnemyTopDownMonsterPlant,
    Thrust_Poison_Enemy_1,
    Slash_Circle_Poison_Enemy,
    EnemyTopDownRatAssassin,
    EnemyTopDownCrab,
    Slash_Circle_Water_Enemy,
    EnemyTopDownFishman,
    Slash_Water_Enemy,
    Thrust_Water_Enemy,
    Spell_Stingray_1,
    EnemyTopDownStingray,
    ChainLightning_LineRenderer,
    ChainLightning_ImpactVFX,
    BasicChargeBuff,
    EnemySpawnVFX,
    PlayerDashVFX,
    StunVFX,
    Spell_Dark_1,
    Spell_Dark_2,
    Spell_Dark_3,
    Spell_Dark_4,
    Spell_Fire_1,
    Spell_Fire_2,
    Spell_Fire_3,
    Spell_Fire_4,
    Spell_Fire_5,
    Spell_Holy_1,
    Spell_Holy_2,
    Spell_Holy_3,
    Spell_Holy_4,
    Spell_Ice_1,
    Spell_Ice_2,
    Spell_Ice_3,
    Spell_Ice_4,
    Spell_Lightning_1,
    Spell_Lightning_2,
    Spell_Lightning_3,
    Spell_Lightning_4,
    Spell_Poison_1,
    Spell_Poison_2,
    Spell_Poison_3,
    Spell_Poison_4,
    Spell_Water_1,
    Spell_Water_2,
    Spell_Water_3,
    Spell_Water_4,
    MageParryVFX,
    CharacterEffect_Fire,
    CharacterEffect_Ice,
    CharacterEffect_Lightning,
    CharacterEffect_Poison, 
    EffectsIcon,
    Basic_Heal,
    EnemyEffectsIcon,
    Explosion_Poison_3,
    FloatingCombatText,
    ParrySuccessVFX,
    WaterAuraVFX,
    Spell_Stingray_2,
    Spell_Stingray_3,
    Spell_Stingray_4,

    FlyingSlash_Dark,
    BossSpawnVFX,
    SlashGroundDark,

    ShockWaveVFX,
    BeholderHeavenStrikeController,
    BeholderHeavenStrikeVfx,
    ImpactFrameVfx,
    BeholderProjectile,
}
