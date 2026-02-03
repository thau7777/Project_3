using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Turnbase
{
    public class SkillTooltipUI : MonoBehaviour
    {
        [Header("UI Components")]
        public GameObject tooltipPanel;
        public TextMeshProUGUI skillNameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI statsText; // Damage, Mana, Type
        public TextMeshProUGUI detailsText; // Target, Element, Debuff

        public void Show(Skill skill)
        {
            if (skill == null) return;

            tooltipPanel.SetActive(true);

            skillNameText.text = skill.skillName;
            descriptionText.text = skill.description;

            // Hiển thị các thông số cơ bản
            statsText.text = $"<b>DMG:</b> {skill.damage} | <b>MP:</b> {skill.manaCost}\n" +
                             $"<b>Type:</b> {skill.skillType}";

            // Hiển thị các thông số chi tiết
            string debuffInfo = skill.debuffProperties.statToModify != DebuffType.None
                ? skill.debuffProperties.statToModify.ToString()
                : "None";

            detailsText.text = $"<b>Target:</b> {skill.targetType}\n" +
                               $"<b>Element:</b> {skill.elementType}\n" +
                               $"<b>Debuff:</b> {debuffInfo}";
        }

        public void Hide()
        {
            tooltipPanel.SetActive(false);
        }
    }
}