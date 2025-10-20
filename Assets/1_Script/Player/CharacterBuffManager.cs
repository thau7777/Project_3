using UnityEngine;



namespace Turnbase
{
    public class CharacterBuffManager : MonoBehaviour
    {
        private CharacterStats stats;
        private Character character;


        [Header("Attack Buff")]
        [HideInInspector] public int attackBuffTurnsRemaining = 0;
        [HideInInspector] private int originalBaseAttack = 0;

        [Header("MAX HP Buff")]
        [HideInInspector] private int originalBaseMaxHP = 0;
        [HideInInspector] public int maxHPBuffTurnsRemaining = 0;

        [Header("Defense Buff")]
        [HideInInspector] private int originalBaseDefense = 0;
        [HideInInspector] public int defenseBuffTurnsRemaining = 0;

        [Header("Agility Buff")]
        [HideInInspector] private int originalBaseAgility = 0;
        [HideInInspector] public int agilityBuffTurnsRemaining = 0;

        [HideInInspector] private int baseShieldAmount = 0;
        [HideInInspector] public int shieldTurnsRemaining = 0;

        void Awake()
        {
            character = GetComponent<Character>();
            if (character != null)
            {
                stats = character.stats;
            }
        }

        public void AddShield(int amount, int duration = 2)
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

            shieldTurnsRemaining = duration;

            stats.currentShield = Mathf.Min(stats.currentShield + amount, stats.maxShield);

            Debug.Log(character.gameObject.name + " đã nhận thêm " + amount + " Shield. Shield hiện tại: " + stats.currentShield);

            if (character.battleManager != null)
            {
                character.battleManager.UpdateCharacterUI(character);
            }
        }


        public void ApplyAttackBuff(int amount, int duration)
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

            attackBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Attack, hiệu lực {duration} lượt. Attack hiện tại: {stats.attack}");
        }

        public void ApplyMaxHPBuff(int amount, int duration)
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

            maxHPBuffTurnsRemaining = duration;

            if (character.battleManager != null)
            {
                character.battleManager.UpdateCharacterUI(character);
            }

            Debug.Log($"{character.name} đã nhận buff +{amount} MaxHP, hiệu lực {duration} lượt. MaxHP hiện tại: {stats.maxHP}");
        }

        public void ApplyDefenseBuff(int amount, int duration)
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

            defenseBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Defense, hiệu lực {duration} lượt. Defense hiện tại: {stats.defense}");
        }

        public void ApplyAgilityBuff(int amount, int duration)
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

            agilityBuffTurnsRemaining = duration;
            Debug.Log($"{character.name} đã nhận buff +{amount} Agility, hiệu lực {duration} lượt. Agility hiện tại: {stats.agility}");
        }

        public void RemoveExpiredAttackBuff()
        {
            if (attackBuffTurnsRemaining > 0) return;

            stats.attack = originalBaseAttack;

            originalBaseAttack = 0;
            attackBuffTurnsRemaining = 0;

            Debug.Log($"Buff Attack của {character.name} đã hết hạn và bị gỡ bỏ. Attack hiện tại: {stats.attack}");
        }

        public void RemoveExpiredShield()
        {
            if (shieldTurnsRemaining > 0) return;

            stats.currentShield = Mathf.Max(0, stats.currentShield - baseShieldAmount);

            baseShieldAmount = 0;
            shieldTurnsRemaining = 0;

            Debug.Log($"Shield của {character.name} đã hết hạn và bị gỡ bỏ. Shield hiện tại: {stats.currentShield}");

            if (character.battleManager != null)
            {
                character.battleManager.UpdateCharacterUI(character);
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

            originalBaseMaxHP = 0;
            maxHPBuffTurnsRemaining = 0;

            if (character.battleManager != null)
            {
                character.battleManager.UpdateCharacterUI(character);
            }

            Debug.Log($"Buff MaxHP của {character.name} đã hết hạn. MaxHP mới: {stats.maxHP}");
        }


        public void RemoveExpiredDefenseBuff()
        {
            if (defenseBuffTurnsRemaining > 0 || originalBaseDefense == 0) return;

            stats.defense = originalBaseDefense;

            originalBaseDefense = 0;
            defenseBuffTurnsRemaining = 0;

            Debug.Log($"Buff Defense của {character.name} đã hết hạn và bị gỡ bỏ. Defense hiện tại: {stats.defense}");
        }


        public void RemoveExpiredAgilityBuff()
        {
            if (agilityBuffTurnsRemaining > 0 || originalBaseAgility == 0) return;

            stats.agility = originalBaseAgility;

            originalBaseAgility = 0;
            agilityBuffTurnsRemaining = 0;

            Debug.Log($"Buff Agility của {character.name} đã hết hạn và bị gỡ bỏ. Agility hiện tại: {stats.agility}");
        }
    }
}
