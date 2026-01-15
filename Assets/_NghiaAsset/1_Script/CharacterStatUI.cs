using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;


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

        [Header("Dynamic Skill Display")]
        public SkillEntryUI skillEntryPrefab;
        public Transform skillContainer;

        [Header("Fixed Components")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI level;
        public GameObject statPanel;

        [Header("Tab Buttons")]
        public Button statsTabButton; 
        public Button skillsTabButton;

        [Header("Scroll View Roots (Để ẩn/hiện)")]
        public GameObject statsScrollView;
        public GameObject skillScrollView;

        [Header("Tooltip Reference")]
        public SkillTooltipUI skillTooltip;

        private Dictionary<string, StatEntry> activeStatEntries = new Dictionary<string, StatEntry>();
        private Dictionary<string, StatusEffectEntry> activeEffectEntries = new Dictionary<string, StatusEffectEntry>();
        private Dictionary<string, SkillEntryUI> activeSkillEntries = new Dictionary<string, SkillEntryUI>();
        private Character currentCharacter;
        private CharacterStatusDataProvider currentDataProvider;


        void Awake()
        {
            if (statsTabButton != null)
                statsTabButton.onClick.AddListener(() => SwitchTab(true));

            if (skillsTabButton != null)
                skillsTabButton.onClick.AddListener(() => SwitchTab(false));

            skillTooltip = GetComponent<SkillTooltipUI>();
        }

        void Start()
        {
            HideStats();
        }

        public void SwitchTab(bool showStats)
        {
            if (statsContainer != null) statsContainer.gameObject.SetActive(showStats);
            if (skillContainer != null) skillContainer.gameObject.SetActive(!showStats);

            if (skillTooltip != null)
            {
                skillTooltip.Hide();
            }

            if (showStats) UpdateStatsUI();
            else UpdateSkillsUI();
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
            if (currentCharacter == null) { HideStats(); return; }

            currentDataProvider = currentCharacter.GetComponent<CharacterStatusDataProvider>();

            statPanel.SetActive(true);

            SwitchTab(true);

            UpdateHeader();
            ClearOldStats();
            UpdateStatsUI();
            UpdateSkillsUI();
        }

        private void UpdateHeader()
        {
            if (currentCharacter == null || currentCharacter.info == null) return;
            characterName.text = currentCharacter.info.name;
            level.text = $"Lv. {currentCharacter.info.level}";
        }

        public void UpdateStatsUI()
        {
            if (currentCharacter == null || currentCharacter.stats == null) return;

            UpdateHeader();
            List<StatData> statsToShow = GetStatsList(currentCharacter.stats);
            UpdateOrCreateStatEntries(statsToShow);
            UpdateStatusEffects();
        }

        private void UpdateSkillsUI()
        {
            if (currentCharacter == null || currentCharacter.skills == null || skillContainer == null) return;

            HashSet<string> keysToRemove = new HashSet<string>(activeSkillEntries.Keys);

            foreach (var skill in currentCharacter.skills)
            {
                string skillKey = skill.skillName;
                keysToRemove.Remove(skillKey);

                if (activeSkillEntries.TryGetValue(skillKey, out SkillEntryUI entry))
                {
                    entry.Setup(skill, (s) => OnSkillClicked(s));
                }
                else if (skillEntryPrefab != null)
                {
                    SkillEntryUI newEntry = Instantiate(skillEntryPrefab, skillContainer);
                    newEntry.Setup(skill, (s) => OnSkillClicked(s));
                    activeSkillEntries.Add(skillKey, newEntry);
                }
            }

            foreach (string key in keysToRemove)
            {
                if (activeSkillEntries.ContainsKey(key))
                {
                    Destroy(activeSkillEntries[key].gameObject);
                    activeSkillEntries.Remove(key);
                }
            }
        }
        private void OnSkillClicked(Skill skill)
        {
            if (skillTooltip != null)
            {
                skillTooltip.Show(skill);
            }
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
                    entry.Setup(stat.Name, stat.Value + stat.Suffix);
                else if (statEntryPrefab != null && statsContainer != null)
                {
                    StatEntry newEntry = Instantiate(statEntryPrefab, statsContainer);
                    newEntry.Setup(stat.Name, stat.Value + stat.Suffix);
                    activeStatEntries.Add(stat.Name, newEntry);
                }
            }
            foreach (string key in keysToRemove)
            {
                Destroy(activeStatEntries[key].gameObject);
                activeStatEntries.Remove(key);
            }
        }

        private void UpdateStatusEffects()
        {
            if (currentDataProvider == null) return;
            List<StatusEffectData> effectsToShow = currentDataProvider.GetActiveStatusEffects();
            HashSet<string> keysToRemove = new HashSet<string>(activeEffectEntries.Keys);
            foreach (var effect in effectsToShow)
            {
                keysToRemove.Remove(effect.Name);
                if (activeEffectEntries.TryGetValue(effect.Name, out StatusEffectEntry entry))
                {
                    entry.Setup(effect.Name, $"{effect.Detail} ({effect.TurnsRemaining}t)");
                    entry.UpdateVisuals(effect);
                }
                else if (statusEffectEntryPrefab != null && statusEffectsContainer != null)
                {
                    StatusEffectEntry newEntry = Instantiate(statusEffectEntryPrefab, statusEffectsContainer);
                    newEntry.Setup(effect.Name, $"{effect.Detail} ({effect.TurnsRemaining}t)");
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
            foreach (var entry in activeStatEntries.Values) Destroy(entry.gameObject);
            activeStatEntries.Clear();

            foreach (var entry in activeEffectEntries.Values) Destroy(entry.gameObject);
            activeEffectEntries.Clear();

            foreach (var entry in activeSkillEntries.Values) Destroy(entry.gameObject);
            activeSkillEntries.Clear();
        }

        public void HideStats()
        {
            if (statPanel != null) statPanel.SetActive(false);
            if (skillTooltip != null) skillTooltip.Hide();
            currentCharacter = null;
            ClearOldStats();
        }
    }
}