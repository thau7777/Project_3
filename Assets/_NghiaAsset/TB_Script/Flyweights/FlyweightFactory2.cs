using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Turnbase
{
    public class FlyweightFactory2 : PersistentSingleton<FlyweightFactory2>
    {

        readonly Dictionary<FlyweightType, IObjectPool<Flyweight2>> pools = new();


        public static Flyweight2 Spawn(FlyweightSettings2 settings) => instance.GetPoolFor(settings)?.Get();
        public static void ReturnToPool(Flyweight2 f) => instance.GetPoolFor(f.settings)?.Release(f);

        IObjectPool<Flyweight2> GetPoolFor(FlyweightSettings2 settings)
        {
            IObjectPool<Flyweight2> pool;

            if (pools.TryGetValue(settings.type, out pool)) return pool;

            pool = new ObjectPool<Flyweight2>(
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



    }

}