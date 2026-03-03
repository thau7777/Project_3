using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "DataSO", menuName = "Scriptable Objects/DataSO")]
    public class DataSO : ScriptableObject
    {
        public bool isFrist = true;
        public Vector3 playerPosInMaze = new Vector3(-435, 10, -825);
        public SigilStorageSO sigilStorageSO;
        public int currentStep;
        public List<bool> matchResults = new List<bool>();
    }
}