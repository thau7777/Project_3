using TMPro;
using UnityEngine;

namespace MyRule
{
    public class CharacterStatsView : Singleton<CharacterStatsView>
    {
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

        private EventBinding<CharacterStatsUpdatedEvent> eventBinding;

        private void OnEnable()
        {
            eventBinding = new EventBinding<CharacterStatsUpdatedEvent>(UpdateStats);
            EventBus<CharacterStatsUpdatedEvent>.Register(eventBinding);
        }

        private void OnDisable()
        {
            EventBus<CharacterStatsUpdatedEvent>.Deregister(eventBinding);
        }

        public void UpdateStats(CharacterStatsUpdatedEvent evt)
        {
            if (vigor != null) vigor.text = evt.characterStats.AttributesData.Vigor.ToString();
            if (mind != null) mind.text = evt.characterStats.AttributesData.Mind.ToString();
            if (endurance != null) endurance.text = evt.characterStats.AttributesData.Endurance.ToString();
            if (str != null) str.text = evt.characterStats.AttributesData.Strength.ToString();
            if (dex != null) dex.text = evt.characterStats.AttributesData.Dexterity.ToString();
            if (intell != null) intell.text = evt.characterStats.AttributesData.Intelligence.ToString();
            if (fai != null) fai.text = evt.characterStats.AttributesData.Faith.ToString();
            if (arc != null) arc.text = evt.characterStats.AttributesData.Arcane.ToString();

            if (hp != null) hp.text = evt.characterStats.BaseStatsData.CurrentHealth.ToString();
            if (fp != null) fp.text = evt.characterStats.BaseStatsData.CurrentMana.ToString();
            if (stamina != null) stamina.text = evt.characterStats.BaseStatsData.CurrentStamina.ToString();
            if (critChance != null) critChance.text = evt.characterStats.BaseStatsData.CritChance.ToString() + "%";
            if (critMult != null) critMult.text = evt.characterStats.BaseStatsData.CritMult.ToString();

            if (physDmg != null) physDmg.text = evt.characterStats.Damage.PhysDmg.ToString();
            if (magDmg != null) magDmg.text = evt.characterStats.Damage.MagDmg.ToString();
            if (fireDmg != null) fireDmg.text = evt.characterStats.Damage.FireDmg.ToString();
            if (lightningDmg != null) lightningDmg.text = evt.characterStats.Damage.LightningDmg.ToString();
            if (waterDmg != null) waterDmg.text = evt.characterStats.Damage.WaterDmg.ToString();
            if (frostDmg != null) frostDmg.text = evt.characterStats.Damage.FrostDmg.ToString();
            if (holyDmg != null) holyDmg.text = evt.characterStats.Damage.HolyDmg.ToString();
            if (darkDmg != null) darkDmg.text = evt.characterStats.Damage.DarkDmg.ToString();
            if (poisonDmg != null) poisonDmg.text = evt.characterStats.Damage.PoisonDmg.ToString();

            if (physDef != null) physDef.text = evt.characterStats.Defense.PhysDef.ToString();
            if (magDef != null) magDef.text = evt.characterStats.Defense.MagDef.ToString();
            if (fireDef != null) fireDef.text = evt.characterStats.Defense.FireDef.ToString();
            if (lightningDef != null) lightningDef.text = evt.characterStats.Defense.LightningDef.ToString();
            if (waterDef != null) waterDef.text = evt.characterStats.Defense.WaterDef.ToString();
            if (frostDef != null) frostDef.text = evt.characterStats.Defense.FrostDef.ToString();
            if (holyDef != null) holyDef.text = evt.characterStats.Defense.HolyDef.ToString();
            if (darkDef != null) darkDef.text = evt.characterStats.Defense.DarkDef.ToString();
            if (poisonDef != null) poisonDef.text = evt.characterStats.Defense.PoisonDef.ToString();

        }
    }
}