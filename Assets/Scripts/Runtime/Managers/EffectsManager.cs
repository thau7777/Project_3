using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// ActiveEffect.cs - add maxDuration field
public class ActiveEffect
{
    public Effect effect;
    public float remainingTime;
    public float maxDuration;       // NEW - needed for fill calculation
    public int currentStacks;
    public Flyweight activeVFX;
    public bool IsApplied;
    public float tickTimer;
    public float storedAttackDamageChanges;
    public float storedMagicAttackDamageChanges;
    public float storedPhysicalDefenseChanges;
    public float storedMagicDefenseChanges;
    public float storedAgilityChanges;
    public float storedCriticalRateChanges;
    public float storedCriticalDamageChanges;
    public float storedAttackSizeScaleChanges;

    public ActiveEffect(Effect eff, float time, int stacks)
    {
        effect = eff;
        remainingTime = time;
        maxDuration = time;         // NEW
        currentStacks = stacks;
        IsApplied = false;
        tickTimer = 0f;
    }
}
public class EffectsManager : MonoBehaviour
{
    private List<ActiveEffect> activeEffectsList = new List<ActiveEffect>();

    public EffectData testEffectData;

    public UnityEvent<ActiveEffect> OnEffectAdded;
    public UnityEvent<ActiveEffect> OnEffectRemoved;   // NEW

    void Update()
    {
        for (int i = activeEffectsList.Count - 1; i >= 0; i--)
        {
            ActiveEffect ae = activeEffectsList[i];

            ae.effect.OnUpdate(gameObject, Time.deltaTime);

            if (ae.remainingTime > 0)
            {
                ae.remainingTime -= Time.deltaTime;
                if(ae.IsApplied)
                    ae.tickTimer -= Time.deltaTime;

                if (ae.tickTimer <= 0 && ae.IsApplied)
                {
                    ae.tickTimer = 1f;
                    ae.effect.ApplyModifier(gameObject, ae, false);
                }

                if (ae.remainingTime <= 0)
                {
                    RemoveEffect(ae);
                }
            }
        }
    }

    [Button]
    public void ApplyTestEffect()
    {
        AddEffect(testEffectData);
    }

    public void AddEffect(EffectData effectData)
    {
        ActiveEffect existing = activeEffectsList.Find(ae => ae.effect.name == effectData.effect.name);
        if (existing != null)
        {
            if (existing.IsApplied) return;

            existing.currentStacks += effectData.stacksToApply;
            if (existing.currentStacks >= existing.effect.stackRequired)
            {
                existing.activeVFX = existing.effect.OnApply(gameObject, existing);
                existing.remainingTime = effectData.effect.durationOnApply;
                existing.maxDuration = effectData.effect.durationOnApply;  // NEW
                existing.currentStacks = 0;
                existing.IsApplied = true;
            }
            else
            {
                existing.remainingTime = effectData.effectHoldDuration;
                existing.maxDuration = effectData.effectHoldDuration;      // NEW
            }

            OnEffectAdded?.Invoke(existing);
        }
        else
        {
            ActiveEffect newEffect = new ActiveEffect(effectData.effect, effectData.effectHoldDuration, effectData.stacksToApply);
            activeEffectsList.Add(newEffect);

            if (!effectData.effect.isStackable || effectData.stacksToApply >= effectData.effect.stackRequired)
            {
                newEffect.activeVFX = effectData.effect.OnApply(gameObject, newEffect);
                newEffect.remainingTime = effectData.effect.durationOnApply;
                newEffect.maxDuration = effectData.effect.durationOnApply;  // NEW
                newEffect.currentStacks = 0;
                newEffect.IsApplied = true;
            }

            OnEffectAdded?.Invoke(newEffect);
        }
    }

    public void RemoveEffect(Effect effect)
    {
        ActiveEffect ae = activeEffectsList.Find(e => e.effect.name == effect.name);
        if (ae != null) RemoveEffect(ae);
    }

    private void RemoveEffect(ActiveEffect ae)
    {
        ae.effect.OnRemove(ae, gameObject);
        activeEffectsList.Remove(ae);
        OnEffectRemoved?.Invoke(ae);    // NEW - notify UI
    }

    public void RemoveEffectByName(string effectName)
    {
        foreach (ActiveEffect ae in activeEffectsList)
        {
            if (ae.effect.name == effectName)
            {
                RemoveEffect(ae);
                break;
            }
        }
    }

    public void RemoveAllActiveEffects()
    {
        for (int i = activeEffectsList.Count - 1; i >= 0; i--)
            RemoveEffect(activeEffectsList[i]);
    }

    public bool HasEffect(string effectName) =>
        activeEffectsList.Exists(ae => ae.effect.name == effectName);

    public List<Effect> GetActiveEffects()
    {
        List<Effect> effects = new List<Effect>();
        foreach (var ae in activeEffectsList)
            effects.Add(ae.effect);
        return effects;
    }

    public float GetRemainingTime(string effectName)
    {
        ActiveEffect ae = activeEffectsList.Find(e => e.effect.name == effectName);
        return ae != null ? ae.remainingTime : 0f;
    }
}
