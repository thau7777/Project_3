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

        private void Start()
        {
            CharacterStatsSO character = CharacterStatsManager.Instance.GetCharacterStats();
            UpdateStats(character);
        }

        public void UpdateStats(CharacterStatsSO characterStats)
        {
            if (vigor != null) vigor.text = characterStats.virgor.ToString();
            if (mind != null) mind.text = characterStats.mind.ToString();
            if (endurance != null) endurance.text = characterStats.endurance.ToString();
            if (str != null) str.text = characterStats.strength.ToString();
            if (dex != null) dex.text = characterStats.dexterity.ToString();
            if (intell != null) intell.text = characterStats.intelligence.ToString();
            if (fai != null) fai.text = characterStats.faith.ToString();
            if (arc != null) arc.text = characterStats.arcane.ToString();

            if (hp != null) hp.text = characterStats.hp.ToString();
            if (fp != null) fp.text = characterStats.fp.ToString();
            if (stamina != null) stamina.text = characterStats.stamina.ToString();
            if (critChance != null) critChance.text = characterStats.critChance.ToString() + "%";
            if (critMult != null) critMult.text = characterStats.critMult.ToString();

            if (physDmg != null) physDmg.text = characterStats.attackDmg.ToString();
            if (magDmg != null) magDmg.text = characterStats.magicDmg.ToString();
            if (fireDmg != null) fireDmg.text = characterStats.fireDmg.ToString();
            if (lightningDmg != null) lightningDmg.text = characterStats.lightningDmg.ToString();
            if (waterDmg != null) waterDmg.text = characterStats.waterDmg.ToString();
            if (frostDmg != null) frostDmg.text = characterStats.frostDmg.ToString();
            if (holyDmg != null) holyDmg.text = characterStats.holyDmg.ToString();
            if (darkDmg != null) darkDmg.text = characterStats.darkDmg.ToString();
            if (poisonDmg != null) poisonDmg.text = characterStats.poisonDmg.ToString();

            if (physDef != null) physDef.text = characterStats.phyDef.ToString();
            if (magDef != null) magDef.text = characterStats.magicDef.ToString();
            if (fireDef != null) fireDef.text = characterStats.fireDef.ToString();
            if (lightningDef != null) lightningDef.text = characterStats.lightningDef.ToString();
            if (waterDef != null) waterDef.text = characterStats.waterDef.ToString();
            if (frostDef != null) waterDef.text = characterStats.waterDef.ToString();
            if (holyDef != null) holyDef.text = characterStats.holyDef.ToString();
            if (darkDef != null) darkDef.text = characterStats.darkDef.ToString();
            if (poisonDef != null) poisonDef.text = characterStats.poisonDef.ToString();

        }
    }
}