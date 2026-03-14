using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Turnbase
{
    public class FlyweightFactory_TB : Singleton<FlyweightFactory_TB>
    {

        readonly Dictionary<FlyweightType, IObjectPool<Flyweight_TB>> pools = new();


        public static Flyweight_TB Spawn(FlyweightSettings_TB settings) => instance.GetPoolFor(settings)?.Get();
        public static void ReturnToPool(Flyweight_TB f) => instance.GetPoolFor(f.settings)?.Release(f);

        IObjectPool<Flyweight_TB> GetPoolFor(FlyweightSettings_TB settings)
        {
            IObjectPool<Flyweight_TB> pool;

            if (pools.TryGetValue(settings.type, out pool)) return pool;

            pool = new ObjectPool<Flyweight_TB>(
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

        public void ClearAllPools()
        {
            var keysToClear = pools.Keys.ToList();

            foreach (var type in keysToClear)
            {
                if (pools.TryGetValue(type, out var pool))
                {
                    pool.Clear();

                    pools.Remove(type);
                }
            }
        }

    }

    public enum FlyweightType
    {
        TB_Impact_Wind,
        Tb_Projectile_Fire,
        Tb_DOT_Burn,
        Tb_Healing,
        Tb_MaxHeal,
        Tb_Buff_P_Attack,
        Tb_Buff_M_Attack,
        Tb_Shield,
        Tb_Paralysis,
        Tb_IceImpact,
        Tb_Ice,
        Tb_Meteorite,
        Tb_Summon_blue,
        Tb_Summon_red,
        Tb_Summon_green,
        Tb_Summon_yellow,
        Tb_CallOfThunder,
        TB_StonesHit,
        Tb_DefReduction,
        Tb_Buff_P_Def,
        Tb_Buff_M_Def,
        Tb_IceImpact2,
        Tb_IceCircle,
        Tb_DOT_Posion,
        Tb_DarkImpact1,
        Tb_DarkImpact2,
        Tb_LightningImpact1,
        Tb_LightningImpact2,
        Tb_Reduction_speed,

        Tb_SlashNormal,
        Tb_SlashDark,
        Tb_SlashLight,
        Tb_SlashWater,
        Tb_SlashFire,
        Tb_SlashPoison,
        Tb_SlashFrost,
        Tb_SlashHoly,

        Tb_Impact_Light,
        Tb_Impact_Poison,
        Tb_Impact_Water,
        Tb_Impact_Fire,
        Tb_Impact_Dark,
        Tb_Impact_Frost,
        Tb_Impact_Holy,

        Tb_Projectile_Light,
        Tb_Projectile_Poison,
        Tb_Projectile_Water,
        Tb_Projectile_Dark,
        Tb_Projectile_Frost,
        Tb_Projectile_Holy,

        Tb_Chot_Water,
        Tb_Chot_Frost1,
        Tb_Chot_Frost2,
        Tb_SlashDark2,

        Tb_baseAttack,
        Tb_Projectile_Gai1,
        Tb_Projectile_Gai2,

        Tb_Laser_Fire,
        Tb_Laser_Light,
        Tb_Laser_Water,
        Tb_Laser_Poison,
        Tb_Laser_Dark,
        Tb_Laser_Frost,
        Tb_Laser_Holy,

        Impact_Holy2,
        Circle_Ice,

        Spell_Lightning_2,
        Spell_ChainLightning,
        Basic_Charge_Buff,

        Spell_Poison_1,
        Spell_Poison_2,
        Spell_Poison_3,
        Spell_Poison_4,

        Spell_Fire_1,
        Spell_Fire_2,
        Spell_Fire_3,
        Spell_Fire_4,
        Spell_Fire_5,

        Spell_Water_1,
        Spell_Water_2,
        Spell_Water_3,
        Spell_Water_4,

        Spell_Ice_1, 
        Spell_Ice_2,
        Spell_Ice_3,
        Spell_Ice_4, 

        Spell_Holy_1,
        Spell_Holy_2,
        Spell_holy_3,
        Spell_Holy_4,

        Spell_Dark_1,
        Spell_Dark_2,
        Spell_Dark_3,
        Spell_Dark_4,
        EnemySpawnVFX,
        Parry,

















    }

}