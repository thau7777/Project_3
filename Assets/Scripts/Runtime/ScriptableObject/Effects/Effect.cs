using Ami.BroAudio;
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
    public StatModifier criticalMultiplierModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier attackSizeModifier;

    [TabGroup("Stat Modifiers")]
    public StatModifier fireDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier waterDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier frostDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier lightningDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier holyDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier darkDamageModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier poisonDamageModifier;

    [TabGroup("Stat Modifiers")]
    public StatModifier fireDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier waterDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier frostDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier lightningDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier holyDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier darkDefenseModifier;
    [TabGroup("Stat Modifiers")]
    public StatModifier poisonDefenseModifier;
    [TabGroup("Visual Feedback")]
    public Vector3 positionOffset;
    [TabGroup("Visual Feedback")]
    public Vector3 rotationOffset = Vector3.one;
    [TabGroup("Visual Feedback")]
    public FlyweightSettings vfxSettings;

    [TabGroup("Sounds")]
    [SerializeField] private List<SoundID> _effectApplySounds;
    [TabGroup("Sounds")]
    [SerializeField] private List<SoundID> _effectLoopSounds;
    [TabGroup("Sounds")]
    [SerializeField] private List<SoundID> _effectRemoveSounds;

    public virtual Flyweight OnApply(GameObject sender, GameObject target, ActiveEffect activeEffect)
    {
        ApplyModifier(sender, target, activeEffect, true);
        TurnOnSound(_effectApplySounds);
        TurnOnSound(_effectLoopSounds);

        if (vfxSettings != null)
        {
            var vfx = FlyweightFactory.Spawn(vfxSettings);
            var effectController = vfx.GetComponent<CharacterEffectController>();
            bool needParent = effectController == null;
            Transform vfxParent = target.GetComponentInChildren<SkinnedMeshRenderer>().transform.GetChild(0);
            var quaternionRotationOffset = Quaternion.Euler(rotationOffset);
            vfx.FlyweightInitialize(
                needParent ? vfxParent.transform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z) :
                Vector3.zero.Add(positionOffset.x, positionOffset.y, positionOffset.z),
                needParent ? vfxParent.transform.rotation * quaternionRotationOffset : Quaternion.identity,
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

        TurnOffSound(_effectLoopSounds);
        TurnOnSound(_effectRemoveSounds);
    }

    private void TurnOnSound(List<SoundID> soundIDs)
    {
        if (soundIDs == null || soundIDs.Count == 0) return;
        foreach (var soundID in soundIDs)
        {
            if(soundID.ToString() != "None")
                BroAudio.Play(soundID);
        }
    }

    private void TurnOffSound(List<SoundID> soundIDs)
    {
        if (soundIDs == null || soundIDs.Count == 0) return;
        foreach (var soundID in soundIDs)
        {
            if(soundID.ToString() != "None")
                BroAudio.Stop(soundID);
        }
    }

    public void ApplyModifier(GameObject sender, GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
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
        if (healthModifier.value != 0) ApplyHealthModifier(sender, target, isApplyInstant);
        if (manaModifier.value != 0) ApplyManaModifier(target, isApplyInstant);
        if (attackDamageModifier.value != 0) ApplyAttackDamageModifier(target, activeEffect, isApplyInstant);
        if (magicAttackDamageModifier.value != 0) ApplyMagicAttackDamageModifier(target, activeEffect, isApplyInstant);
        if (physicalDefenseModifier.value != 0) ApplyPhysicalDefenseModifier(target, activeEffect, isApplyInstant);
        if (magicDefenseModifier.value != 0) ApplyMagicDefenseModifier(target, activeEffect, isApplyInstant);
        if (speedModifier.value != 0) ApplySpeedModifier(target, activeEffect, isApplyInstant);
        if (criticalRateModifier.value != 0) ApplyCriticalRateModifier(target, activeEffect, isApplyInstant);
        if (criticalMultiplierModifier.value != 0) ApplyCriticalMultiplierModifier(target, activeEffect, isApplyInstant);
        if (attackSizeModifier.value != 0) ApplyAttackSizeModifier(target, activeEffect, isApplyInstant);

        if (fireDamageModifier.value != 0) ApplyFireDamageModifier(target, activeEffect, isApplyInstant);
        if (waterDamageModifier.value != 0) ApplyWaterDamageModifier(target, activeEffect, isApplyInstant);
        if (frostDamageModifier.value != 0) ApplyFrostDamageModifier(target, activeEffect, isApplyInstant);
        if (lightningDamageModifier.value != 0) ApplyLightningDamageModifier(target, activeEffect, isApplyInstant);
        if (holyDamageModifier.value != 0) ApplyHolyDamageModifier(target, activeEffect, isApplyInstant);
        if (darkDamageModifier.value != 0) ApplyDarkDamageModifier(target, activeEffect, isApplyInstant);
        if (poisonDamageModifier.value != 0) ApplyPoisonDamageModifier(target, activeEffect, isApplyInstant);

        if (fireDefenseModifier.value != 0) ApplyFireDefenseModifier(target, activeEffect, isApplyInstant);
        if (waterDefenseModifier.value != 0) ApplyWaterDefenseModifier(target, activeEffect, isApplyInstant);
        if (frostDefenseModifier.value != 0) ApplyFrostDefenseModifier(target, activeEffect, isApplyInstant);
        if (lightningDefenseModifier.value != 0) ApplyLightningDefenseModifier(target, activeEffect, isApplyInstant);
        if (holyDefenseModifier.value != 0) ApplyHolyDefenseModifier(target, activeEffect, isApplyInstant);
        if (darkDefenseModifier.value != 0) ApplyDarkDefenseModifier(target, activeEffect, isApplyInstant);
        if (poisonDefenseModifier.value != 0) ApplyPoisonDefenseModifier(target, activeEffect, isApplyInstant);
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
        if (criticalMultiplierModifier.value != 0 && criticalMultiplierModifier.isTemporary)
            RemoveCriticalDamageModifier(target, activeEffect);
        if (attackSizeModifier.value != 0 && attackSizeModifier.isTemporary)
            RemoveAttackSizeModifier(target, activeEffect);

        if (fireDamageModifier.value != 0 && fireDamageModifier.isTemporary) RemoveFireDamageModifier(target, activeEffect);
        if (waterDamageModifier.value != 0 && waterDamageModifier.isTemporary) RemoveWaterDamageModifier(target, activeEffect);
        if (frostDamageModifier.value != 0 && frostDamageModifier.isTemporary) RemoveFrostDamageModifier(target, activeEffect);
        if (lightningDamageModifier.value != 0 && lightningDamageModifier.isTemporary) RemoveLightningDamageModifier(target, activeEffect);
        if (holyDamageModifier.value != 0 && holyDamageModifier.isTemporary) RemoveHolyDamageModifier(target, activeEffect);
        if (darkDamageModifier.value != 0 && darkDamageModifier.isTemporary) RemoveDarkDamageModifier(target, activeEffect);
        if (poisonDamageModifier.value != 0 && poisonDamageModifier.isTemporary) RemovePoisonDamageModifier(target, activeEffect);

        if (fireDefenseModifier.value != 0 && fireDefenseModifier.isTemporary) RemoveFireDefenseModifier(target, activeEffect);
        if (waterDefenseModifier.value != 0 && waterDefenseModifier.isTemporary) RemoveWaterDefenseModifier(target, activeEffect);
        if (frostDefenseModifier.value != 0 && frostDefenseModifier.isTemporary) RemoveFrostDefenseModifier(target, activeEffect);
        if (lightningDefenseModifier.value != 0 && lightningDefenseModifier.isTemporary) RemoveLightningDefenseModifier(target, activeEffect);
        if (holyDefenseModifier.value != 0 && holyDefenseModifier.isTemporary) RemoveHolyDefenseModifier(target, activeEffect);
        if (darkDefenseModifier.value != 0 && darkDefenseModifier.isTemporary) RemoveDarkDefenseModifier(target, activeEffect);
        if (poisonDefenseModifier.value != 0 && poisonDefenseModifier.isTemporary) RemovePoisonDefenseModifier(target, activeEffect);
    }
    #region Apply Modifier Methods
    private void ApplyHealthModifier(GameObject sender, GameObject target, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out Damageable damageAble) || healthModifier.instantApply && !isApplyInstant)
            return;

        float valueToApply = healthModifier.isPercentage ? damageAble.MaxHealth * (healthModifier.value / 100f) : healthModifier.value;
        if (valueToApply < 0)
        {
            float finalDamage = -valueToApply + PlayerTopDownStateDriver.Instance.GetComponent<CharacterStats>().MagicAttackDamage * 0.1f;
            finalDamage = ElementalManager.Instance.CalculateDamage(Mathf.RoundToInt(finalDamage), damageElementalType, target.GetComponent<CharacterStats>().ElementalType);
            damageAble.TakeDamage(sender, null, true, Mathf.RoundToInt(finalDamage), dealTrueDamage, Vector3.zero, 0, damageElementalType, respectInvincibilityTime: false);
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
    private void ApplyCriticalMultiplierModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || criticalMultiplierModifier.instantApply && !isApplyInstant)
            return;
        float valueToApply = criticalMultiplierModifier.isPercentage ? characterStats.CriticalMultiplier * (criticalMultiplierModifier.value / 100f) : criticalMultiplierModifier.value;
        characterStats.ModifyCriticalDamage(valueToApply);

        activeEffect.storedCriticalMultiplierChanges += valueToApply;
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
        characterStats.ModifyAttackDamage(Mathf.RoundToInt(-activeEffect.storedAttackDamageChanges));
    }
    private void RemoveMagicAttackDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyMagicAttackDamage(Mathf.RoundToInt(-activeEffect.storedMagicAttackDamageChanges));
    }
    private void RemovePhysicalDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyPhysicalDefense(Mathf.RoundToInt(-activeEffect.storedPhysicalDefenseChanges));
    }
    private void RemoveMagicDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyMagicDefense(Mathf.RoundToInt(-activeEffect.storedMagicDefenseChanges));
    }
    private void RemoveAgilityModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifySpeed(Mathf.RoundToInt(-activeEffect.storedSpeedChanges));
    }
    private void RemoveCriticalRateModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyCriticalRate(Mathf.RoundToInt(-activeEffect.storedCriticalRateChanges));
    }
    private void RemoveCriticalDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyCriticalDamage(Mathf.RoundToInt(-activeEffect.storedCriticalMultiplierChanges));
    }

    private void RemoveAttackSizeModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyAttackSizeScale(Mathf.RoundToInt(-activeEffect.storedAttackSizeScaleChanges));
    }
    #endregion

    #region Elemental Damage Apply Methods
    private void ApplyFireDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || fireDamageModifier.instantApply && !isApplyInstant) return;
        float valueToApply = fireDamageModifier.isPercentage ? characterStats.FireDamage * (fireDamageModifier.value / 100f) : fireDamageModifier.value;
        characterStats.ModifyFireDamage(Mathf.RoundToInt(valueToApply));
        activeEffect.storedFireDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyWaterDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || waterDamageModifier.instantApply && !isApplyInstant) return;
        float valueToApply = waterDamageModifier.isPercentage ? characterStats.WaterDamage * (waterDamageModifier.value / 100f) : waterDamageModifier.value;
        characterStats.ModifyWaterDamage(Mathf.RoundToInt(valueToApply));
        activeEffect.storedWaterDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyFrostDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || frostDamageModifier.instantApply && !isApplyInstant) return;
        float valueToApply = frostDamageModifier.isPercentage ? characterStats.FrostDamage * (frostDamageModifier.value / 100f) : frostDamageModifier.value;
        characterStats.ModifyFrostDamage(Mathf.RoundToInt(valueToApply));
        activeEffect.storedFrostDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyLightningDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || lightningDamageModifier.instantApply && !isApplyInstant) return;
        float valueToApply = lightningDamageModifier.isPercentage ? characterStats.LightningDamage * (lightningDamageModifier.value / 100f) : lightningDamageModifier.value;
        characterStats.ModifyLightningDamage(Mathf.RoundToInt(valueToApply));
        activeEffect.storedLightningDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyHolyDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || holyDamageModifier.instantApply && !isApplyInstant) return;
        float valueToApply = holyDamageModifier.isPercentage ? characterStats.HolyDamage * (holyDamageModifier.value / 100f) : holyDamageModifier.value;
        characterStats.ModifyHolyDamage(Mathf.RoundToInt(valueToApply));
        activeEffect.storedHolyDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyDarkDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || darkDamageModifier.instantApply && !isApplyInstant) return;
        float valueToApply = darkDamageModifier.isPercentage ? characterStats.DarkDamage * (darkDamageModifier.value / 100f) : darkDamageModifier.value;
        characterStats.ModifyDarkDamage(Mathf.RoundToInt(valueToApply));
        activeEffect.storedDarkDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyPoisonDamageModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || poisonDamageModifier.instantApply && !isApplyInstant) return;
        float valueToApply = poisonDamageModifier.isPercentage ? characterStats.PoisonDamage * (poisonDamageModifier.value / 100f) : poisonDamageModifier.value;
        characterStats.ModifyPoisonDamage(Mathf.RoundToInt(valueToApply));
        activeEffect.storedPoisonDamageChanges += Mathf.RoundToInt(valueToApply);
    }
    #endregion

    #region Elemental Defense Apply Methods
    private void ApplyFireDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || fireDefenseModifier.instantApply && !isApplyInstant) return;
        float valueToApply = fireDefenseModifier.isPercentage ? characterStats.FireDefense * (fireDefenseModifier.value / 100f) : fireDefenseModifier.value;
        characterStats.ModifyFireDefense(Mathf.RoundToInt(valueToApply));
        activeEffect.storedFireDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyWaterDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || waterDefenseModifier.instantApply && !isApplyInstant) return;
        float valueToApply = waterDefenseModifier.isPercentage ? characterStats.WaterDefense * (waterDefenseModifier.value / 100f) : waterDefenseModifier.value;
        characterStats.ModifyWaterDefense(Mathf.RoundToInt(valueToApply));
        activeEffect.storedWaterDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyFrostDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || frostDefenseModifier.instantApply && !isApplyInstant) return;
        float valueToApply = frostDefenseModifier.isPercentage ? characterStats.FrostDefense * (frostDefenseModifier.value / 100f) : frostDefenseModifier.value;
        characterStats.ModifyFrostDefense(Mathf.RoundToInt(valueToApply));
        activeEffect.storedFrostDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyLightningDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || lightningDefenseModifier.instantApply && !isApplyInstant) return;
        float valueToApply = lightningDefenseModifier.isPercentage ? characterStats.LightningDefense * (lightningDefenseModifier.value / 100f) : lightningDefenseModifier.value;
        characterStats.ModifyLightningDefense(Mathf.RoundToInt(valueToApply));
        activeEffect.storedLightningDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyHolyDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || holyDefenseModifier.instantApply && !isApplyInstant) return;
        float valueToApply = holyDefenseModifier.isPercentage ? characterStats.HolyDefense * (holyDefenseModifier.value / 100f) : holyDefenseModifier.value;
        characterStats.ModifyHolyDefense(Mathf.RoundToInt(valueToApply));
        activeEffect.storedHolyDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyDarkDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || darkDefenseModifier.instantApply && !isApplyInstant) return;
        float valueToApply = darkDefenseModifier.isPercentage ? characterStats.DarkDefense * (darkDefenseModifier.value / 100f) : darkDefenseModifier.value;
        characterStats.ModifyDarkDefense(Mathf.RoundToInt(valueToApply));
        activeEffect.storedDarkDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    private void ApplyPoisonDefenseModifier(GameObject target, ActiveEffect activeEffect, bool isApplyInstant)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats) || poisonDefenseModifier.instantApply && !isApplyInstant) return;
        float valueToApply = poisonDefenseModifier.isPercentage ? characterStats.PoisonDefense * (poisonDefenseModifier.value / 100f) : poisonDefenseModifier.value;
        characterStats.ModifyPoisonDefense(Mathf.RoundToInt(valueToApply));
        activeEffect.storedPoisonDefenseChanges += Mathf.RoundToInt(valueToApply);
    }
    #endregion

    #region Elemental Damage Remove Methods
    private void RemoveFireDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyFireDamage(Mathf.RoundToInt(-activeEffect.storedFireDamageChanges));
    }
    private void RemoveWaterDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyWaterDamage(Mathf.RoundToInt(-activeEffect.storedWaterDamageChanges));
    }
    private void RemoveFrostDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyFrostDamage(Mathf.RoundToInt(-activeEffect.storedFrostDamageChanges));
    }
    private void RemoveLightningDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyLightningDamage(Mathf.RoundToInt(-activeEffect.storedLightningDamageChanges));
    }
    private void RemoveHolyDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyHolyDamage(Mathf.RoundToInt(-activeEffect.storedHolyDamageChanges));
    }
    private void RemoveDarkDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyDarkDamage(Mathf.RoundToInt(-activeEffect.storedDarkDamageChanges));
    }
    private void RemovePoisonDamageModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyPoisonDamage(Mathf.RoundToInt(-activeEffect.storedPoisonDamageChanges));
    }
    #endregion

    #region Elemental Defense Remove Methods
    private void RemoveFireDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyFireDefense(Mathf.RoundToInt(-activeEffect.storedFireDefenseChanges));
    }
    private void RemoveWaterDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyWaterDefense(Mathf.RoundToInt(-activeEffect.storedWaterDefenseChanges));
    }
    private void RemoveFrostDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyFrostDefense(Mathf.RoundToInt(-activeEffect.storedFrostDefenseChanges));
    }
    private void RemoveLightningDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyLightningDefense(Mathf.RoundToInt(-activeEffect.storedLightningDefenseChanges));
    }
    private void RemoveHolyDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyHolyDefense(Mathf.RoundToInt(-activeEffect.storedHolyDefenseChanges));
    }
    private void RemoveDarkDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyDarkDefense(Mathf.RoundToInt(-activeEffect.storedDarkDefenseChanges));
    }
    private void RemovePoisonDefenseModifier(GameObject target, ActiveEffect activeEffect)
    {
        if (!target.TryGetComponent(out CharacterStats characterStats)) return;
        characterStats.ModifyPoisonDefense(Mathf.RoundToInt(-activeEffect.storedPoisonDefenseChanges));
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

    [Button]
    public void ResetStatModifier()
    {
        healthModifier.value = 0;
        manaModifier.value = 0;
        attackDamageModifier.value = 0;
        magicAttackDamageModifier.value = 0;
        physicalDefenseModifier.value = 0;
        magicDefenseModifier.value = 0;
        speedModifier.value = 0;
        criticalRateModifier.value = 0;
        criticalMultiplierModifier.value = 0;
        attackSizeModifier.value = 0;

        fireDamageModifier.value = 0;
        waterDamageModifier.value = 0;
        frostDamageModifier.value = 0;
        lightningDamageModifier.value = 0;
        holyDamageModifier.value = 0;
        darkDamageModifier.value = 0;
        poisonDamageModifier.value = 0;

        fireDefenseModifier.value = 0;
        waterDefenseModifier.value = 0;
        frostDefenseModifier.value = 0;
        lightningDefenseModifier.value = 0;
        holyDefenseModifier.value = 0;
        darkDefenseModifier.value = 0;
        poisonDefenseModifier.value = 0;
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