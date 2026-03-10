using System.Collections.Generic;
using UnityEngine;
using static Turnbase.Tb_Item;

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

            if (item.type == ItemType.Healing || item.type == ItemType.Mana)
            {
                if (item.type == ItemType.Healing) character.Heal(item.value);
                else character.RestoreMana(item.value);
                return;
            }

            AddTimedBuff(item, duration);
        }

        private void AddTimedBuff(Tb_Item item, int duration)
        {
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
                    ModifyStat(activeBuffs[i].type, -activeBuffs[i].value);

                    activeBuffs.RemoveAt(i);
                }
            }
            character.UpdateOwnUI();
        }

        private void ModifyStat(ItemType type, int amount)
        {
            if (character.stats == null) return;
            var s = character.stats;

            switch (type)
            {
                case ItemType.PhysicalAttack: s.physicalAttack += amount; break;
                case ItemType.MagicalAttack: s.magicAttack += amount; break;
                case ItemType.PhysicalDef: s.physicalDefense += amount; break;
                case ItemType.MagicalDef: s.magicDefense += amount; break;
                case ItemType.FireDef: s.fireDefense += amount; break;
                case ItemType.WaterDef: s.waterDefense += amount; break;
                case ItemType.LightningDef: s.lightningDefense += amount; break;
                case ItemType.PoisonDef: s.poisonDefense += amount; break;
                case ItemType.FrostDef: s.frostDefense += amount; break;
                case ItemType.HolyDef: s.holyDefense += amount; break;
                case ItemType.DarkDef: s.darkDefense += amount; break;
                case ItemType.FireDMG: s.fireDamageBonus += amount; break;
                case ItemType.WaterDMG: s.waterDamageBonus += amount; break;
                case ItemType.LightningDMG: s.lightningDamageBonus += amount; break;
                case ItemType.PoisonDMG: s.poisonDamageBonus += amount; break;
                case ItemType.FrostDMG: s.frostDamageBonus += amount; break;
                case ItemType.HolyDMG: s.holyDamageBonus += amount; break;
                case ItemType.DarkDMG: s.darkDamageBonus += amount; break;
            }
        }
    }
}