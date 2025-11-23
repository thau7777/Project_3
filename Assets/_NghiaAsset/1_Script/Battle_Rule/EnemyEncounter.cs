using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Encounter_", menuName = "Battle/Enemy Encounter Config", order = 1)]
    
    [System.Serializable]
    public class EnemyWave
    {
        [Tooltip("Danh sách các Prefab kẻ địch trong đợt này.")]
        public Character[] enemiesInWave;
    }

    [CreateAssetMenu(fileName = "Encounter_MultiWave", menuName = "Battle/Multi-Wave Encounter Config", order = 1)]
    public class EnemyEncounter : ScriptableObject
    {
        [Header("Cấu hình Đợt Quái")]
        public EnemyWave[] waves;
    }
}