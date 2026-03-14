using System.Collections.Generic;
using UnityEngine;
using MyRule;

namespace Turnbase
{
    public class CharacterIItemBuffManager : MonoBehaviour
    {
        private Character character;

        [System.Serializable]
        public class ActiveItemBuff
        {
            public string itemName;
            public ItemType type;
            public int value;
            public int duration;
            public Sprite icon;
        }

        public List<ActiveItemBuff> activeBuffs = new List<ActiveItemBuff>();

        private void Awake() => character = GetComponent<Character>();

        public void ApplyItemEffect(Tb_Item item, int duration)
        {
            if (item == null || character == null) return;

            // 1. Xử lý vật phẩm hồi phục tức thì (Instant Recovery)
            if (item.type == ItemType.HealthPotion || item.type == ItemType.ManaPotion)
            {
                if (item.type == ItemType.HealthPotion)
                    character.Heal(item.value);
                else
                    character.RestoreMana(item.value);

                return;
            }

            // 2. Xử lý các vật phẩm tăng chỉ số có thời hạn (Buff)
            AddTimedBuff(item, duration);
        }

        private void AddTimedBuff(Tb_Item item, int duration)
        {
            // Cộng chỉ số ngay khi kích hoạt
            ModifyStat(item.type, item.value);

            activeBuffs.Add(new ActiveItemBuff
            {
                itemName = item.itemName,
                type = item.type,
                value = item.value,
                duration = duration,
                icon = item.icon
            });

            character.UpdateOwnUI();
        }

        public void ProcessTurnDecay()
        {
            if (activeBuffs.Count == 0) return;

            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                activeBuffs[i].duration--;

                if (activeBuffs[i].duration <= 0)
                {
                    // Trừ lại chỉ số khi hết thời gian hiệu lực
                    ModifyStat(activeBuffs[i].type, -activeBuffs[i].value);
                    activeBuffs.RemoveAt(i);
                }
            }
            character.UpdateOwnUI();
        }

        private void ModifyStat(ItemType type, int amount)
        {
            if (character == null || character.stats == null) return;

            var s = character.stats;

            switch (type)
            {
                // Attack Buffs
                case ItemType.PhysicDmgPotion: s.physicalAttack += amount; break;
                case ItemType.MagicDmgPotion: s.magicAttack += amount; break;
                case ItemType.FireDmgPotion: s.fireDamageBonus += amount; break;
                case ItemType.WaterDmgPotion: s.waterDamageBonus += amount; break;
                case ItemType.FrostDmgPotion: s.frostDamageBonus += amount; break;
                case ItemType.HolyDmgPotion: s.holyDamageBonus += amount; break;
                case ItemType.DarkDmgPotion: s.darkDamageBonus += amount; break;
                case ItemType.PoisonDmgPotion: s.poisonDamageBonus += amount; break;
                case ItemType.LightingDmgPotion: s.lightningDamageBonus += amount; break;

                // Defense Buffs
                case ItemType.PhysicDefPotion: s.physicalDefense += amount; break;
                case ItemType.MagicDefPotion: s.magicDefense += amount; break;
                case ItemType.FireDefPotion: s.fireDefense += amount; break;
                case ItemType.WaterDefPotion: s.waterDefense += amount; break;
                case ItemType.FrostDefPotion: s.frostDefense += amount; break;
                case ItemType.HolyDefPotion: s.holyDefense += amount; break;
                case ItemType.DarkDefPotion: s.darkDefense += amount; break;
                case ItemType.PosionDefPotion: s.poisonDefense += amount; break; // Giữ nguyên lỗi typo 'Posion' từ Enum của bạn
                case ItemType.LightningDefPotion: s.lightningDefense += amount; break;

                default:
                    Debug.LogWarning($"[BuffManager] Chưa định nghĩa logic cho loại: {type}");
                    break;
            }
        }
    }
}