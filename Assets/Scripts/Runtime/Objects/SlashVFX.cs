using System.Collections;
using UnityEngine;

public class SlashVFX : Flyweight
{
    new SlashVFXSetting settings => (SlashVFXSetting)base.settings;

    private void OnEnable()
    {
        StartCoroutine(DespawnAfterDelay(settings.despawnDelay));
    }
    IEnumerator DespawnAfterDelay(float delay)
    {
        yield return Helpers.GetWaitForSeconds(delay);
        FlyweightFactory.ReturnToPool(this);
    }
}
