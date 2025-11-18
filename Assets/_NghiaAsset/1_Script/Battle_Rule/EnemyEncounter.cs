using UnityEngine;

namespace Turnbase
{    
    [System.Serializable]
    public class EnemyWave
    {
        public Character[] enemiesInWave;
    }

    [CreateAssetMenu(fileName = "Encounter_MultiWave", menuName = "Battle/Multi-Wave Encounter Config", order = 1)]
    public class EnemyEncounter : ScriptableObject
    {
        [Header("Cấu hình Đợt Quái")]
        public EnemyWave[] waves;
    }
}