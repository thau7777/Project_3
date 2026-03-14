using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class PlanetEnemyView : MonoBehaviour
    {
        [SerializeField] private Image enemyImg;
        [SerializeField] private TextMeshProUGUI enemyName;

        public void SetEnemyData(EnemyDataSO enemyData)
        {
            if (enemyData != null)
            {
                enemyImg.sprite = enemyData.enemyImage;
                enemyName.text = enemyData.name;
            }
        }
    }
}