using UnityEngine;

namespace Turnbase
{
    public class DamagePopupSpawn : MonoBehaviour
    {
        private static DamagePopupSpawn _i;

        public static DamagePopupSpawn i
        {
            get
            {
                if (_i == null)
                {
                    _i = FindFirstObjectByType<DamagePopupSpawn>();
                }
                return _i;
            }
        }

        public GameObject pfdamagePopup;

    }
}