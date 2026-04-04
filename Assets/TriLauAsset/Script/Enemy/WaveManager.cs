using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class EnemyInMap
    {
        [SerializeField] public EMap mapType;
        [SerializeField] public MapEnemies enemies;
    }

    [Serializable] 
    public class BossInMap
    {
        public EMap mapType;
        public EnemyDataSO bossConfig;
    }

    public struct WaveCountRate
    {
        public int count;
        public int rate;
    }

    public struct EnemyCountRate
    {
        public int count;
        public int rate;
    }

    public class WaveManager : PersistentSingleton<WaveManager>, IGameData
    {
        [SerializeField] private List<EnemyInMap> enemiesInMap;
        [SerializeField] private MapEnemies mapEnemies;
        [SerializeField] private List<BossInMap> bossInMap;
        [SerializeField] private EnemyDataSO bossSO;

        private GroupWave groupWave;

        private EventBinding<WaveEvent> eventBinding;

        private void OnEnable()
        {
            eventBinding = new EventBinding<WaveEvent>(OnWaveEvent);
            EventBus<WaveEvent>.Register(eventBinding);

            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            EventBus<WaveEvent>.Deregister(eventBinding);

            GameSystemManager.Instance.Unregister(this);
        }

        private void OnWaveEvent(WaveEvent waveEvent)
        {
            groupWave = waveEvent.GroupWave;
            CombatManager.Instance.CombatData.SetGroupWave(groupWave);
        }

        //public UniTask CreateNewWave()
        //{
        //    InitWave();

        //    return UniTask.CompletedTask;
        //}    

        public EnemyDataSO GetEnemySOById(EnemyId id)
        {
            return mapEnemies.enemies.Find(so => so.enemyId == id);
        }

        public GroupWave GetCurrentWave()
        {
            return groupWave;
        }

        public GroupWave CreateNewWave()
        {
            List<WaveCountRate> waveCountRates = new List<WaveCountRate>();

            List<EnemyCountRate> enemyCountRates = new List<EnemyCountRate>();

            int currentLevel = MatchManager.Instance.MatchData.CurrentStep;
            Debug.Log("currentLevel " +  currentLevel);

            if (currentLevel >= 0 && currentLevel < 6)
            {
                waveCountRates = new List<WaveCountRate>()
                {
                    new WaveCountRate()
                    {
                        count = 2,
                        rate = 60,
                    },
                    new WaveCountRate()
                    {
                        count = 3,
                        rate = 40,
                    }
                };

                enemyCountRates = new List<EnemyCountRate>()
                {
                    new EnemyCountRate()
                    {
                        count = 2,
                        rate = 70,
                    },
                    new EnemyCountRate()
                    {
                        count = 3,
                        rate = 30,
                    }
                };
            }
            else if (currentLevel >= 6 && currentLevel < 15)
            {
                waveCountRates = new List<WaveCountRate>()
                {
                    new WaveCountRate()
                    {
                        count = 2,
                        rate = 30,
                    },
                    new WaveCountRate()
                    {
                        count = 3,
                        rate = 70,
                    }
                };

                enemyCountRates = new List<EnemyCountRate>()
                {
                    new EnemyCountRate()
                    {
                        count = 2,
                        rate = 20,
                    },
                    new EnemyCountRate()
                    {
                        count = 3,
                        rate = 60,
                    },
                    new EnemyCountRate()
                    {
                        count = 4,
                        rate = 20,
                    }
                };
            }
            else if (currentLevel >= 15 && currentLevel < 22)
            {
                waveCountRates = new List<WaveCountRate>()
                {
                    new WaveCountRate()
                    {
                        count = 2,
                        rate = 20,
                    },
                    new WaveCountRate()
                    {
                        count = 3,
                        rate = 60,
                    },
                    new WaveCountRate()
                    {
                        count = 4,
                        rate = 20,
                    },
                };

                enemyCountRates = new List<EnemyCountRate>()
                {
                    new EnemyCountRate()
                    {
                        count = 3,
                        rate = 40,
                    },
                    new EnemyCountRate()
                    {
                        count = 4,
                        rate = 40,
                    },
                    new EnemyCountRate()
                    {
                        count = 5,
                        rate = 20,
                    }
                };
            }
            else if (currentLevel >= 22 && currentLevel < 28)
            {
                waveCountRates = new List<WaveCountRate>()
                {
                    new WaveCountRate()
                    {
                        count = 3,
                        rate = 30,
                    },
                    new WaveCountRate()
                    {
                        count = 4,
                        rate = 70,
                    },
                };


                enemyCountRates = new List<EnemyCountRate>()
                {
                    new EnemyCountRate()
                    {
                        count = 3,
                        rate = 20,
                    },
                    new EnemyCountRate()
                    {
                        count = 4,
                        rate = 40,
                    },
                    new EnemyCountRate()
                    {
                        count = 5,
                        rate = 40,
                    }
                };
            }
            else if (currentLevel >= 28)
            {
                waveCountRates = new List<WaveCountRate>()
                {
                    new WaveCountRate()
                    {
                        count = 3,
                        rate = 20,
                    },
                    new WaveCountRate()
                    {
                        count = 4,
                        rate = 40,
                    },
                    new WaveCountRate()
                    {
                        count = 5,
                        rate = 40,
                    }
                };

                enemyCountRates = new List<EnemyCountRate>()
                {
                    new EnemyCountRate()
                    {
                        count = 3,
                        rate = 20,
                    },
                    new EnemyCountRate()
                    {
                        count = 4,
                        rate = 30,
                    },
                    new EnemyCountRate()
                    {
                        count = 5,
                        rate = 50,
                    }
                };
            }



            int waveCount = 0;
            int waveRandomRate = UnityEngine.Random.Range(1, 100);
            int currentWaveRate = 0;
            foreach (WaveCountRate rate in waveCountRates)
            {
                currentWaveRate += rate.rate;
                if (currentWaveRate > waveRandomRate)
                {
                    waveCount = rate.count;
                    break;
                }
            }

            GroupWave groupWave = new GroupWave(waveCount);

            for (int i = 0; i < waveCount; i++)
            {
                int enemyCount = 0;

                int randomRate = UnityEngine.Random.Range(1, 100);
                int currentRate = 0;

                foreach(EnemyCountRate rate in enemyCountRates)
                {
                    currentRate += rate.rate;
                    if (currentRate > randomRate)
                    {
                        enemyCount = rate.count;
                        break;
                    }
                }

                WaveData waveData = new WaveData(enemyCount);

                for (int j = 0; j < enemyCount; j++)
                {
                    EnemyDataSO enemyDataSO = GetRandomEnemyDataSO();
                    
                    EnemyData enemyData = new EnemyData(currentLevel, enemyDataSO.enemyId,
                        enemyDataSO.phys, enemyDataSO.mag, enemyDataSO.fire, enemyDataSO.frost,
                        enemyDataSO.lightning, enemyDataSO.holy, enemyDataSO.dark, enemyDataSO.water, enemyDataSO.poison,
                        enemyDataSO.health, enemyDataSO.stamina, enemyDataSO.phyDef, enemyDataSO.magDef, enemyDataSO.fireDef,
                        enemyDataSO.frostDef, enemyDataSO.lightning, enemyDataSO.holy, enemyDataSO.darkDef,
                        enemyDataSO.water, enemyDataSO.poisonDef, enemyDataSO.resRate, enemyDataSO.attackSpeed, enemyDataSO.critChance,
                        enemyDataSO.critMult);
                    
                    waveData.AddEnemy(j, enemyData);
                }

                groupWave.AddWaveData(i, waveData);
            }

            return groupWave;
        }

        public GroupWave CreateBossWave()
        {
            int currentLevel = MatchManager.Instance.MatchData.CurrentStep;

            GroupWave groupWave = new GroupWave(1);

            WaveData waveData = new WaveData(1);

            EnemyData bossData = new EnemyData(currentLevel, bossSO.enemyId,
                        bossSO.phys, bossSO.mag, bossSO.fire, bossSO.frost,
                        bossSO.lightning, bossSO.holy, bossSO.dark, bossSO.water, bossSO.poison,
                        bossSO.health, bossSO.stamina, bossSO.phyDef, bossSO.magDef, bossSO.fireDef,
                        bossSO.frostDef, bossSO.lightning, bossSO.holy, bossSO.darkDef,
                        bossSO.water, bossSO.poisonDef, bossSO.resRate, bossSO.attackSpeed, bossSO.critChance,
                        bossSO.critMult);

            waveData.AddEnemy(0, bossData);

            groupWave.AddWaveData(0, waveData);
            
            return groupWave;
        }

        private EnemyDataSO GetRandomEnemyDataSO()
        {
            int index = UnityEngine.Random.Range(0, mapEnemies.enemies.Count);
            return mapEnemies.enemies[index];
        }

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                mapEnemies = enemiesInMap.Find(m => m.mapType == data.MatchData.MapType).enemies;

                bossSO = bossInMap.Find(m => m.mapType == data.MatchData.MapType).bossConfig;
                
                if (data.MatchData.CombatData != null)
                {
                    groupWave = data.MatchData.CombatData.GroupWave;
                }
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            return;
        }

        public UniTask NewGame()
        {
            groupWave = null;
            return UniTask.CompletedTask;
        }
    }
}