using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class CombatEnemyView : MonoBehaviour
    {
        [SerializeField] private Image enemyImg;

        public void SetUp(Sprite sprite)
        {
            enemyImg.sprite = sprite;
        }
    }
}