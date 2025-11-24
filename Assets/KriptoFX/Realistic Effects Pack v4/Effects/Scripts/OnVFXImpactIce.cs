using UnityEngine;

namespace Turnbase
{
    public class OnVFXImpactIce : MonoBehaviour
    {
        public GameObject parentVFX;
        public GameObject vfxImpact;


        private void OnDisable()
        {
            Instantiate(vfxImpact,parentVFX.transform.position,parentVFX.transform.rotation);
        }



    }

}
