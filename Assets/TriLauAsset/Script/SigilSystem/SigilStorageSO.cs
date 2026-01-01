using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "SigilStorageSO", menuName = "Sigil/SigilStorageSO")]
    public class SigilStorageSO : ScriptableObject
    {
        public Texture2D sigilTexture0;
        public Texture2D sigilTexture1;
        public Texture2D sigilTexture2;
        public Texture2D sigilTexture3;

        public Texture2D pSigil1;
        public Texture2D pSigil2;
        public Texture2D pSigil3;
        public Texture2D pSigil4;
        public Texture2D pSigil5;
        public Texture2D pSigil6;
        public Texture2D pSigil7;
        public Texture2D pSigil8;
        public Texture2D pSigil9;
        public Texture2D pSigil10;
        public Texture2D pSigil11;
        public Texture2D pSigil12;

        private void OnDestroy()
        {
            sigilTexture0 = null;
            sigilTexture1 = null;
            sigilTexture2 = null;
            sigilTexture3 = null;

            pSigil1 = null;
            pSigil2 = null;
            pSigil3 = null;
            pSigil4 = null;
            pSigil5 = null;
            pSigil6 = null;
            pSigil7 = null;
            pSigil8 = null;
            pSigil9 = null;
            pSigil10 = null;
            pSigil11 = null;
            pSigil12 = null;
        }
    }
}