using System.Collections;
using UnityEngine;

public class SimpleOneShotVFX : Flyweight
{
    new SimpleOneShotVFXSettings settings => (SimpleOneShotVFXSettings)base.settings;
    private Coroutine _despawnCoroutine;
    private Coroutine _decalSpawnCoroutine;

    private void OnEnable()
    {
        if (_despawnCoroutine != null)
            StopCoroutine(_despawnCoroutine);

        _despawnCoroutine = StartCoroutine(LifetimeRoutine());

        if(_decalSpawnCoroutine != null)
        {
            StopCoroutine(_decalSpawnCoroutine);
            _decalSpawnCoroutine = null;
        }
    }

    private void OnDisable()
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
            _despawnCoroutine = null;
        }

        if (_decalSpawnCoroutine != null)
        {
            StopCoroutine(_decalSpawnCoroutine);
            _decalSpawnCoroutine = null;
        }
    }

    private IEnumerator LifetimeRoutine()
    {
        if(settings.decalEffectSettings)
        {
            if (settings.decalDelayTime > settings.LifeTime)
                Debug.LogWarning("decal delay time is bigger than the effect lifeTime");
            else
            {
                if (settings.decalDelayTime <= 0)
                    SpawnDecal(settings.decalSize, settings.decalDuration);
                else
                    _decalSpawnCoroutine = StartCoroutine(DecalSpawnCoroutine(settings.decalSize, settings.decalDelayTime, settings.decalDuration));
            }
        }

        yield return new WaitForSeconds(settings.LifeTime);
        FlyweightFactory.ReturnToPool(this);
    }
    private void SpawnDecal(float size, float decalDuration)
    {
        DecalProjectorController decal = FlyweightFactory.Spawn(settings.decalEffectSettings).GetComponent<DecalProjectorController>();
        decal.transform.position = transform.position;
        decal.SetSize(size, size);
        decal.FadeOut(decalDuration);
    }
    private IEnumerator DecalSpawnCoroutine(float size, float decalDelayTime, float decalDuration)
    {
        yield return Helpers.GetWaitForSeconds(decalDelayTime);
        SpawnDecal(size, decalDuration);
    }
}