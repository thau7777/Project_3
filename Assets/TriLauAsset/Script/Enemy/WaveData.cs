using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "WaveData", menuName = "Wave/WaveData")]
    public class WaveData : ScriptableObject
    {
        public List<Wave> waves;
        public int waveDelay;
    }

    [Serializable]
    public class Wave
    {
        [SerializeField] private int enemyCount;
        [SerializeField] private int delay;
        [SerializeField] private EnemyData enemyData;

        public int EnemyCount => enemyCount;
        public int Delay => delay;
        public EnemyData EnemyData => enemyData;
    }
}