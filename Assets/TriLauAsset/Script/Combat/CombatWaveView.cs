using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class CombatWaveView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI waveNumb;
        [SerializeField] private Transform enmiesViewParent;
        [SerializeField] private GameObject enemyViewPreb;

        public void SetUpWave(int waveNumb, WaveData waveData)
        {
            this.waveNumb.text = "Wave " + waveNumb;

            for (int i = 0; i < waveData.Enemies.Length; i++)
            {
                EnemyData enemyData = waveData.Enemies[i];

                if (enemyData != null)
                {
                    var enemyView = Instantiate(enemyViewPreb, enmiesViewParent);
                    CombatEnemyView combatEnemyView = enemyView.GetComponent<CombatEnemyView>();
                    EnemyDataSO enemyDataSO = WaveManager.Instance.GetEnemySOById(enemyData.EnemyId);
                    combatEnemyView.SetUp(enemyDataSO.enemyImage);
                }
            }
        }
    }
}