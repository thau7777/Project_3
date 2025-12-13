using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class AdvanceOneShotVFX : Flyweight
{
    new AdvanceOneShotVFXSettings settings => (AdvanceOneShotVFXSettings)base.settings;

    private VisualEffect _vfxGraph;

    private Coroutine _playCoroutine;
    private Coroutine _decalSpawnCoroutine;
    private void Awake()
    {
        _vfxGraph = GetComponent<VisualEffect>();
    }
    public void OnEnable()
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }

        if (_decalSpawnCoroutine != null)
        {
            StopCoroutine(_decalSpawnCoroutine);
            _decalSpawnCoroutine = null;
        }
    }
    public void OnDisable()
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }
        if (_decalSpawnCoroutine != null)
        {
            StopCoroutine(_decalSpawnCoroutine);
            _decalSpawnCoroutine = null;
        }
    }
    public void PlayEffect(float duration, float size, float decalDelayTime = 0, float decalDuration = 2)
    {
        _vfxGraph.SetFloat(settings.durationName, duration);
        _vfxGraph.SetFloat(settings.sizeName, size);
        _vfxGraph.SendEvent(settings.playEventName);

        _playCoroutine = StartCoroutine(PlayEffectCoroutine(duration));
        if(settings.decalEffectSettings)
        {
            if (decalDelayTime > duration)
                Debug.LogWarning("decal delay time is bigger than the effect lifeTime");
            else
            {
                if(decalDelayTime <= 0)
                    SpawnDecal(size, decalDuration);
                else
                    _decalSpawnCoroutine = StartCoroutine(SpawnDecalCoroutine(size, decalDelayTime, decalDuration));
            }
        }
            
    }

    private IEnumerator PlayEffectCoroutine(float duration)
    {
        yield return Helpers.GetWaitForSeconds(duration);
        ReturnToPool();
    }

    private IEnumerator SpawnDecalCoroutine(float size, float decalDelayTime, float decalDuration)
    {
        yield return Helpers.GetWaitForSeconds(decalDelayTime);
        SpawnDecal(size,decalDuration);
    }

    private void SpawnDecal(float size, float decalDuration)
    {
        DecalProjectorController decal = FlyweightFactory.Spawn(settings.decalEffectSettings).GetComponent<DecalProjectorController>();
        decal.transform.position = transform.position;
        decal.SetSize(size, size);
        decal.FadeOut(decalDuration);
    }


}
