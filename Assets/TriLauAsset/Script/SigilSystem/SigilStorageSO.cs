using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "SigilStorageSO", menuName = "Sigil/SigilStorageSO")]
    public class SigilStorageSO : ScriptableObject
    {
        public SigilSO sigil_L;
        public SigilSO sigil_R;
        public SigilSO sigil_S;
        public SigilSO sigil_F;

        public SigilSO pSigil1;
        public SigilSO pSigil2;
        public SigilSO pSigil3;
        public SigilSO pSigil4;
        public SigilSO pSigil5;
        public SigilSO pSigil6;
        public SigilSO pSigil7;
        public SigilSO pSigil8;
    }
}