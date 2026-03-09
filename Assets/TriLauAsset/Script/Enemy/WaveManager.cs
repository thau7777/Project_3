using Cysharp.Threading.Tasks;
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

            //test purpose
            InitWave();
        }

        private void OnDisable()
        {
            EventBus<WaveEvent>.Deregister(eventBinding);
        }

        private void OnWaveEvent(WaveEvent waveEvent)
        {
            groupWave = waveEvent.GroupWave;
        }

        public UniTask CreateNewWave()
        {
            InitWave();

            return UniTask.CompletedTask;
        }    

        public EnemyDataSO GetEnemySOById(EnemyId id)
        {
            return enemyDataSOs.Find(so => so.enemyId == id);
        }

        public GroupWave GetCurrentWave()
        {
            return groupWave;
        }

        private void InitWave()
        {
            int waveCount = Random.Range(2, 5);
            
            groupWave = new GroupWave(waveCount);

            for (int i = 0; i < waveCount; i++)
            {
                int enemyCount;

                if (waveCount == 2)
                {
                    enemyCount = Random.Range(3, 6);
                }
                else if (waveCount == 3)
                {
                    enemyCount = Random.Range(2, 5);
                }
                else if (waveCount == 4)
                {
                    enemyCount = Random.Range(2, 4);
                }
                else enemyCount = 3;

                WaveData waveData = new WaveData(enemyCount);

                for (int j = 0; j < enemyCount; j++)
                {
                    EnemyDataSO enemyDataSO = GetRandomEnemyDataSO();
                    
                    EnemyData enemyData = new EnemyData(currentLevel, enemyDataSO.enemyId,
                        enemyDataSO.phys, enemyDataSO.mag, enemyDataSO.fire, enemyDataSO.frost,
                        enemyDataSO.lightning, enemyDataSO.holy, enemyDataSO.dark, enemyDataSO.water, enemyDataSO.poison,
                        enemyDataSO.health, enemyDataSO.phyDef, enemyDataSO.magDef, enemyDataSO.fireDef,
                        enemyDataSO.frostDef, enemyDataSO.lightning, enemyDataSO.holy, enemyDataSO.darkDef,
                        enemyDataSO.water, enemyDataSO.poisonDef, enemyDataSO.resRate, enemyDataSO.attackSpeed, enemyDataSO.critChance,
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