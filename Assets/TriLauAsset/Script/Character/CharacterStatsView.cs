using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class CharacterStatsView : Singleton<CharacterStatsView>
    {
        [SerializeField] private TextMeshProUGUI backStory;

        [SerializeField] private TextMeshProUGUI vigor;
        [SerializeField] private TextMeshProUGUI mind;
        [SerializeField] private TextMeshProUGUI endurance;
        [SerializeField] private TextMeshProUGUI str;
        [SerializeField] private TextMeshProUGUI dex;
        [SerializeField] private TextMeshProUGUI intell;
        [SerializeField] private TextMeshProUGUI fai;
        [SerializeField] private TextMeshProUGUI arc;

        [SerializeField] private TextMeshProUGUI hp;
        [SerializeField] private TextMeshProUGUI fp;
        [SerializeField] private TextMeshProUGUI stamina;
        [SerializeField] private TextMeshProUGUI critChance;
        [SerializeField] private TextMeshProUGUI critMult;

        [SerializeField] private TextMeshProUGUI physDmg;
        [SerializeField] private TextMeshProUGUI magDmg;
        [SerializeField] private TextMeshProUGUI fireDmg;
        [SerializeField] private TextMeshProUGUI lightningDmg;
        [SerializeField] private TextMeshProUGUI waterDmg;
        [SerializeField] private TextMeshProUGUI frostDmg;
        [SerializeField] private TextMeshProUGUI holyDmg;
        [SerializeField] private TextMeshProUGUI darkDmg;
        [SerializeField] private TextMeshProUGUI poisonDmg;

        [SerializeField] private TextMeshProUGUI physDef;
        [SerializeField] private TextMeshProUGUI magDef;
        [SerializeField] private TextMeshProUGUI fireDef;
        [SerializeField] private TextMeshProUGUI lightningDef;
        [SerializeField] private TextMeshProUGUI waterDef;
        [SerializeField] private TextMeshProUGUI frostDef;
        [SerializeField] private TextMeshProUGUI holyDef;
        [SerializeField] private TextMeshProUGUI darkDef;
        [SerializeField] private TextMeshProUGUI poisonDef;

        private EventBinding<CharacterUpdatedEvent> eventBinding;

        private void OnEnable()
        {
            eventBinding = new EventBinding<CharacterUpdatedEvent>(HandleCharacterStats);
            EventBus<CharacterUpdatedEvent>.Register(eventBinding);
        }

        private void OnDisable()
        {
            EventBus<CharacterUpdatedEvent>.Deregister(eventBinding);
        }

        private void HandleCharacterStats(CharacterUpdatedEvent evt)
        {
            UpdateStats(evt.character);
        }

        public void UpdateStats(CharacterData character)
        {
            if (backStory != null) backStory.text = character.BackStory;

            if (vigor != null) vigor.text = character.CharacterStatsData.AttributesData.Vigor.ToString();
            if (mind != null) mind.text = character.CharacterStatsData.AttributesData.Mind.ToString();
            if (endurance != null) endurance.text = character.CharacterStatsData.AttributesData.Endurance.ToString();
            if (str != null) str.text = character.CharacterStatsData.AttributesData.Strength.ToString();
            if (dex != null) dex.text = character.CharacterStatsData.AttributesData.Dexterity.ToString();
            if (intell != null) intell.text = character.CharacterStatsData.AttributesData.Intelligence.ToString();
            if (fai != null) fai.text = character.CharacterStatsData.AttributesData.Faith.ToString();
            if (arc != null) arc.text = character.CharacterStatsData.AttributesData.Arcane.ToString();

            if (hp != null) hp.text = character.CharacterStatsData.BaseStatsData.CurrentHealth.ToString();
            if (fp != null) fp.text = character.CharacterStatsData.BaseStatsData.CurrentMana.ToString();
            if (stamina != null) stamina.text = character.CharacterStatsData.BaseStatsData.CurrentStamina.ToString();
            if (critChance != null) critChance.text = character.CharacterStatsData.BaseStatsData.CritChance.ToString() + "%";
            if (critMult != null) critMult.text = character.CharacterStatsData.BaseStatsData.CritMult.ToString();

            if (physDmg != null) physDmg.text = character.CharacterStatsData.Damage.PhysDmg.ToString();
            if (magDmg != null) magDmg.text = character.CharacterStatsData.Damage.MagDmg.ToString();
            if (fireDmg != null) fireDmg.text = character.CharacterStatsData.Damage.FireDmg.ToString();
            if (lightningDmg != null) lightningDmg.text = character.CharacterStatsData.Damage.LightningDmg.ToString();
            if (waterDmg != null) waterDmg.text = character.CharacterStatsData.Damage.WaterDmg.ToString();
            if (frostDmg != null) frostDmg.text = character.CharacterStatsData.Damage.FrostDmg.ToString();
            if (holyDmg != null) holyDmg.text = character.CharacterStatsData.Damage.HolyDmg.ToString();
            if (darkDmg != null) darkDmg.text = character.CharacterStatsData.Damage.DarkDmg.ToString();
            if (poisonDmg != null) poisonDmg.text = character.CharacterStatsData.Damage.PoisonDmg.ToString();

            if (physDef != null) physDef.text = character.CharacterStatsData.Defense.PhysDef.ToString();
            if (magDef != null) magDef.text = character.CharacterStatsData.Defense.MagDef.ToString();
            if (fireDef != null) fireDef.text = character.CharacterStatsData.Defense.FireDef.ToString();
            if (lightningDef != null) lightningDef.text = character.CharacterStatsData.Defense.LightningDef.ToString();
            if (waterDef != null) waterDef.text = character.CharacterStatsData.Defense.WaterDef.ToString();
            if (frostDef != null) frostDef.text = character.CharacterStatsData.Defense.FrostDef.ToString();
            if (holyDef != null) holyDef.text = character.CharacterStatsData.Defense.HolyDef.ToString();
            if (darkDef != null) darkDef.text = character.CharacterStatsData.Defense.DarkDef.ToString();
            if (poisonDef != null) poisonDef.text = character.CharacterStatsData.Defense.PoisonDef.ToString();
        }
    }
}