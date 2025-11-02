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
        [HideInInspector] public Flyweight attackVFXInstance;
        [HideInInspector] public Sprite attackBuffIcon;

        [Header("MAX HP Buff")]
        [HideInInspector] public int originalBaseMaxHP = 0;
        [HideInInspector] public int maxHPBuffTurnsRemaining = 0;
        [HideInInspector] public Flyweight maxHPVFXInstance;
        [HideInInspector] public Sprite maxHPBuffIcon;

        [Header("Defense Buff")]
        [HideInInspector] public int originalBaseDefense = 0;
        [HideInInspector] public int defenseBuffTurnsRemaining = 0;
        [HideInInspector] public Flyweight defenseVFXInstance;
        [HideInInspector] public Sprite defenseBuffIcon;

        [Header("Agility Buff")]
        [HideInInspector] public int originalBaseAgility = 0;
        [HideInInspector] public int agilityBuffTurnsRemaining = 0;
        [HideInInspector] public Flyweight agilityVFXInstance;
        [HideInInspector] public Sprite agilityBuffIcon;

        [Header("Magical Attack Buff")]
        [HideInInspector] public int magicalAttackBuffTurnsRemaining = 0;
        [HideInInspector] public int magicalOriginalBaseAttack = 0;
        [HideInInspector] public Flyweight magicalAttackVFXInstance;
        [HideInInspector] public Sprite magicalAttackBuffIcon;

        [Header("Magical Defense Buff")]
        [HideInInspector] public int magicalOriginalBaseDefense = 0;
        [HideInInspector] public int magicalDefenseBuffTurnsRemaining = 0;
        [HideInInspector] public Flyweight magicalDefenseVFXInstance;
        [HideInInspector] public Sprite magicalDefenseBuffIcon;


        [Header("Shield")]
        [HideInInspector] public int baseShieldAmount = 0;
        [HideInInspector] public int shieldTurnsRemaining = 0;
        [HideInInspector] public Flyweight shieldVFXInstance;
        [HideInInspector] public Sprite shieldIcon;


        void Awake()
        {
            character = GetComponent<Character>();
            if (character != null)
            {
                stats = character.stats;
            }
        }

        public void AddShield(int amount, int duration, Flyweight vfxInstance, Sprite icon)
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

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }


        public void ApplyAttackBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
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

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} Attack, hiệu lực {duration} lượt. Attack hiện tại: {stats.physicalAttack}");
        }

        public void ApplyMaxHPBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
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

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} MaxHP, hiệu lực {duration} lượt. MaxHP hiện tại: {stats.maxHP}");
        }

        public void ApplyDefenseBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (defenseBuffTurnsRemaining <= 0)
            {
                originalBaseDefense = stats.physicalDefense;
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

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} Defense, hiệu lực {duration} lượt. Defense hiện tại: {stats.physicalDefense}");
        }

        public void ApplyAgilityBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (agilityBuffTurnsRemaining <= 0)
            {
                originalBaseAgility = stats.agility;
                stats.agility += amount;
            }
            else
            {
                Debug.Log($"Buff Agility của {character.name} đã được làm mới thời gian.");
            }

            if (agilityVFXInstance != null && agilityVFXInstance != vfxInstance)
            {
                Destroy(agilityVFXInstance);
            }
            agilityVFXInstance = vfxInstance;
            agilityBuffIcon = icon;

            agilityBuffTurnsRemaining = duration;

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} Agility, hiệu lực {duration} lượt. Agility hiện tại: {stats.agility}");
        }

        public void ApplyMagicalAttackBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
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

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} Magical Attack, hiệu lực {duration} lượt. Magical Attack hiện tại: {stats.magicAttack}");
        }

        public void ApplyMagicalDefenseBuff(int amount, int duration, Flyweight vfxInstance, Sprite icon)
        {
            if (amount <= 0 || duration <= 0) return;

            if (magicalDefenseBuffTurnsRemaining <= 0)
            {
                magicalOriginalBaseDefense = stats.magicDefense;
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

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} Magical Defense, hiệu lực {duration} lượt. Magical Defense hiện tại: {stats.magicDefense}");
        }


        public void ApplyBuff(Skill.BuffSettings buffSettings, Flyweight buffVFX, int amount)
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
                default:
                    Debug.LogWarning($"Loại Buff {buffSettings.statToModify} không được hỗ trợ hoặc không có giá trị.");
                    break;
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

            stats.physicalDefense = originalBaseDefense;

            if (defenseVFXInstance != null)
            {
                defenseVFXInstance.transform.SetParent(null);
                defenseVFXInstance.ReturnToPool();
                defenseVFXInstance = null;
            }

            originalBaseDefense = 0;
            defenseBuffTurnsRemaining = 0;
            defenseBuffIcon = null;

            Debug.Log($"Buff Defense của {character.name} đã hết hạn và bị gỡ bỏ. Defense hiện tại: {stats.physicalDefense}");
        }


        public void RemoveExpiredAgilityBuff()
        {
            if (agilityBuffTurnsRemaining > 0 || originalBaseAgility == 0) return;

            stats.agility = originalBaseAgility;

            if (agilityVFXInstance != null)
            {
                agilityVFXInstance.transform.SetParent(null);
                agilityVFXInstance.ReturnToPool();
                agilityVFXInstance = null;
            }

            originalBaseAgility = 0;
            agilityBuffTurnsRemaining = 0;
            agilityBuffIcon = null;

            Debug.Log($"Buff Agility của {character.name} đã hết hạn và bị gỡ bỏ. Agility hiện tại: {stats.agility}");
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

            stats.magicDefense = magicalOriginalBaseDefense;

            if (magicalDefenseVFXInstance != null)
            {
                magicalDefenseVFXInstance.transform.SetParent(null);
                magicalDefenseVFXInstance.ReturnToPool();
                magicalDefenseVFXInstance = null;
            }

            magicalOriginalBaseDefense = 0;
            magicalDefenseBuffTurnsRemaining = 0;
            magicalDefenseBuffIcon = null;

            Debug.Log($"Buff Magical Defense của {character.name} đã hết hạn và bị gỡ bỏ. Magical Defense hiện tại: {stats.magicDefense}");
        }


        public void ProcessTurnStartDecay()
        {
            bool uiUpdateNeeded = false;

            if (character.buffManager.attackBuffTurnsRemaining > 0)
            {
                character.buffManager.attackBuffTurnsRemaining--;
                if (character.buffManager.attackBuffTurnsRemaining <= 0)
                {
                    character.buffManager.RemoveExpiredAttackBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (character.buffManager.maxHPBuffTurnsRemaining > 0)
            {
                character.buffManager.maxHPBuffTurnsRemaining--;
                if (character.buffManager.maxHPBuffTurnsRemaining <= 0)
                {
                    character.buffManager.RemoveExpiredMaxHPBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (character.buffManager.defenseBuffTurnsRemaining > 0)
            {
                character.buffManager.defenseBuffTurnsRemaining--;
                if (character.buffManager.defenseBuffTurnsRemaining <= 0)
                {
                    character.buffManager.RemoveExpiredDefenseBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (character.buffManager.agilityBuffTurnsRemaining > 0)
            {
                character.buffManager.agilityBuffTurnsRemaining--;
                if (character.buffManager.agilityBuffTurnsRemaining <= 0)
                {
                    character.buffManager.RemoveExpiredAgilityBuff();
                    uiUpdateNeeded = true;
                }
            }

            if (character.buffManager.shieldTurnsRemaining > 0)
            {
                character.buffManager.shieldTurnsRemaining--;
                if (character.buffManager.shieldTurnsRemaining <= 0)
                {
                    character.buffManager.RemoveExpiredShield();
                    uiUpdateNeeded = true;
                }
            }

            if (character.buffManager.magicalDefenseBuffTurnsRemaining > 0)
            {
                character.buffManager.magicalDefenseBuffTurnsRemaining--;
                if (character.buffManager.magicalDefenseBuffTurnsRemaining <= 0)
                {
                    character.buffManager.RemoveExpiredMagicalDefenseBuff();
                    uiUpdateNeeded = true;
                }
            }
            if (character.buffManager.magicalAttackBuffTurnsRemaining > 0)
            {
                character.buffManager.magicalAttackBuffTurnsRemaining--;
                if (character.buffManager.magicalAttackBuffTurnsRemaining <= 0)
                {
                    character.buffManager.RemoveExpiredMagicalAttackBuff();
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