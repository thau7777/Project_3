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

    public class WaveManager : PersistentSingleton<WaveManager>, IGameData
    {
        [SerializeField] private List<EnemyInMap> enemiesInMap;
        private int currentLevel = 1;
        private GroupWave groupWave;
        
        [SerializeField] private MapEnemies mapEnemies;

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
            int waveCount = UnityEngine.Random.Range(2, 5);
            
            GroupWave groupWave = new GroupWave(waveCount);

            for (int i = 0; i < waveCount; i++)
            {
                int enemyCount;

                if (waveCount == 2)
                {
                    enemyCount = UnityEngine.Random.Range(3, 6);
                }
                else if (waveCount == 3)
                {
                    enemyCount = UnityEngine.Random.Range(2, 5);
                }
                else if (waveCount == 4)
                {
                    enemyCount = UnityEngine.Random.Range(2, 4);
                }
                else enemyCount = 3;

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
            currentLevel = 1;
            groupWave = null;
            return UniTask.CompletedTask;
        }
    }
}