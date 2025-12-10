using System.Collections.Generic;
using UnityEngine;

public class EffectApplier : MonoBehaviour
{
    [SerializeField] private List<Effect> _effectsToApply;

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
    public void SetEffects(List<Effect> effectList)
    {
        _effectsToApply = effectList;
    }
    public void ApplyEffect(GameObject target)
    {
        if (_effectsToApply == null)
        {
            Debug.LogWarning("No effect assigned to EffectApplier!");
            return;
        }

        EffectsManager manager = target.GetComponent<EffectsManager>();

        if (manager == null)
        {
            manager = target.AddComponent<EffectsManager>();
        }

        foreach (Effect effect in _effectsToApply)
        {
            manager.AddEffect(effect);
        }
    }

    // Overload to apply effect to this GameObject
    public void ApplyEffect()
    {
        ApplyEffect(gameObject);
    }

}