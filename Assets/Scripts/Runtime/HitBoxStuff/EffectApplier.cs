using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    [SerializeField] private List<EffectData> _effectsToApply;

    private void Awake()
    {
        if (TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.OnColliderHit.AddListener(ApplyEffect);
        }
    }
    private void OnDestroy()
    {
        if (TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.OnColliderHit.RemoveListener(ApplyEffect);
        }
    }
    public void SetEffects(List<EffectData> effectList)
    {
        _effectsToApply = effectList;
    }
    public void ApplyEffect(GameObject sender , GameObject hitOrigin, GameObject target)
    {
        if (_effectsToApply == null ||
            TryGetComponent<HitBoxHandler>(out var hitboxHandler) &&
            hitboxHandler.ParryAble &&
            target.TryGetComponent<PlayerTopDownStateDriver>(out var player) && 
            player.IsParrying)
        {
            return;
        }

        EffectsManager manager = target.GetOrAdd<EffectsManager>();

        OneShotVFXSettings oneShotVFXSettings = GetComponent<OneShotVFX>().settings as OneShotVFXSettings;
        if (oneShotVFXSettings.pickRandomEffectFromList)
        {
            EffectData randomEffect = _effectsToApply[Random.Range(0, _effectsToApply.Count)];
            manager.AddEffect(randomEffect);
        }
        else
        {
            foreach (EffectData effect in _effectsToApply)
            {
                manager.AddEffect(effect);
            }
        }
            
    }

    // Overload to apply effect to this GameObject
    public void ApplyEffectSelf()
    {
        ApplyEffect(gameObject,gameObject,gameObject);
    }

}