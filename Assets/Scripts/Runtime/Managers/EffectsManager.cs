using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private List<ActiveEffect> activeEffectsList = new List<ActiveEffect>();

    private class ActiveEffect
    {
        public Effect effect;
        public float remainingTime;
        public int currentStacks;
        public GameObject particleInstance;

        public ActiveEffect(Effect eff, float time, int stacks)
        {
            effect = eff;
            remainingTime = time;
            currentStacks = stacks;
        }
    }

    void Update()
    {
        for (int i = activeEffectsList.Count - 1; i >= 0; i--)
        {
            ActiveEffect ae = activeEffectsList[i];

            ae.effect.OnUpdate(gameObject, Time.deltaTime);

            if (!ae.effect.isPermanent)
            {
                ae.remainingTime -= Time.deltaTime;

                if (ae.remainingTime <= 0)
                {
                    RemoveEffect(ae);
                }
            }
        }
    }

    public void AddEffect(EffectData effectData)
    {
        // Find if an effect with the same name already exists
        ActiveEffect existing = activeEffectsList.Find(ae => ae.effect.name == effectData.effect.name);

        if (existing != null)
        {
            if(existing.effect.isStackable)
            {
                existing.currentStacks++;
                if(existing.currentStacks >= existing.effect.stackRequired)
                {
                    // Trigger the effect
                    existing.effect.OnApply(gameObject);
                    existing.currentStacks = 0; // Reset stacks after applying
                }
             
                return;
            }
            existing.remainingTime = effectData.effect.duration;
        }

        // Add new effect
        ActiveEffect newEffect = new ActiveEffect(effectData.effect, effectData.effect.duration,1);
        activeEffectsList.Add(newEffect);

        if(!effectData.effect.isStackable)
            effectData.effect.OnApply(gameObject);

    }

    public void RemoveEffect(Effect effect)
    {
        ActiveEffect ae = activeEffectsList.Find(e => e.effect.name == effect.name);
        if (ae != null)
        {
            RemoveEffect(ae);
        }
    }

    private void RemoveEffect(ActiveEffect ae)
    {
        ae.effect.OnRemove(gameObject);
        activeEffectsList.Remove(ae);
    }
    public void RemoveEffectByName(string effectName)
    {
        foreach(ActiveEffect ae in activeEffectsList)
        {
            if(ae.effect.name == effectName)
            {
                RemoveEffect(ae);
                break;
            }
        }
    }
    public void RemoveAllActiveEffects()
    {
        for (int i = activeEffectsList.Count - 1; i >= 0; i--)
        {
            RemoveEffect(activeEffectsList[i]);
        }
    }
    public bool HasEffect(string effectName)
    {
        return activeEffectsList.Exists(ae => ae.effect.name == effectName);
    }

    
    public List<Effect> GetActiveEffects()
    {
        List<Effect> effects = new List<Effect>();
        foreach (var ae in activeEffectsList)
        {
            effects.Add(ae.effect);
        }
        return effects;
    }

    public float GetRemainingTime(string effectName)
    {
        ActiveEffect ae = activeEffectsList.Find(e => e.effect.name == effectName);
        return ae != null ? ae.remainingTime : 0f;
    }
}
