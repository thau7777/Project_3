using System.Collections;
using UnityEngine;

public class OneShotVFX : Flyweight
{
    new OneShotVFXSettings settings => (OneShotVFXSettings)base.settings;

    private void OnEnable()
    {
        StartCoroutine(DespawnAfterDelay(settings.DespawnDelay));
    }
    IEnumerator DespawnAfterDelay(float delay)
    {
        yield return Helpers.GetWaitForSeconds(delay);
        FlyweightFactory.ReturnToPool(this);
    }
}
