using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Turnbase
{
    public class CharacterStatUI : MonoBehaviour
    {
        [Header("UI Components")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI level;
        public Image Avatar;
        public TextMeshProUGUI maxHP;
        public TextMeshProUGUI currentHP;
        public TextMeshProUGUI maxMP;
        public TextMeshProUGUI currentMP;
        public TextMeshProUGUI maxShield;
        public TextMeshProUGUI currentShield;
        public TextMeshProUGUI physicalAttack;
        public TextMeshProUGUI physicalDefense;
        public TextMeshProUGUI magicAttack;
        public TextMeshProUGUI magicDefense;
        public TextMeshProUGUI crit;
        public TextMeshProUGUI critDamage;
        public TextMeshProUGUI agility;

        public GameObject statPanel;

        private Character currentCharacter;

        void Start()
        {
            HideStats();
        }

        private void OnEnable()
        {
            if (currentCharacter != null)
            {
                UpdateStatsUI(currentCharacter);
            }
        }

        public void ShowStats(Character character)
        {
            currentCharacter = character;
            characterName.text = character.info.name;
            level.text = "Lv. " + character.info.level.ToString();
            Avatar.sprite = character.stats.Avatar;
            maxHP.text = character.stats.maxHP.ToString();
            currentHP.text = character.stats.currentHP.ToString();
            maxMP.text = character.stats.maxMP.ToString();
            currentMP.text = character.stats.currentMP.ToString();
            maxShield.text = character.stats.maxShield.ToString();
            currentShield.text = character.stats.currentShield.ToString();
            physicalAttack.text = character.stats.physicalAttack.ToString();
            physicalDefense.text = character.stats.physicalDefense.ToString();
            magicAttack.text = character.stats.magicAttack.ToString();
            magicDefense.text = character.stats.magicDefense.ToString();
            crit.text = character.stats.crit.ToString() + "%";
            critDamage.text = character.stats.critDamage.ToString() + "%";
            agility.text = character.stats.agility.ToString();
            statPanel.SetActive(true);
        }

        public void UpdateStatsUI(Character character)
        {
            if (character == null || character.stats == null || character.info == null)
            {
                HideStats();
                return;
            }

            characterName.text = character.info.name;
            level.text = "Lv. " + character.info.level.ToString();
            Avatar.sprite = character.stats.Avatar;

            maxHP.text = character.stats.maxHP.ToString();
            currentHP.text = character.stats.currentHP.ToString();
            maxMP.text = character.stats.maxMP.ToString();
            currentMP.text = character.stats.currentMP.ToString();

            if (character.stats.maxShield > 0)
            {
                maxShield.text = character.stats.maxShield.ToString();
                currentShield.text = character.stats.currentShield.ToString();
            }

            physicalAttack.text = character.stats.physicalAttack.ToString();
            physicalDefense.text = character.stats.physicalDefense.ToString();
            magicAttack.text = character.stats.magicAttack.ToString();
            magicDefense.text = character.stats.magicDefense.ToString();

            crit.text = character.stats.crit.ToString() + "%";
            critDamage.text = character.stats.critDamage.ToString() + "%";
            agility.text = character.stats.agility.ToString();
        }

        public void HideStats()
        {
            statPanel.SetActive(false);
            currentCharacter = null;
        }








    }                 

}
