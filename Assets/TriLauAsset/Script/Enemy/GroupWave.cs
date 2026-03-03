using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "GroupWave", menuName = "Scriptable Objects/GroupWave")]
    public class GroupWave : ScriptableObject
    {
        [SerializeField] private List<WaveData> enemyWaves;
    }
}