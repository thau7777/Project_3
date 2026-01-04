using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Turnbase
{
    public class FlyweightFactory_TB : PersistentSingleton<FlyweightFactory_TB>
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











    }

}