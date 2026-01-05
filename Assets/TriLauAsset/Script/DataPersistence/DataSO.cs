using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "DataSO", menuName = "Scriptable Objects/DataSO")]
    public class DataSO : ScriptableObject
    {
        public bool isFrist = true;
        public Vector3 playerPosInMaze;
        public SigilStorageSO sigilStorageSO;
        public int currentStep;
        public List<bool> matchResults = new List<bool>();
    }
}