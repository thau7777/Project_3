using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class OneShotVFX_TB : Flyweight_TB
    {
        new OneShotVFXSettings_TB settings => (OneShotVFXSettings_TB)base.settings;

        public void SetupDespawn()
        {
            StartCoroutine(DespawnAfterDelay(settings.DespawnDelay));
        }

        IEnumerator DespawnAfterDelay(float delay)
        {
            yield return Helpers.GetWaitForSeconds(delay);
            FlyweightFactory_TB.ReturnToPool(this);
        }
    }
}