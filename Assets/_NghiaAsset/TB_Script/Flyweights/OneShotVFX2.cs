using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class OneShotVFX2 : Flyweight2
    {
        new OneShotVFXSettings2 settings => (OneShotVFXSettings2)base.settings;

        private void OnEnable()
        {
            StartCoroutine(DespawnAfterDelay(settings.DespawnDelay));
        }
        IEnumerator DespawnAfterDelay(float delay)
        {
            yield return Helpers.GetWaitForSeconds(delay);
            FlyweightFactory2.ReturnToPool(this);
        }
    }

}