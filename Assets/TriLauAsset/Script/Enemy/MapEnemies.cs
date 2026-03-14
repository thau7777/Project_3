using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "MapEnemies", menuName = "Scriptable Objects/MapEnemies")]
    public class MapEnemies : ScriptableObject
    {
        public List<EnemyDataSO> enemies;
    }
}