using UnityEngine;

namespace Turnbase
{
    public class OnVFXImpactIce : MonoBehaviour
    {
        public FlyweightSettings_TB impactIceSetting;


        private void OnDisable()
        {
            var spawnedImpactIce = FlyweightFactory_TB.Spawn(impactIceSetting);

            if(spawnedImpactIce)
            {
                spawnedImpactIce.transform.position = transform.position;
                spawnedImpactIce.transform.rotation = transform.rotation;
                spawnedImpactIce.transform.localScale = gameObject.transform.localScale;

                spawnedImpactIce.gameObject.SetActive(true);
            }
            
        }



    }

}
