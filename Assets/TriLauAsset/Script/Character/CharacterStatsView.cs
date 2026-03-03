using TMPro;
using UnityEngine;

namespace MyRule
{
    public class CharacterStatsView : Singleton<CharacterStatsView>
    {
        [SerializeField] private TextMeshProUGUI virgo;
        [SerializeField] private TextMeshProUGUI mind;
        [SerializeField] private TextMeshProUGUI endurance;
        [SerializeField] private TextMeshProUGUI str;
        [SerializeField] private TextMeshProUGUI dex;
        [SerializeField] private TextMeshProUGUI intell;
        [SerializeField] private TextMeshProUGUI fai;
        [SerializeField] private TextMeshProUGUI arc;

        public void UpdateStats(CharacterStatsSO characterStats)
        {
            virgo.text = characterStats.virgor.ToString();
            mind.text = characterStats.mind.ToString();
            endurance.text = characterStats.endurance.ToString();
            str.text = characterStats.strength.ToString();
            dex.text = characterStats.dexterity.ToString();
            intell.text = characterStats.intelligence.ToString();
            fai.text = characterStats.faith.ToString();
            arc.text = characterStats.arcane.ToString();
        }
    }
}