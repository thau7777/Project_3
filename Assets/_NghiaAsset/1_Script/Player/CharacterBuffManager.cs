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

        [Header("Splash Attack Buff")]
        [HideInInspector] public int splashAttackTurnsRemaining = 0;
        [HideInInspector] public float splashDamagePercentage = 0f; 
        [HideInInspector] public Sprite splashAttackIcon;

        [Header("Divine Shield (One-Time Block)")]
        [HideInInspector] public bool hasDivineShield = false;
        [HideInInspector] public Flyweight_TB divineShieldVFXInstance;
        [HideInInspector] public Sprite divineShieldIcon;

        [Header("LifeForPower Buff")]
        [HideInInspector] public int lifeForPowerTurnsRemaining = 0;
        [HideInInspector] public int lifeForPowerBonusDamage = 0;
        [HideInInspector] public Flyweight_TB lifeForPowerVFXInstance;
        [HideInInspector] public Sprite lifeForPowerIcon;

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

        public void ApplyMagicalDefenseBuff(int amount, int duration, Flyweight_TB vfxInstance, Sprite icon, Skill sourceSkill = null)
        {
            if (amount <= 0 || duration <= 0) return;


            if (magicalDefenseBuffTurnsRemaining <= 0)
            {
                magicalOriginalBaseDefense = stats.magicDefense;
                magicalDefenseBuffAmount = amount;
                stats.magicDefense += amount;
            }

            magicalDefenseVFXInstance = vfxInstance;
            magicalDefenseBuffIcon = icon;
            magicalDefenseBuffTurnsRemaining = duration;

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));
            Debug.Log($"[BUFF] {character.name} đã bật giáp phép. Sẵn sàng phản đòn.");
        }

        public void ApplyBasicAttackBuff(int amount, int duration, Sprite icon)
        {
            basicAttackBuffAmount = amount;
            basicAttackBuffTurnsRemaining = duration;
            basicAttackBuffIcon = icon;

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));
        }

        public void ApplySplashAttackBuff(float percentage, int duration, Sprite icon)
        {
            if (duration <= 0) return;

            splashDamagePercentage = percentage;
            splashAttackTurnsRemaining = duration;
            splashAttackIcon = icon;

            Debug.Log($"<color=cyan>[BUFF]</color> {character.name} nhận Buff Đánh Lan ({percentage * 100}%) trong {duration} lượt!");

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));
        }

        public void ApplyDivineShield(Flyweight_TB vfxInstance, Sprite icon)
        {
            hasDivineShield = true;
            divineShieldIcon = icon;

            if (vfxInstance != null)
            {
                if (divineShieldVFXInstance != null) divineShieldVFXInstance.ReturnToPool();
                divineShieldVFXInstance = vfxInstance;
            }

            Debug.Log($"<color=yellow>[DIVINE SHIELD]</color> {character.name} đã được bảo vệ!");

            character.UpdateOwnUI();
            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));
        }

        public void ApplyLifeForPower(float healthCostPercent, float damageBuffPercent, int duration, Flyweight_TB vfx, Sprite icon)
        {
            if (character == null || character.stats == null) return;

            int healthCost = Mathf.FloorToInt(character.stats.currentHP * healthCostPercent);

            character.stats.currentHP = Mathf.Max(1, character.stats.currentHP - healthCost);

            lifeForPowerBonusDamage = Mathf.FloorToInt(character.stats.physicalAttack * damageBuffPercent);
            lifeForPowerTurnsRemaining = duration;

            lifeForPowerIcon = icon;
            lifeForPowerVFXInstance = vfx;

            character.UpdateOwnUI(); 
            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"[Berserk] {character.name} hiến tế {healthCost} HP (Trừ trực tiếp). HP còn lại: {character.stats.currentHP}");
        }

        public void ApplyBuff(Skill.BuffSettings buffSettings, Flyweight_TB buffVFX, int amount, Skill sourceSkill = null)
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
                    ApplyMagicalDefenseBuff(amount, buffSettings.durationTurns, buffVFX, buffSettings.icon, sourceSkill);
                    break;
                case StatType.BasicAttackDamage:
                    ApplyBasicAttackBuff(amount, buffSettings.durationTurns, buffSettings.icon);
                    break;
                case StatType.SplashDamage:
                    float percentage = amount / 100f;
                    ApplySplashAttackBuff(percentage, buffSettings.durationTurns, buffSettings.icon);
                    break;
                case StatType.DivineShield:
                    ApplyDivineShield(buffVFX, buffSettings.icon);
                    break;
                case StatType.ApplyLifeForPower:
                    float healthCostRate = 0.5f; 
                    float damageBonusRate = amount / 100f;
                    ApplyLifeForPower(healthCostRate, damageBonusRate, buffSettings.durationTurns, buffVFX, buffSettings.icon);
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

            if (applicationTarget == StackApplicationTarget.None || string.IsNullOrEmpty(stackSetting.stackId))
                return;

            if (applicationTarget == StackApplicationTarget.Counter && targetCharacter == character)
            {
                return;
            }

            Character stackTarget = (applicationTarget == StackApplicationTarget.Self) ? character : targetCharacter;

            if (stackTarget == null || !stackTarget.isAlive || stackTarget.buffManager == null) return;

            CharacterBuffManager targetBuffManager = stackTarget.buffManager;
            string stackId = stackSetting.stackId;

            if (stackSetting.isStackFinisher && (applicationTarget == StackApplicationTarget.Target || applicationTarget == StackApplicationTarget.Counter))
            {
                if (stackTarget.debuffManager != null)
                {
                    DebuffType activeType = skill.activatedDebuff.statToModify;
                    if (IsDebuffStackActive(stackTarget.debuffManager, activeType))
                    {
                        return;
                    }
                }
            }

            if (!targetBuffManager.activeStacks.TryGetValue(stackId, out StackData stackData))
            {
                stackData = new StackData { stackId = stackId, currentStacks = 0, icon = stackSetting.iconStack };
                targetBuffManager.activeStacks.Add(stackId, stackData);
            }

            if (stackSetting.isStackBuilder)
            {
                stackData.currentStacks += stackSetting.stackAmountPerUse;
                Debug.Log($"[Stack] {stackTarget.name} bị tích {stackId}. Hiện tại: {stackData.currentStacks}/{stackSetting.stackThreshold}");
            }

            if (stackSetting.isStackFinisher && stackData.currentStacks >= stackSetting.stackThreshold)
            {
                if (applicationTarget == StackApplicationTarget.Self)
                {
                    targetBuffManager.ApplyBuff(skill.activatedBuff, null, skill.activatedBuff.amount, skill);
                }
                else if ((applicationTarget == StackApplicationTarget.Target || applicationTarget == StackApplicationTarget.Counter)
                         && stackTarget.debuffManager != null)
                {
                    stackTarget.debuffManager.ApplyDebuff(this.character, skill.activatedDebuff);
                }

                stackData.currentStacks = 0;
            }

            if (stackTarget.battleUIManager != null)
                stackTarget.battleUIManager.UpdateCharacterUI(stackTarget);
        }
        private bool IsDebuffStackActive(CharacterDebuffManager dm, DebuffType type)
        {
            return type switch
            {
                DebuffType.Burn => dm.burnTurnsRemaining > 0,
                DebuffType.Poison => dm.poisonTurnsRemaining > 0,
                DebuffType.Stun => dm.stunTurnsRemaining > 0,
                _ => false
            };
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

        public void RemoveExpireLifeForPower()
        {
            if (lifeForPowerTurnsRemaining > 0) return;

            if (lifeForPowerVFXInstance != null)
            {
                lifeForPowerVFXInstance.transform.SetParent(null);
                lifeForPowerVFXInstance.ReturnToPool();
                lifeForPowerVFXInstance = null;
            }

            lifeForPowerTurnsRemaining = 0;
            lifeForPowerBonusDamage = 0;
            lifeForPowerIcon = null;

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"Trạng thái Hiến Tế của {character.name} đã hết hạn. Sát thương bonus đã bị gỡ bỏ.");
        }

        public void RecalculateDefenseStat()
        {
            if (character.debuffManager == null) return;

            float defRed = character.debuffManager.defReductionPercentage;
            float poisonRed = character.debuffManager.poisonReductionPercentage;

            float totalReductionPercent = Mathf.Max(defRed, poisonRed);

            int finalPDef = originalBaseDefense;
            if (defenseBuffTurnsRemaining > 0) finalPDef += defenseBuffAmount;

            if (totalReductionPercent > 0f)
            {
                finalPDef -= Mathf.FloorToInt(finalPDef * totalReductionPercent);
            }
            stats.physicalDefense = Mathf.Max(0, finalPDef);

            int finalMDef = magicalOriginalBaseDefense;
            if (magicalDefenseBuffTurnsRemaining > 0) finalMDef += magicalDefenseBuffAmount;

            if (totalReductionPercent > 0f)
            {
                finalMDef -= Mathf.FloorToInt(finalMDef * totalReductionPercent);
            }
            stats.magicDefense = Mathf.Max(0, finalMDef);

            Debug.Log($"[{character.name}] New Def: P={stats.physicalDefense}, M={stats.magicDefense} (Total Red: {totalReductionPercent * 100}%)");
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

        public bool CheckAndConsumeDivineShield()
        {
            if (hasDivineShield)
            {
                hasDivineShield = false; 

                if (divineShieldVFXInstance != null)
                {
                    divineShieldVFXInstance.ReturnToPool();
                    divineShieldVFXInstance = null;
                }
                divineShieldIcon = null;

                Debug.Log($"<color=cyan>[BLOCK]</color> Khiên của {character.name} đã vỡ, sát thương bị triệt tiêu!");

                character.UpdateOwnUI();
                EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));
                return true;
            }
            return false;
        }

        public void RemoveExpiredSplashBuff()
        {
            if (splashAttackTurnsRemaining > 0) return;

            splashDamagePercentage = 0f;
            splashAttackTurnsRemaining = 0;
            splashAttackIcon = null;

            Debug.Log($"<color=gray>[EXPIRED]</color> Buff Đánh Lan của {character.name} đã hết hạn.");

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }

        public void BreakShield()
        {
            stats.currentShield = 0;
            baseShieldAmount = 0;
            shieldTurnsRemaining = 0;
            shieldIcon = null;

            if (shieldVFXInstance != null)
            {
                shieldVFXInstance.transform.SetParent(null);
                shieldVFXInstance.ReturnToPool();
                shieldVFXInstance = null;
            }

            if (character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            EventBusUI<StatusEffectChangedEvent>.Raise(new StatusEffectChangedEvent(character));

            Debug.Log($"<color=yellow>[SHIELD BROKEN]</color> Giáp của {character.name} đã bị đánh vỡ!");
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

            if (splashAttackTurnsRemaining > 0)
            {
                splashAttackTurnsRemaining--;
                if (splashAttackTurnsRemaining <= 0)
                {
                    RemoveExpiredSplashBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (lifeForPowerTurnsRemaining > 0)
            {
                lifeForPowerTurnsRemaining--;
                if (lifeForPowerTurnsRemaining <= 0)
                {
                    lifeForPowerBonusDamage = 0;
                    RemoveExpireLifeForPower();
                    uiUpdateNeeded = true;
                }
            }

            if (uiUpdateNeeded && character.battleUIManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }
    }
}