using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    [SerializeField] private List<EffectData> _effectsToApply;

    private GameObject _sender;
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
    public void SetUpForParticle(GameObject sender)
    {
        _sender = sender;
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

        FlyweightSettings flyweightSettings = GetComponent<Flyweight>().settings as FlyweightSettings;
        if(flyweightSettings is OneShotVFXSettings oneShotVFXSettings)
        {
            if (oneShotVFXSettings.pickRandomEffectFromList)
            {
                EffectData randomEffect = _effectsToApply[Random.Range(0, _effectsToApply.Count)];
                manager.AddEffect(sender,randomEffect);
            }
            else
            {
                foreach (EffectData effect in _effectsToApply)
                {
                    manager.AddEffect(sender,effect);
                }
            }

        }else if(flyweightSettings is StraightProjectileSettings straightProjectileSettings)
        {
            if (straightProjectileSettings.pickRandomEffectFromList)
            {
                EffectData randomEffect = _effectsToApply[Random.Range(0, _effectsToApply.Count)];
                manager.AddEffect(sender, randomEffect);
            }
            else
            {
                foreach (EffectData effect in _effectsToApply)
                {
                    manager.AddEffect(sender, effect);
                }
            }
        }
            
    }

    public void ApplyEffect(GameObject target)
    {
        ApplyEffect(_sender,gameObject, target);
    }

}