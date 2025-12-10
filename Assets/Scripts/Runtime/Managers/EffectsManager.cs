using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private List<ActiveEffect> activeEffectsList = new List<ActiveEffect>();

    private class ActiveEffect
    {
        public Effect effect;
        public float remainingTime;
        public GameObject particleInstance;

        public ActiveEffect(Effect eff, float time)
        {
            effect = eff;
            remainingTime = time;
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

    public void AddEffect(Effect effect)
    {
        // Find if an effect with the same name already exists
        ActiveEffect existing = activeEffectsList.Find(ae => ae.effect.name == effect.name);

        if (existing != null)
        {
            existing.remainingTime = effect.duration;
            return;
            // Check if modifiers are the same
            //if (existing.effect.HasSameModifiers(effect))
            //{
            //    // Same modifiers, just refresh duration
            //    existing.remainingTime = effect.duration;
            //    Debug.Log($"Refreshed {effect.effectName} duration on {gameObject.name}");
            //    return;
            //}
            //else
            //{
            //    // Different modifiers, remove old effect and apply new one
            //    Debug.Log($"Replacing {effect.effectName} with new modifiers on {gameObject.name}");
            //    RemoveEffect(existing);
            //}
        }

        // Add new effect
        ActiveEffect newEffect = new ActiveEffect(effect, effect.duration);
        activeEffectsList.Add(newEffect);

        effect.OnApply(gameObject);

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
