using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class OneShotVFX_TB : Flyweight_TB
    {
        new OneShotVFXSettings_TB settings => (OneShotVFXSettings_TB)base.settings;

        public override void Initialize(Vector3 position, Quaternion rotation)
        {
            base.Initialize(position, rotation);

            StopAllCoroutines();

            StartCoroutine(DespawnAfterDelay(settings.DespawnDelay));
        }

        IEnumerator DespawnAfterDelay(float delay)
        {
            yield return Helpers.GetWaitForSeconds(delay);
            FlyweightFactory_TB.ReturnToPool(this);
        }
    }
}