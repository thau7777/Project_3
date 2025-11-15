using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;


namespace Turnbase
{
    public class CharacterStatUI : MonoBehaviour
    {
        [Header("Dynamic Stat Display")]
        public StatEntry statEntryPrefab;
        public Transform statsContainer;

        [Header("Effect Stat Display")]
        public StatusEffectEntry statusEffectEntryPrefab;
        public Transform statusEffectsContainer;


        [Header("Fixed Components")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI level;
        public GameObject statPanel;

        private Dictionary<string, StatEntry> activeStatEntries = new Dictionary<string, StatEntry>();
        private Dictionary<string, StatusEffectEntry> activeEffectEntries = new Dictionary<string, StatusEffectEntry>(); // Đã đổi kiểu dữ liệu
        private Character currentCharacter;

        void Start()
        {
            HideStats();
        }

        private void OnEnable()
        {
            if (currentCharacter != null)
            {
                UpdateStatsUI();
            }
        }

        public void ShowStats(Character character)
        {
            currentCharacter = character;

            if (currentCharacter == null || currentCharacter.info == null)
            {
                HideStats();
                return;
            }

            characterName.text = currentCharacter.info.name;
            level.text = $"Lv. {currentCharacter.info.level}";

            ClearOldStats();
            UpdateStatsUI();

            statPanel.SetActive(true);
        }

        public void UpdateStatsUI()
        {
            if (currentCharacter == null || currentCharacter.stats == null || currentCharacter.info == null)
            {
                HideStats();
                return;
            }

            characterName.text = currentCharacter.info.name;
            level.text = $"Lv. {currentCharacter.info.level}";

            List<StatData> statsToShow = GetStatsList(currentCharacter.stats);
            UpdateOrCreateStatEntries(statsToShow);

            UpdateStatusEffects();
        }

        private List<StatData> GetStatsList(CharacterStats stats)
        {
            CharacterInfo info = currentCharacter.info;

            return new List<StatData>
            {
                new StatData { Name = "Name", Value = info.name },
                new StatData { Name = "Level", Value = info.level.ToString() },

                new StatData { Name = "HP", Value = $"{stats.currentHP}/{stats.maxHP}" },
                new StatData { Name = "MP", Value = $"{stats.currentMP}/{stats.maxMP}" },
                new StatData { Name = "Shield", Value = $"{stats.currentShield}/{stats.maxShield}" },

                new StatData { Name = "P. Attack", Value = stats.physicalAttack.ToString() },
                new StatData { Name = "M. Attack", Value = stats.magicAttack.ToString() },
                new StatData { Name = "P. Defense", Value = stats.physicalDefense.ToString() },
                new StatData { Name = "M. Defense", Value = stats.magicDefense.ToString() },

                new StatData { Name = "Crit Chance", Value = stats.crit.ToString(), Suffix = "%" },
                new StatData { Name = "Crit Damage", Value = stats.critDamage.ToString(), Suffix = "%" },
                new StatData { Name = "Agility", Value = stats.agility.ToString() },
            };
        }

        private void UpdateOrCreateStatEntries(List<StatData> stats)
        {
            HashSet<string> keysToRemove = new HashSet<string>(activeStatEntries.Keys);

            foreach (var stat in stats)
            {
                keysToRemove.Remove(stat.Name);

                if (activeStatEntries.TryGetValue(stat.Name, out StatEntry entry))
                {
                    entry.Setup(stat.Name, stat.Value + stat.Suffix);
                }
                else if (statEntryPrefab != null && statsContainer != null)
                {
                    StatEntry newEntry = Instantiate(statEntryPrefab, statsContainer);
                    newEntry.Setup(stat.Name, stat.Value + stat.Suffix);
                    activeStatEntries.Add(stat.Name, newEntry);
                }
            }

            foreach (string key in keysToRemove)
            {
                if (activeStatEntries.TryGetValue(key, out StatEntry entryToRemove))
                {
                    Destroy(entryToRemove.gameObject);
                    activeStatEntries.Remove(key);
                }
            }
        }

        private void UpdateStatusEffects()
        {
            if (currentCharacter == null) return;

            List<StatusEffectData> effectsToShow = currentCharacter.GetActiveStatusEffects();

            HashSet<string> keysToRemove = new HashSet<string>(activeEffectEntries.Keys);

            foreach (var effect in effectsToShow)
            {
                keysToRemove.Remove(effect.Name);
                string displayValue = $"{effect.Detail} ({effect.TurnsRemaining} turn)";

                if (activeEffectEntries.TryGetValue(effect.Name, out StatusEffectEntry entry))
                {
                    entry.Setup(effect.Name, displayValue);
                    entry.UpdateVisuals(effect);
                }
                else if (statusEffectEntryPrefab != null && statusEffectsContainer != null)
                {
                    StatusEffectEntry newEntry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
                    newEntry.Setup(effect.Name, displayValue);
                    newEntry.UpdateVisuals(effect);
                    activeEffectEntries.Add(effect.Name, newEntry);
                }
            }

            foreach (string key in keysToRemove)
            {
                Destroy(activeEffectEntries[key].gameObject);
                activeEffectEntries.Remove(key);
            }
        }

        private void ClearOldStats()
        {
            foreach (var entry in activeStatEntries.Values)
            {
                Destroy(entry.gameObject);
            }
            activeStatEntries.Clear();

            foreach (var entry in activeEffectEntries.Values)
            {
                Destroy(entry.gameObject);
            }
            activeEffectEntries.Clear();
        }

        public void HideStats()
        {
            statPanel.SetActive(false);
            currentCharacter = null;
            ClearOldStats();
        }
    }
}