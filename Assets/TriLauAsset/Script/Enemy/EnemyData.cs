using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("Prefab")]
        public GameObject enemyPreb;

        [Header("Stats")]

        [LabelText("phys", LabelTextAttribute.LabelColor.gray)]
        public int phys;
        [LabelText("mag", LabelTextAttribute.LabelColor.cyan)]
        public int mag;
        [LabelText("health", LabelTextAttribute.LabelColor.green)]
        public int health;
        [LabelText("def", LabelTextAttribute.LabelColor.yellow)]
        public int def;
        [LabelText("resRate", LabelTextAttribute.LabelColor.orange)]
        public int resRate;
        [LabelText("attackSpeed", LabelTextAttribute.LabelColor.blue)]
        public float attackSpeed;
        [LabelText("critChance", LabelTextAttribute.LabelColor.red)]
        public float critChance;
        [Range(1.5f, 2.3f)]
        public float critMult = 1.5f;
    }
}