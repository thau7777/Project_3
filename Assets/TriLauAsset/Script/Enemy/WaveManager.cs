using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class WaveManager : PersistentSingleton<WaveManager>
    {
        [SerializeField] private List<EnemyDataSO> enemyDataSOs;
        private int currentLevel = 1;
        private GroupWave groupWave;
        
        private EventBinding<WaveEvent> eventBinding;

        private void OnEnable()
        {
            eventBinding = new EventBinding<WaveEvent>(OnWaveEvent);
            EventBus<WaveEvent>.Register(eventBinding);
        }

        private void OnDisable()
        {
            EventBus<WaveEvent>.Deregister(eventBinding);
        }

        private void OnWaveEvent(WaveEvent waveEvent)
        {
            groupWave = waveEvent.GroupWave;
        }

        private void InitEnemy()
        {

        }

        public GroupWave GetCurrentWave()
        {
            return groupWave;
        }

        private void InitWave()
        {
            int waveCount = Random.Range(2, 4);
            
            groupWave = new GroupWave(waveCount);

            for (int i = 0; i < waveCount; i++)
            {
                int enemyCount = Random.Range(2, 4);
                
                WaveData waveData = new WaveData(enemyCount);

                for (int j = 0; j < enemyCount; j++)
                {
                    EnemyDataSO enemyDataSO = GetRandomEnemyDataSO();
                    
                    EnemyData enemyData = new EnemyData(currentLevel, enemyDataSO.enemyId,
                        enemyDataSO.phys, enemyDataSO.mag, enemyDataSO.fire, enemyDataSO.frost,
                        enemyDataSO.lightning, enemyDataSO.holy, enemyDataSO.dark, enemyDataSO.water,
                        enemyDataSO.health, enemyDataSO.phyDef, enemyDataSO.magDef, enemyDataSO.fireDef,
                        enemyDataSO.frostDef, enemyDataSO.lightning, enemyDataSO.holy, enemyDataSO.darkDef,
                        enemyDataSO.water, enemyDataSO.resRate, enemyDataSO.attackSpeed, enemyDataSO.critChance,
                        enemyDataSO.critMult);
                    
                    waveData.AddEnemy(j, enemyData);
                }

                groupWave.AddWaveData(i, waveData);
            }
        }

        private EnemyDataSO GetRandomEnemyDataSO()
        {
            int index = Random.Range(0, enemyDataSOs.Count);
            return enemyDataSOs[index];
        }
    }
}