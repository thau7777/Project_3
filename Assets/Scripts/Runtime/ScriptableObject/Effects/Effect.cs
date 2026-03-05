using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static EffectsManager;

[System.Serializable]
public struct StatModifier
{
    public float value;
    public bool isPercentage;
    public bool instantApply;
    public bool isTemporary;
}

// Base ScriptableObject for all effects
[CreateAssetMenu(fileName = "New Effect", menuName = "Scriptable Objects/Effect/Base Effect")]
public class Effect : ScriptableObject
{
    public Sprite icon;
    public EffectType effectType;
    public float holdDuration = 5;
    public float durationOnApply = 10;

    public bool isStackable;
    [ShowIf("isStackable")]
    [Range(2, 10)]
    public int stackRequired = 3;

    [TabGroup("Damage Settings")]
    public ElementalType damageElementalType;
    [TabGroup("Damage Settings")]
    public bool dealTrueDamage;

    [TabGroup("Stat Modifiers")]
    public StatModifier healthModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier manaModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier attackDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier magicAttackDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier physicalDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier magicDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier speedModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier criticalRateModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier criticalDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier attackSizeModifier;

    [TabGroup("Visual Feedback")]
    public Vector3 positionOffset;
    [TabGroup("Visual Feedback")]
    public FlyweightSettings vfxSettings;

    public virtual Flyweight OnApply(GameObject target, ActiveEffect activeEffect)
    {
        ApplyModifier(target, activeEffect, true);
        if (vfxSettings != null)
        {
            var vfx = FlyweightFactory.Spawn(vfxSettings);
            var effectController = vfx.GetComponent<CharacterEffectController>();
            bool needParent = effectController == null;
            Transform vfxParent = target.GetComponentInChildren<SkinnedMeshRenderer>().transform.GetChild(0);

            vfx.FlyweightInitialize(
                needParent ? vfxParent.transform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z) :
                Vector3.zero.Add(positionOffset.x, positionOffset.y, positionOffset.z),
                Quaternion.identity,
                needParent ? vfxParent : null);

            if(vfx is ContinousVFX)
            {
                (vfx as ContinousVFX).InitializeVFX((vfxSettings as ContinousVFXSettings).DefaultSize);
                vfx.gameObject.name = vfxSettings.prefab.name;
            }
            else if(vfx is OneShotVFX)
            {
                (vfx as OneShotVFX).InitializeVFX((vfxSettings as OneShotVFXSettings).DefaultSize, (vfxSettings as OneShotVFXSettings).DefaultLifeTime);
                vfx.gameObject.name = vfxSettings.prefab.name;
            }
            if (effectController != null)
                effectController.SetupCharacterEffect(target.GetComponentInChildren<SkinnedMeshRenderer>().transform);

            return vfx;
        }


        return null;
    }

    public virtual void OnUpdate(GameObject target, float deltaTime)
    {
        // Called each frame while effect is active
    }

    public virtual void OnRemove(ActiveEffect activeEffect, GameObject target = null)
    {
        activeEffect.activeVFX?.ReturnToPool();
        activeEffect.activeVFX?.transform.SetParent(null);
        if (target != null && activeEffect != null)
            RemoveModifier(target, activeEffect);
    }

    public void ApplyModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {        
        if (activeEffect.effect.name == "Berserker's Rage Effect" && isApplyInstant)
        {
            if (healthModifier.value != 0)
            target.GetComponent<Damageable>().OnHealthChanged?.Invoke(
                target.GetComponent<Damageable>().CurrentHealth *= -healthModifier.value / 100, 
                target.GetComponent<Damageable>().MaxHealth);

            if (attackDamageModifier.value != 0) ApplyAttackDamageModifier(target, activeEffect, isApplyInstant);
            if (magicAttackDamageModifier.value != 0) ApplyMagicAttackDamageModifier(target, activeEffect, isApplyInstant);
            return;
        }
        if (healthModifier.value != 0) ApplyHealthModifier(target, isApplyInstant);
        if (manaModifier.value != 0) ApplyManaModifier(target, isApplyInstant);
        if (attackDamageModifier.value != 0) ApplyAttackDamageModifier(target, activeEffect, isApplyInstant);
        if (magicAttackDamageModifier.value != 0) ApplyMagicAttackDamageModifier(target, activeEffect, isApplyInstant);
        if (physicalDefenseModifier.value != 0) ApplyPhysicalDefenseModifier(target, activeEffect, isApplyInstant);
        if (magicDefenseModifier.value != 0) ApplyMagicDefenseModifier(target, activeEffect, isApplyInstant);
        if (speedModifier.value != 0) ApplySpeedModifier(target, activeEffect, isApplyInstant);
        if (criticalRateModifier.value != 0) ApplyCriticalRateModifier(target, activeEffect, isApplyInstant);
        if (criticalDamageModifier.value != 0) ApplyCriticalDamageModifier(target, activeEffect, isApplyInstant);
        if (attackSizeModifier.value != 0) ApplyAttackSizeModifier(target, activeEffect, isApplyInstant);
    }
    private void RemoveModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (attackDamageModifier.value != 0 && attackDamageModifier.isTemporary)
            RemoveAttackDamageModifier(target, activeEffect);
        if (magicAttackDamageModifier.value != 0 && magicAttackDamageModifier.isTemporary)
            RemoveMagicAttackDamageModifier(target, activeEffect);
        if (physicalDefenseModifier.value != 0 && physicalDefenseModifier.isTemporary)
            RemovePhysicalDefenseModifier(target, activeEffect);
        if (magicDefenseModifier.value != 0 && magicDefenseModifier.isTemporary)
            RemoveMagicDefenseModifier(target, activeEffect);
        if (speedModifier.value != 0 && speedModifier.isTemporary)
            RemoveAgilityModifier(target, activeEffect);
        if (criticalRateModifier.value != 0 && criticalRateModifier.isTemporary)
            RemoveCriticalRateModifier(target, activeEffect);
        if (criticalDamageModifier.value != 0 && criticalDamageModifier.isTemporary)
            RemoveCriticalDamageModifier(target, activeEffect);
        if (attackSizeModifier.value != 0 && attackSizeModifier.isTemporary)
            RemoveAttackSizeModifier(target, activeEffect);
    }
    #region Apply Modifier Methods
    private void ApplyHealthModifier(GameObject target, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out Damageable damageAble) || healthModifier.instantApply && !isApplyInstant)
            return;

        float valueToApply = healthModifier.isPercentage ? damageAble.MaxHealth * (healthModifier.value / 100f) : healthModifier.value;
        if (valueToApply < 0)
        {
            float finalDamage = -valueToApply + PlayerTopDownStateDriver.Instance.GetComponent<CharacterStats>().MagicAttackDamage * 0.1f;
            finalDamage = ElementalManager.Instance.CalculateDamage(finalDamage, damageElementalType, target.GetComponent<CharacterStats>().ElementalType);
            damageAble.TakeDamage(null, null, finalDamage, dealTrueDamage, Vector3.zero, 0, damageElementalType, respectInvincibilityTime: false);
        }
        else
        {
            damageAble.Heal(valueToApply);

        }
    }
    private void ApplyManaModifier(GameObject target, bool isApplyInstant)
    {
        if(!target.TryGetComponent(out SkillExecutor mana) || manaModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = manaModifier.isPercentage ? mana.MaxMana * (manaModifier.value / 100f) : manaModifier.value;
        if (valueToApply < 0)
            mana.ConsumeMana(valueToApply);
        else
            mana.RestoreMana(valueToApply);
    }
    private void ApplyAttackDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || attackDamageModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = attackDamageModifier.isPercentage ? characterStats.AttackDamage * (attackDamageModifier.value / 100f) : attackDamageModifier.value;
        characterStats.ModifyAttackDamage(Mathf.RoundToInt(valueToApply));

        activeEffect.storedAttackDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyMagicAttackDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || magicAttackDamageModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = magicAttackDamageModifier.isPercentage ? characterStats.MagicAttackDamage * (magicAttackDamageModifier.value / 100f) : magicAttackDamageModifier.value;
        characterStats.ModifyMagicAttackDamage(Mathf.RoundToInt(valueToApply));

        activeEffect.storedMagicAttackDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyPhysicalDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || physicalDefenseModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = physicalDefenseModifier.isPercentage ? characterStats.PhysicalDefense * (physicalDefenseModifier.value / 100f) : physicalDefenseModifier.value;
        characterStats.ModifyPhysicalDefense(Mathf.RoundToInt(valueToApply));

        activeEffect.storedPhysicalDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyMagicDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || magicDefenseModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = magicDefenseModifier.isPercentage ? characterStats.MagicDefense * (magicDefenseModifier.value / 100f) : magicDefenseModifier.value;
        characterStats.ModifyMagicDefense(Mathf.RoundToInt(valueToApply));

        activeEffect.storedMagicDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplySpeedModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || speedModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = speedModifier.isPercentage ? characterStats.Speed * (speedModifier.value / 100f) : speedModifier.value;
        characterStats.ModifySpeed(valueToApply);

        activeEffect.storedSpeedChanges += valueToApply;
    }
    private void ApplyCriticalRateModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || criticalRateModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = criticalRateModifier.isPercentage ? characterStats.CriticalRate * (criticalRateModifier.value / 100f) : criticalRateModifier.value;
        characterStats.ModifyCriticalRate(valueToApply);

        activeEffect.storedCriticalRateChanges += valueToApply;
    }
    private void ApplyCriticalDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || criticalDamageModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = criticalDamageModifier.isPercentage ? characterStats.CriticalDamage * (criticalDamageModifier.value / 100f) : criticalDamageModifier.value;
        characterStats.ModifyCriticalDamage(valueToApply);

        activeEffect.storedCriticalDamageChanges += valueToApply;
    }
    private void ApplyAttackSizeModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || attackSizeModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = attackSizeModifier.isPercentage ? characterStats.AttackSizeScale * (attackSizeModifier.value / 100f) : attackSizeModifier.value;
        characterStats.ModifyAttackSizeScale(valueToApply);
        activeEffect.storedAttackSizeScaleChanges += valueToApply;
    }
    #endregion


    #region Remove Modifier Methods
    private void RemoveAttackDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyAttackDamage(-activeEffect.storedAttackDamageChanges);
    }
    private void RemoveMagicAttackDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyMagicAttackDamage(-activeEffect.storedMagicAttackDamageChanges);
    }
    private void RemovePhysicalDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyPhysicalDefense(-activeEffect.storedPhysicalDefenseChanges);
    }
    private void RemoveMagicDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyMagicDefense(-activeEffect.storedMagicDefenseChanges);
    }
    private void RemoveAgilityModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifySpeed(-activeEffect.storedSpeedChanges);
    }
    private void RemoveCriticalRateModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyCriticalRate(-activeEffect.storedCriticalRateChanges);
    }
    private void RemoveCriticalDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyCriticalDamage(-activeEffect.storedCriticalDamageChanges);
    }

    private void RemoveAttackSizeModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyAttackSizeScale(-activeEffect.storedAttackSizeScaleChanges);
    }
    #endregion
    public Flyweight GetVfxFlyweightOnTarget(GameObject target)
    {
        if (!vfxSettings) return null;
        Transform foundTransform = target.transform.Find(vfxSettings.prefab.name);
        if (foundTransform == null) return null;
        return foundTransform.GetComponent<Flyweight>();
    }

    public virtual bool HasSameModifiers(Effect other)
    {
        return Mathf.Approximately(healthModifier.value, other.healthModifier.value) &&
               Mathf.Approximately(speedModifier.value, other.speedModifier.value) &&
               Mathf.Approximately(attackDamageModifier.value, other.attackDamageModifier.value) &&
               Mathf.Approximately(physicalDefenseModifier.value, other.physicalDefenseModifier.value);
    }
}

public enum EffectType
{
    Buff,
    Debuff,
    Neutral
}

[System.Serializable]
public struct EffectData
{
    public Effect effect;
    public int stacksToApply;
}