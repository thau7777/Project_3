using UnityEngine;
using System.Collections.Generic;

namespace Turnbase
{
    public class CharacterBuffManager : MonoBehaviour
    {
        private CharacterStats stats;
        private Character character;


        [Header("Attack Buff")]
        [HideInInspector] public int attackBuffTurnsRemaining = 0;
        [HideInInspector] private int originalBaseAttack = 0;
        [HideInInspector] public GameObject attackVFXInstance;

        [Header("MAX HP Buff")]
        [HideInInspector] private int originalBaseMaxHP = 0;
        [HideInInspector] public int maxHPBuffTurnsRemaining = 0;
        [HideInInspector] public GameObject maxHPVFXInstance;

        [Header("Defense Buff")]
        [HideInInspector] private int originalBaseDefense = 0;
        [HideInInspector] public int defenseBuffTurnsRemaining = 0;
        [HideInInspector] public GameObject defenseVFXInstance;

        [Header("Agility Buff")]
        [HideInInspector] private int originalBaseAgility = 0;
        [HideInInspector] public int agilityBuffTurnsRemaining = 0;
        [HideInInspector] public GameObject agilityVFXInstance;

        [Header("Magical Attack Buff")]
        [HideInInspector] public int magicalAttackBuffTurnsRemaining = 0;
        [HideInInspector] private int magicalOriginalBaseAttack = 0;
        [HideInInspector] public GameObject magicalAttackVFXInstance;

        [Header("Magical Defense Buff")]
        [HideInInspector] private int magicalOriginalBaseDefense = 0;
        [HideInInspector] public int magicalDefenseBuffTurnsRemaining = 0;
        [HideInInspector] public GameObject magicalDefenseVFXInstance;


        [Header("Shield")]
        [HideInInspector] private int baseShieldAmount = 0;
        [HideInInspector] public int shieldTurnsRemaining = 0;
        [HideInInspector] public GameObject shieldVFXInstance;

        void Awake()
        {
            character = GetComponent<Character>();
            if (character != null)
            {
                stats = character.stats;
            }
        }

        public void AddShield(int amount, int duration = 2, GameObject vfxInstance = null)
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

            stats.currentShield = Mathf.Min(stats.currentShield + amount, stats.maxShield);

            Debug.Log(character.gameObject.name + " đã nhận thêm " + amount + " Shield. Shield hiện tại: " + stats.currentShield);

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }
        }


        public void ApplyAttackBuff(int amount, int duration, GameObject vfxInstance)
        {
            if (amount <= 0 || duration <= 0) return;

            if (attackBuffTurnsRemaining <= 0)
            {
                originalBaseAttack = stats.attack;
                stats.attack += amount;
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

            attackBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Attack, hiệu lực {duration} lượt. Attack hiện tại: {stats.attack}");
        }

        public void ApplyMaxHPBuff(int amount, int duration, GameObject vfxInstance)
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

            maxHPBuffTurnsRemaining = duration;

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} MaxHP, hiệu lực {duration} lượt. MaxHP hiện tại: {stats.maxHP}");
        }

        public void ApplyDefenseBuff(int amount, int duration, GameObject vfxInstance)
        {
            if (amount <= 0 || duration <= 0) return;

            if (defenseBuffTurnsRemaining <= 0)
            {
                originalBaseDefense = stats.defense;
                stats.defense += amount;
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

            defenseBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Defense, hiệu lực {duration} lượt. Defense hiện tại: {stats.defense}");
        }

        public void ApplyAgilityBuff(int amount, int duration, GameObject vfxInstance)
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

            agilityBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Agility, hiệu lực {duration} lượt. Agility hiện tại: {stats.agility}");
        }

        public void ApplyMagicalAttackBuff(int amount, int duration, GameObject vfxInstance)
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

            magicalAttackBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Magical Attack, hiệu lực {duration} lượt. Magical Attack hiện tại: {stats.magicAttack}");
        }

        public void ApplyMagicalDefenseBuff(int amount, int duration, GameObject vfxInstance)
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

            magicalDefenseBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Magical Defense, hiệu lực {duration} lượt. Magical Defense hiện tại: {stats.magicDefense}");
        }


        public void RemoveExpiredAttackBuff()
        {
            if (attackBuffTurnsRemaining > 0) return;

            stats.attack = originalBaseAttack;

            if (attackVFXInstance != null)
            {
                Destroy(attackVFXInstance);
                attackVFXInstance = null;
            }

            originalBaseAttack = 0;
            attackBuffTurnsRemaining = 0;

            Debug.Log($"Buff Attack của {character.name} đã hết hạn và bị gỡ bỏ. Attack hiện tại: {stats.attack}");
        }

        public void RemoveExpiredShield()
        {
            if (shieldTurnsRemaining > 0) return;

            stats.currentShield = Mathf.Max(0, stats.currentShield - baseShieldAmount);

            if (shieldVFXInstance != null)
            {
                Destroy(shieldVFXInstance);
                shieldVFXInstance = null;
            }

            baseShieldAmount = 0;
            shieldTurnsRemaining = 0;

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
                Destroy(maxHPVFXInstance);
                maxHPVFXInstance = null;
            }

            originalBaseMaxHP = 0;
            maxHPBuffTurnsRemaining = 0;

            if (character.battleManager != null)
            {
                character.battleUIManager.UpdateCharacterUI(character);
            }

            Debug.Log($"Buff MaxHP của {character.name} đã hết hạn. MaxHP mới: {stats.maxHP}");
        }


        public void RemoveExpiredDefenseBuff()
        {
            if (defenseBuffTurnsRemaining > 0 || originalBaseDefense == 0) return;

            stats.defense = originalBaseDefense;

            if (defenseVFXInstance != null)
            {
                Destroy(defenseVFXInstance);
                defenseVFXInstance = null;
            }

            originalBaseDefense = 0;
            defenseBuffTurnsRemaining = 0;

            Debug.Log($"Buff Defense của {character.name} đã hết hạn và bị gỡ bỏ. Defense hiện tại: {stats.defense}");
        }


        public void RemoveExpiredAgilityBuff()
        {
            if (agilityBuffTurnsRemaining > 0 || originalBaseAgility == 0) return;

            stats.agility = originalBaseAgility;

            if (agilityVFXInstance != null)
            {
                Destroy(agilityVFXInstance);
                agilityVFXInstance = null;
            }

            originalBaseAgility = 0;
            agilityBuffTurnsRemaining = 0;

            Debug.Log($"Buff Agility của {character.name} đã hết hạn và bị gỡ bỏ. Agility hiện tại: {stats.agility}");
        }

        public void RemoveExpiredMagicalAttackBuff()
        {
            if (magicalAttackBuffTurnsRemaining > 0 || magicalOriginalBaseAttack == 0) return;

            stats.magicAttack = magicalOriginalBaseAttack;

            if (magicalAttackVFXInstance != null)
            {
                Destroy(magicalAttackVFXInstance);
                magicalAttackVFXInstance = null;
            }

            magicalOriginalBaseAttack = 0;
            magicalAttackBuffTurnsRemaining = 0;

            Debug.Log($"Buff Magical Attack của {character.name} đã hết hạn và bị gỡ bỏ. Magical Attack hiện tại: {stats.magicAttack}");
        }

        public void RemoveExpiredMagicalDefenseBuff()
        {
            if (magicalDefenseBuffTurnsRemaining > 0 || magicalOriginalBaseDefense == 0) return;

            stats.magicDefense = magicalOriginalBaseDefense;

            if (magicalDefenseVFXInstance != null)
            {
                Destroy(magicalDefenseVFXInstance);
                magicalDefenseVFXInstance = null;
            }

            magicalOriginalBaseDefense = 0;
            magicalDefenseBuffTurnsRemaining = 0;

            Debug.Log($"Buff Magical Defense của {character.name} đã hết hạn và bị gỡ bỏ. Magical Defense hiện tại: {stats.magicDefense}");
        }

        public void ProcessTurnStartDecay()
        {
            if (character.buffManager != null)
            {
                if (character.buffManager.attackBuffTurnsRemaining > 0)
                {
                    character.buffManager.attackBuffTurnsRemaining--;
                    character.buffManager.RemoveExpiredAttackBuff();
                }

                if (character.buffManager.maxHPBuffTurnsRemaining > 0)
                {
                    character.buffManager.maxHPBuffTurnsRemaining--;
                    character.buffManager.RemoveExpiredMaxHPBuff();
                }

                if (character.buffManager.defenseBuffTurnsRemaining > 0)
                {
                    character.buffManager.defenseBuffTurnsRemaining--;
                    character.buffManager.RemoveExpiredDefenseBuff();
                }

                if (character.buffManager.agilityBuffTurnsRemaining > 0)
                {
                    character.buffManager.agilityBuffTurnsRemaining--;
                    character.buffManager.RemoveExpiredAgilityBuff();
                }

                if (character.buffManager.shieldTurnsRemaining > 0)
                {
                    character.buffManager.shieldTurnsRemaining--;
                    character.buffManager.RemoveExpiredShield();
                }

                if (character.buffManager.magicalDefenseBuffTurnsRemaining > 0)
                {
                    character.buffManager.magicalDefenseBuffTurnsRemaining--;
                    character.buffManager.RemoveExpiredMagicalDefenseBuff();
                }
                if (character.buffManager.magicalAttackBuffTurnsRemaining > 0)
                {
                    character.buffManager.magicalAttackBuffTurnsRemaining--;
                    character.buffManager.RemoveExpiredMagicalAttackBuff();
                }
            }
        }
    }
}