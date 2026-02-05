using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    [SerializeField] private List<EffectData> _effectsToApply;

    //private void OnEnable()
    //{
        
    //}
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
    public void ApplyEffect(GameObject sender,GameObject target)
    {
        if (_effectsToApply == null && GetComponent<HitBoxHandler>().Parryable)
        {
            Debug.LogWarning("No effect assigned to EffectApplier!");
            return;
        }

        EffectsManager manager = target.GetComponent<EffectsManager>();

        if (manager == null)
        {
            manager = target.AddComponent<EffectsManager>();
        }

        foreach (EffectData effect in _effectsToApply)
        {
            manager.AddEffect(effect);
        }
    }

    // Overload to apply effect to this GameObject
    public void ApplyEffect()
    {
        ApplyEffect(gameObject,gameObject);
    }

}