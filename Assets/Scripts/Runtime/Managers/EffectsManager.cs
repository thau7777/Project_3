using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ActiveEffect
{
    public GameObject sender;
    public Effect effect;
    public float remainingTime;
    public float maxDuration;
    public int currentStacks;
    public Flyweight activeVFX;
    public bool IsApplied;
    public float tickTimer;
    public int storedAttackDamageChanges;
    public int storedMagicAttackDamageChanges;
    public int storedPhysicalDefenseChanges;
    public int storedMagicDefenseChanges;
    public float storedSpeedChanges;
    public float storedCriticalRateChanges;
    public float storedCriticalMultiplierChanges;
    public float storedAttackSizeScaleChanges;

    public float storedFireDamageChanges;
    public float storedWaterDamageChanges;
    public float storedFrostDamageChanges;
    public float storedLightningDamageChanges;
    public float storedHolyDamageChanges;
    public float storedDarkDamageChanges;
    public float storedPoisonDamageChanges;

    public int storedFireDefenseChanges;
    public int storedWaterDefenseChanges;
    public int storedFrostDefenseChanges;
    public int storedLightningDefenseChanges;
    public int storedHolyDefenseChanges;
    public int storedDarkDefenseChanges;
    public int storedPoisonDefenseChanges;

    public ActiveEffect(GameObject sender, Effect eff, float time, int stacks)
    {
        this.sender = sender;
        effect = eff;
        remainingTime = time;
        maxDuration = time;
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
    public UnityEvent<ActiveEffect> OnEffectRemoved;

    [SerializeField] private float _invincibleDuration = 0.1f;
    public float InvincibleDuration => _invincibleDuration;
    private float _invincibleElapsedTime = 0;

    private void OnEnable()
    {
        _invincibleElapsedTime = 0;
    }

    void Update()
    {
        ActiveEffectsHandler();
    }

    [Button]
    public void ApplyTestEffect()
    {
        AddEffect(gameObject, testEffectData);
    }
    private void ActiveEffectsHandler()
    {
        if (_invincibleElapsedTime > 0)
            _invincibleElapsedTime -= Time.deltaTime;

        for (int i = activeEffectsList.Count - 1; i >= 0; i--)
        {
            ActiveEffect ae = activeEffectsList[i];

            ae.effect.OnUpdate(gameObject, Time.deltaTime);

            if (ae.remainingTime > 0)
            {
                ae.remainingTime -= Time.deltaTime;
                if (ae.IsApplied)
                    ae.tickTimer -= Time.deltaTime;

                if (ae.tickTimer <= 0 && ae.IsApplied)
                {
                    ae.tickTimer = 1f;
                    ae.effect.ApplyModifier(ae.sender,gameObject, ae, false);
                }

                if (ae.remainingTime <= 0)
                {
                    RemoveEffect(ae);
                }
            }
        }
    }
    public void AddEffect(GameObject sender,EffectData effectData)
    {
        if (_invincibleElapsedTime > 0) return;

        ActiveEffect existing = activeEffectsList.Find(ae => ae.effect.name == effectData.effect.name);
        if (existing != null)
        {
            if (existing.IsApplied) return;

            existing.currentStacks += effectData.stacksToApply;
            if (existing.currentStacks >= existing.effect.stackRequired)
            {
                existing.activeVFX = existing.effect.OnApply(sender, gameObject, existing);
                existing.remainingTime = effectData.effect.durationOnApply;
                existing.maxDuration = effectData.effect.durationOnApply;
                existing.currentStacks = 0;
                existing.IsApplied = true;
            }
            else
            {
                existing.remainingTime = effectData.effect.holdDuration;
                existing.maxDuration = effectData.effect.holdDuration;
            }

            _invincibleElapsedTime = InvincibleDuration;
            OnEffectAdded?.Invoke(existing);
        }
        else
        {
            if (effectData.effect.name == "The Wish Under Star")
            {
                RemoveAllDebuff();
            }

            ActiveEffect newEffect = new ActiveEffect(sender, effectData.effect, effectData.effect.holdDuration, effectData.stacksToApply);
            activeEffectsList.Add(newEffect);

            if (!effectData.effect.isStackable || effectData.stacksToApply >= effectData.effect.stackRequired)
            {
                newEffect.activeVFX = effectData.effect.OnApply(sender, gameObject, newEffect);
                newEffect.remainingTime = effectData.effect.durationOnApply;
                newEffect.maxDuration = effectData.effect.durationOnApply;
                newEffect.currentStacks = 0;
                newEffect.IsApplied = true;
            }

            _invincibleElapsedTime = InvincibleDuration;
            OnEffectAdded?.Invoke(newEffect);
        }
    }

    public void RemoveEffect(Effect effect)
    {
        ActiveEffect ae = activeEffectsList.Find(e => e.effect.name == effect.name);
        if (ae != null) RemoveEffect(ae);
    }

    public void RemoveEffect(ActiveEffect ae)
    {
        ae.effect.OnRemove(ae, gameObject);
        activeEffectsList.Remove(ae);
        OnEffectRemoved?.Invoke(ae);
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

    public void RemoveAllDebuff()
    {
        for (int i = activeEffectsList.Count - 1; i >= 0; i--)
        {
            if (activeEffectsList[i].effect.effectType == EffectType.Debuff)
                RemoveEffect(activeEffectsList[i]);
        }
    }

    public bool HasEffect(string effectName)
    {
        foreach (ActiveEffect ae in activeEffectsList)
        {
            if (ae.effect.name == effectName && ae.IsApplied)
                return true;
        }
        return false;
    }

    public bool HasAnyActiveEffect()
    {
        foreach (ActiveEffect ae in activeEffectsList)
        {
            if (ae.IsApplied)
                return true;
        }
        return false;
    }

    public List<ActiveEffect> GetActiveEffectsList()
    {
        return activeEffectsList;
    }

    public float GetRemainingTime(string effectName)
    {
        ActiveEffect ae = activeEffectsList.Find(e => e.effect.name == effectName);
        return ae != null ? ae.remainingTime : 0f;
    }
}