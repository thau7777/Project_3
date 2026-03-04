using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "SigilStorageSO", menuName = "Sigil/SigilStorageSO")]
    public class SigilStorageSO : ScriptableObject
    {
        public List<SigilSO> activeSigils;
        public List<SigilSO> passiveSigils;
    }
}