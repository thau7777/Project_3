using UnityEngine;
using System.Collections.Generic;
using static Skill;

namespace Turnbase
{
    public class CharacterBuffManager : MonoBehaviour
    {
        private CharacterStats stats;
        private Character character;


        [Header("Attack Buff")]
        [HideInInspector] public int attackBuffTurnsRemaining = 0;
        [HideInInspector] public int originalBaseAttack = 0;
        [HideInInspector] public Flyweight_TB attackVFXInstance;
        [HideInInspector] public Sprite attackBuffIcon;

        [Header("MAX HP Buff")]
        [HideInInspector] public int originalBaseMaxHP = 0;
        [HideInInspector] public int maxHPBuffTurnsRemaining = 0;
        [HideInInspector] public Flyweight_TB maxHPVFXInstance;
        [HideInInspector] public Sprite maxHPBuffIcon;

        [Header("Defense Buff")]
        [HideInInspector] public int originalBaseDefense = 0;
        [HideInInspector] public int defenseBuffTurnsRemaining = 0;
        [HideInInspector] public int defenseBuffAmount = 0;
        [HideInInspector] public Flyweight_TB defenseVFXInstance;
        [HideInInspector] public Sprite defenseBuffIcon;

        [Header("Agility Buff")]
        [HideInInspector] public int originalBaseAgility = 0;
        [HideInInspector] public int agilityBuffTurnsRemaining = 0;
        [HideInInspector] public int agilityBuffAmount = 0;
        [HideInInspector] public Flyweight_TB agilityVFXInstance;
        [HideInInspector] public Sprite agilityBuffIcon;

        [Header("Magical Attack Buff")]
        [HideInInspector] public int magicalAttackBuffTurnsRemaining = 0;
        [HideInInspector] public int magicalOriginalBaseAttack = 0;
        [HideInInspector] public Flyweight_TB magicalAttackVFXInstance;
        [HideInInspector] public Sprite magicalAttackBuffIcon;

        [Header("Magical Defense Buff")]
        [HideInInspector] public int magicalOriginalBaseDefense = 0;
        [HideInInspector] public int magicalDefenseBuffTurnsRemaining = 0;
        [HideInInspector] public int magicalDefenseBuffAmount = 0;
        [HideInInspector] public Flyweight_TB magicalDefenseVFXInstance;
        [HideInInspector] public Sprite magicalDefenseBuffIcon;


        [Header("Shield")]
        [HideInInspector] public int baseShieldAmount = 0;
        [HideInInspector] public int shieldTurnsRemaining = 0;
        [HideInInspector] public Flyweight_TB shieldVFXInstance;
        [HideInInspector] public Sprite shieldIcon;

        [Header("Basic Attack Buff")]
        [HideInInspector] public int basicAttackBuffAmount = 0;
        [HideInInspector] public int basicAttackBuffTurnsRemaining = 0;
        [HideInInspector] public Sprite basicAttackBuffIcon;

        [Header("Stack Manager")]
        public Dictionary<string, StackData> activeStacks = new Dictionary<string, StackData>();

        void Awake()
        {
            character = GetComponent<Character>();
            if (character != null)
            {
                stats = character.stats;
                InitializeBaseStats();
            }
        }

        public void InitializeBaseStats()
        {
            if (originalBaseDefense == 0 && stats.physicalDefense > 0)
            {
                originalBaseDefense = stats.physicalDefense;
            }
            if (magicalOriginalBaseDefense == 0 && stats.magicDefense > 0)
            {
                magicalOriginalBaseDefense = stats.magicDefense;
            }
            if (originalBaseAgility == 0 && stats.agility > 0)
            {
                originalBaseAgility = stats.agility;
            }
        }

        public void AddShield(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (amount <= 0) return;

            if (shieldTurnsRemaining <= 0)
            {
                baseShieldAmount = amount;
            }
            else
            {
                baseShieldAmount += amount;
            }

            if (vfxInstance != null)
            {
                if (shieldVFXInstance != null && shieldVFXInstance != vfxInstance)
                {
                    Destroy(shieldVFXInstance);
                }
                shieldVFXInstance = vfxInstance;
            }

            shieldTurnsRemaining = duration;
            shieldIcon = icon;

            stats.currentShield = Mathf.Min(stats.currentShield + amount, stats.maxShield);

            Debug.Log(character.gameObject.name + " đã nhận thêm " + amount + " Shield. Shield hiện tại: " + stats.currentShield);

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));
        }


        public void ApplyAttackBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (attackBuffTurnsRemaining <= 0)
            {
                originalBaseAttack = stats.physicalAttack;
                stats.physicalAttack += amount;
            }
            else
            {
                Debug.Log("Buff Attack đã được làm mới thời gian.");
            }

            if (attackVFXInstance != null && attackVFXInstance != vfxInstance)
            {
                Destroy(attackVFXInstance);
            }
            attackVFXInstance = vfxInstance;
            attackBuffIcon = icon;

            attackBuffTurnsRemaining = duration;

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));

            Debug.Log($"{character.name} đã nhận buff +{amount} Attack, hiệu lực {duration} lượt. Attack hiện tại: {stats.physicalAttack}");
        }

        public void ApplyMaxHPBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (maxHPBuffTurnsRemaining <= 0)
            {
                originalBaseMaxHP = stats.maxHP;
                stats.maxHP += amount;
                stats.currentHP += amount;
            }
            else
            {
                Debug.Log($"Buff MaxHP của {character.name} đã được làm mới thời gian.");
            }

            if (maxHPVFXInstance != null && maxHPVFXInstance != vfxInstance)
            {
                Destroy(maxHPVFXInstance);
            }
            maxHPVFXInstance = vfxInstance;
            maxHPBuffIcon = icon;

            maxHPBuffTurnsRemaining = duration;

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));


            Debug.Log($"{character.name} đã nhận buff +{amount} MaxHP, hiệu lực {duration} lượt. MaxHP hiện tại: {stats.maxHP}");
        }

        public void ApplyDefenseBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (defenseBuffTurnsRemaining <= 0)
            {
                originalBaseDefense = stats.physicalDefense;
                defenseBuffAmount = amount;
                stats.physicalDefense += amount;
            }
            else
            {
                Debug.Log($"Buff Defense của {character.name} đã được làm mới thời gian.");
            }

            if (defenseVFXInstance != null && defenseVFXInstance != vfxInstance)
            {
                Destroy(defenseVFXInstance);
            }
            defenseVFXInstance = vfxInstance;
            defenseBuffIcon = icon;

            defenseBuffTurnsRemaining = duration;

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));


            Debug.Log($"{character.name} đã nhận buff +{amount} Defense, hiệu lực {duration} lượt. Defense hiện tại: {stats.physicalDefense}");
        }

        public void ApplyAgilityBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (originalBaseAgility <= 0)
            {
                originalBaseAgility = stats.agility;
            }

            agilityBuffAmount = amount;
            agilityBuffTurnsRemaining = duration;

            if (agilityVFXInstance != null && agilityVFXInstance != vfxInstance)
            {
                Destroy(agilityVFXInstance);
            }
            agilityVFXInstance = vfxInstance;
            agilityBuffIcon = icon;

            RecalculateSpeedStat();

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));

            Debug.Log($"{character.name} đã nhận buff +{amount} Agility. Agility thực tế sau tính toán: {stats.agility}");
        }

        public void ApplyMagicalAttackBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (magicalAttackBuffTurnsRemaining <= 0)
            {
                magicalOriginalBaseAttack = stats.magicAttack;
                stats.magicAttack += amount;
            }
            else
            {
                Debug.Log($"Buff Magical Attack của {character.name} đã được làm mới thời gian.");
            }

            if (magicalAttackVFXInstance != null && magicalAttackVFXInstance != vfxInstance)
            {
                Destroy(magicalAttackVFXInstance);
            }
            magicalAttackVFXInstance = vfxInstance;
            magicalAttackBuffIcon = icon;

            magicalAttackBuffTurnsRemaining = duration;

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));


            Debug.Log($"{character.name} đã nhận buff +{amount} Magical Attack, hiệu lực {duration} lượt. Magical Attack hiện tại: {stats.magicAttack}");
        }

        public void ApplyMagicalDefenseBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (magicalDefenseBuffTurnsRemaining <= 0)
            {
                magicalOriginalBaseDefense = stats.magicDefense;
                magicalDefenseBuffAmount = amount;
                stats.magicDefense += amount;
            }
            else
            {
                Debug.Log($"Buff Magical Defense của {character.name} đã được làm mới thời gian.");
            }

            if (magicalDefenseVFXInstance != null && magicalDefenseVFXInstance != vfxInstance)
            {
                Destroy(magicalDefenseVFXInstance);
            }
            magicalDefenseVFXInstance = vfxInstance;
            magicalDefenseBuffIcon = icon;

            magicalDefenseBuffTurnsRemaining = duration;

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));

            Debug.Log($"{character.name} đã nhận buff +{amount} Magical Defense, hiệu lực {duration} lượt. Magical Defense hiện tại: {stats.magicDefense}");
        }

        public void ApplyBasicAttackBuff(int amount, int duration, Sprite icon)
        {
            basicAttackBuffAmount = amount;
            basicAttackBuffTurnsRemaining = duration;
            basicAttackBuffIcon = icon;

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));
        }


        public void ApplyBuff(Skill.BuffSettings buffSettings, Flyweight_TB buffVFX, int amount)
        {
            if (buffSettings.durationTurns <= 0) return;

            switch (buffSettings.statToModify)
            {
                case StatType.Attack:
                    ApplyAttackBuff(amount, buffSettings.durationTurns, buffVFX, buffSettings.icon);
                    break;
                case StatType.Defense:
                    ApplyDefenseBuff(amount, buffSettings.durationTurns, buffVFX, buffSettings.icon);
                    break;
                case StatType.Agility:
                    ApplyAgilityBuff(amount, buffSettings.durationTurns, buffVFX, buffSettings.icon);
                    break;
                case StatType.MaxHP:
                    ApplyMaxHPBuff(amount, buffSettings.durationTurns, buffVFX, buffSettings.icon);
                    break;
                case StatType.MagicalAttack:
                    ApplyMagicalAttackBuff(amount, buffSettings.durationTurns, buffVFX, buffSettings.icon);
                    break;
                case StatType.MagicalDefense:
                    ApplyMagicalDefenseBuff(amount, buffSettings.durationTurns, buffVFX, buffSettings.icon);
                    break;
                case StatType.BasicAttackDamage:
                    ApplyBasicAttackBuff(amount, buffSettings.durationTurns, buffSettings.icon);
                    break;
                default:
                    Debug.LogWarning($"Loại Buff {buffSettings.statToModify} không được hỗ trợ hoặc không có giá trị.");
                    break;
            }
        }

        public void ProcessSkillStacks(Skill skill, Character targetCharacter)
        {
            var stackSetting = skill.stackSetting;
            var applicationTarget = skill.stackApplicationTarget;

            if (applicationTarget == StackApplicationTarget.None || stackSetting.stackId == string.Empty)
                return;

            Character stackTarget = (applicationTarget == StackApplicationTarget.Self) ? character : targetCharacter;

            if (stackTarget == null || !stackTarget.isAlive || stackTarget.buffManager == null) return;

            CharacterBuffManager targetBuffManager = stackTarget.buffManager;
            string stackId = stackSetting.stackId;

            if (stackSetting.isStackBuilder)
            {
                if (!targetBuffManager.activeStacks.TryGetValue(stackId, out StackData currentStackData))
                {
                    currentStackData = new StackData
                    {
                        stackId = stackId,
                        currentStacks = 0,
                        icon = stackSetting.iconStack 
                    };
                    targetBuffManager.activeStacks.Add(stackId, currentStackData);
                }

                currentStackData.currentStacks += stackSetting.stackAmountPerUse;
                Debug.Log($"[Stack Builder] {stackTarget.info.name} tích lũy Stack '{stackId}': +{stackSetting.stackAmountPerUse}. Tổng: {currentStackData.currentStacks}");
            }

            if (!targetBuffManager.activeStacks.TryGetValue(stackId, out StackData currentStackDataForFinisher))
            {
                return;
            }


            if (stackSetting.isStackFinisher)
            {
                if (currentStackDataForFinisher.currentStacks >= stackSetting.stackThreshold)
                {
                    Debug.Log($"🎉 [Stack Finisher] {stackTarget.info.name} đạt ngưỡng Stack '{stackId}' ({stackSetting.stackThreshold})! Kích hoạt hiệu ứng.");

                    if (applicationTarget == StackApplicationTarget.Self && skill.activatedBuff.statToModify != StatType.None)
                    {
                        int buffAmount = skill.activatedBuff.amount;
                        targetBuffManager.ApplyBuff(skill.activatedBuff, null, buffAmount);
                    }
                    else if (applicationTarget == StackApplicationTarget.Target && stackTarget.debuffManager != null)
                    {
                        stackTarget.debuffManager.ApplyDebuff(character, skill.activatedDebuff);
                        Debug.Log($"[Stack Finisher Activated] {stackTarget.info.name} nhận Debuff {skill.activatedDebuff.statToModify}");
                    }

                    currentStackDataForFinisher.currentStacks = 0;
                    Debug.Log($"Stack '{stackId}' của {stackTarget.info.name} đã được Reset về 0.");
                }
            }

            if (stackTarget.battleUIManager != null)
            {
                stackTarget.battleUIManager.UpdateCharacterUI(stackTarget);
            }
        }

        public void RemoveExpiredAttackBuff()
        {
            if (attackBuffTurnsRemaining > 0) return;

            stats.physicalAttack = originalBaseAttack;

            if (attackVFXInstance != null)
            {
                attackVFXInstance.transform.SetParent(null);
                attackVFXInstance.ReturnToPool();
                attackVFXInstance = null;
            }

            originalBaseAttack = 0;
            attackBuffTurnsRemaining = 0;
            attackBuffIcon = null;

            Debug.Log($"Buff Attack của {character.name} đã hết hạn và bị gỡ bỏ. Attack hiện tại: {stats.physicalAttack}");
        }

        public void RemoveExpiredShield()
        {
            if (shieldTurnsRemaining > 0) return;

            if (shieldVFXInstance != null)
            {
                shieldVFXInstance.transform.SetParent(null);
                shieldVFXInstance.ReturnToPool();
                shieldVFXInstance = null;
            }

            stats.currentShield = 0;
            baseShieldAmount = 0;
            shieldTurnsRemaining = 0;
            shieldIcon = null;

            Debug.Log($"Shield của {character.name} đã hết hạn và bị gỡ bỏ. Shield hiện tại: {stats.currentShield}");

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }
        public void RemoveExpiredMaxHPBuff()
        {
            if (maxHPBuffTurnsRemaining > 0 || originalBaseMaxHP == 0) return;

            int oldMaxHP = stats.maxHP;
            stats.maxHP = originalBaseMaxHP;

            if (stats.currentHP > stats.maxHP)
            {
                stats.currentHP = stats.maxHP;
            }

            if (maxHPVFXInstance != null)
            {
                maxHPVFXInstance.transform.SetParent(null);
                maxHPVFXInstance.ReturnToPool();
                maxHPVFXInstance = null;
            }

            originalBaseMaxHP = 0;
            maxHPBuffTurnsRemaining = 0;
            maxHPBuffIcon = null;

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"Buff MaxHP của {character.name} đã hết hạn. MaxHP mới: {stats.maxHP}");
        }


        public void RemoveExpiredDefenseBuff()
        {
            if (defenseBuffTurnsRemaining > 0 || originalBaseDefense == 0) return;

            if (defenseVFXInstance != null)
            {
                defenseVFXInstance.transform.SetParent(null);
                defenseVFXInstance.ReturnToPool();
                defenseVFXInstance = null;
            }

            originalBaseDefense = 0;
            defenseBuffTurnsRemaining = 0;
            defenseBuffIcon = null;
            defenseBuffAmount = 0; 

            RecalculateDefenseStat();

            Debug.Log($"Buff Defense của {character.name} đã hết hạn và bị gỡ bỏ. Defense hiện tại: {stats.physicalDefense}");
        }


        public void RemoveExpiredAgilityBuff()
        {
            if (agilityBuffTurnsRemaining > 0 || originalBaseAgility == 0) return;

            if (agilityVFXInstance != null)
            {
                agilityVFXInstance.transform.SetParent(null);
                agilityVFXInstance.ReturnToPool();
                agilityVFXInstance = null;
            }

            agilityBuffTurnsRemaining = 0;
            agilityBuffAmount = 0;
            agilityBuffIcon = null;

            RecalculateSpeedStat();

            Debug.Log($"Buff Agility của {character.name} đã hết hạn. Agility hiện tại: {stats.agility}");
        }

        public void RemoveExpiredMagicalAttackBuff()
        {
            if (magicalAttackBuffTurnsRemaining > 0 || magicalOriginalBaseAttack == 0) return;

            stats.magicAttack = magicalOriginalBaseAttack;

            if (magicalAttackVFXInstance != null)
            {
                magicalAttackVFXInstance.transform.SetParent(null);
                magicalAttackVFXInstance.ReturnToPool();
                magicalAttackVFXInstance = null;
            }

            magicalOriginalBaseAttack = 0;
            magicalAttackBuffTurnsRemaining = 0;
            magicalAttackBuffIcon = null;

            Debug.Log($"Buff Magical Attack của {character.name} đã hết hạn và bị gỡ bỏ. Magical Attack hiện tại: {stats.magicAttack}");
        }

        public void RemoveExpiredMagicalDefenseBuff()
        {
            if (magicalDefenseBuffTurnsRemaining > 0 || magicalOriginalBaseDefense == 0) return;


            if (magicalDefenseVFXInstance != null)
            {
                magicalDefenseVFXInstance.transform.SetParent(null);
                magicalDefenseVFXInstance.ReturnToPool();
                magicalDefenseVFXInstance = null;
            }

            magicalOriginalBaseDefense = 0;
            magicalDefenseBuffTurnsRemaining = 0;
            magicalDefenseBuffIcon = null;
            magicalDefenseBuffAmount = 0;

            RecalculateDefenseStat();

            Debug.Log($"Buff Magical Defense của {character.name} đã hết hạn và bị gỡ bỏ. Magical Defense hiện tại: {stats.magicDefense}");
        }

        public void RecalculateDefenseStat()
        {
            if (character.debuffManager == null) return;
            float defReductionPercentage = character.debuffManager.defReductionPercentage;

            int finalPDef = originalBaseDefense;

            if (defenseBuffTurnsRemaining > 0)
            {
                finalPDef += defenseBuffAmount;
            }

            if (defReductionPercentage > 0f)
            {
                float reduction = finalPDef * defReductionPercentage;
                finalPDef -= Mathf.FloorToInt(reduction);
            }

            stats.physicalDefense = Mathf.Max(0, finalPDef);


            int finalMDef = magicalOriginalBaseDefense;

            if (magicalDefenseBuffTurnsRemaining > 0)
            {
                finalMDef += magicalDefenseBuffAmount;
            }

            if (defReductionPercentage > 0f)
            {
                float reduction = finalMDef * defReductionPercentage;
                finalMDef -= Mathf.FloorToInt(reduction);
            }

            stats.magicDefense = Mathf.Max(0, finalMDef);

            Debug.Log($"[{character.name}] Recalculate: PDef={stats.physicalDefense}, MDef={stats.magicDefense}. Debuff: -{defReductionPercentage * 100:F0}%");
            character.UpdateOwnUI();
        }

        public void RecalculateSpeedStat()
        {
            if (character.debuffManager == null) return;

            float speedReductionPercentage = character.debuffManager.speedReductionPercentage;

            int finalAgility = originalBaseAgility;

            if (agilityBuffTurnsRemaining > 0)
            {
                finalAgility += agilityBuffAmount;
            }

            if (speedReductionPercentage > 0f)
            {
                float reduction = finalAgility * speedReductionPercentage;
                finalAgility -= Mathf.FloorToInt(reduction);
            }

            stats.agility = Mathf.Max(0, finalAgility);

            Debug.Log($"[{character.name}] Recalculate: Agility={stats.agility}. Debuff: -{speedReductionPercentage * 100:F0}%");
            character.UpdateOwnUI();
        }

        public bool IsBuffActive(StatType statType)
        {
            switch (statType)
            {
                case StatType.Attack:
                    return attackBuffTurnsRemaining > 0;
                case StatType.Defense:
                    return defenseBuffTurnsRemaining > 0;
                case StatType.Agility:
                    return agilityBuffTurnsRemaining > 0;
                case StatType.MaxHP:
                    return maxHPBuffTurnsRemaining > 0;
                case StatType.MagicalAttack:
                    return magicalAttackBuffTurnsRemaining > 0;
                case StatType.MagicalDefense:
                    return magicalDefenseBuffTurnsRemaining > 0;
                default:
                    return false;
            }
        }

        public int GetBuffTurnsRemaining(StatType statType)
        {
            switch (statType)
            {
                case StatType.Attack:
                    return attackBuffTurnsRemaining;
                case StatType.Defense:
                    return defenseBuffTurnsRemaining;
                case StatType.Agility:
                    return agilityBuffTurnsRemaining;
                case StatType.MaxHP:
                    return maxHPBuffTurnsRemaining;
                case StatType.MagicalAttack:
                    return magicalAttackBuffTurnsRemaining;
                case StatType.MagicalDefense:
                    return magicalDefenseBuffTurnsRemaining;
                default:
                    return 0; 
            }
        }


        public void ProcessTurnStartDecay()
        {
            bool uiUpdateNeeded = false;

            if (attackBuffTurnsRemaining > 0)
            {
                attackBuffTurnsRemaining--;
                if (attackBuffTurnsRemaining <= 0)
                {
                    RemoveExpiredAttackBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (maxHPBuffTurnsRemaining > 0)
            {
                maxHPBuffTurnsRemaining--;
                if (maxHPBuffTurnsRemaining <= 0)
                {
                    RemoveExpiredMaxHPBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (defenseBuffTurnsRemaining > 0)
            {
                defenseBuffTurnsRemaining--;
                if (defenseBuffTurnsRemaining <= 0)
                {
                    RemoveExpiredDefenseBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (agilityBuffTurnsRemaining > 0)
            {
                agilityBuffTurnsRemaining--;
                if (agilityBuffTurnsRemaining <= 0)
                {
                    RemoveExpiredAgilityBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (shieldTurnsRemaining > 0)
            {
                shieldTurnsRemaining--;
                if (shieldTurnsRemaining <= 0)
                {
                    RemoveExpiredShield();
                    uiUpdateNeeded = true;
                }
            }

            if (magicalDefenseBuffTurnsRemaining > 0)
            {
                magicalDefenseBuffTurnsRemaining--;
                if (magicalDefenseBuffTurnsRemaining <= 0)
                {
                    RemoveExpiredMagicalDefenseBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (magicalAttackBuffTurnsRemaining > 0)
            {
                magicalAttackBuffTurnsRemaining--;
                if (magicalAttackBuffTurnsRemaining <= 0)
                {
                    RemoveExpiredMagicalAttackBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (basicAttackBuffTurnsRemaining > 0)
            {
                basicAttackBuffTurnsRemaining--;
                if (basicAttackBuffTurnsRemaining <= 0)
                {
                    basicAttackBuffAmount = 0;
                    basicAttackBuffIcon = null;
                }
            }

            if (uiUpdateNeeded && character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }
    }
}