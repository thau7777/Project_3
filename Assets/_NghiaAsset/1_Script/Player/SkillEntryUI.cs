using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Turnbase
{
    public class SkillEntryUI : MonoBehaviour
    {
        public Button skillButton;
        public Image skillIcon;
        public TextMeshProUGUI skillNameText;
        public TextMeshProUGUI manaCostText;
        public TextMeshProUGUI typeText;

        [Header("Passive Settings")]
        public float passiveAlpha = 1; 

        public Skill skillData { get; private set; }

        public void Setup(Skill skill, Action<Skill> clickAction)
        {
            skillData = skill;

            if (skillIcon != null && skill.icon != null)
            {
                skillIcon.sprite = skill.icon;
                skillIcon.color = Color.white;
            }

            if (skillNameText != null)
            {
                skillNameText.text = skill.skillType == SkillType.XPassive
                    ? skill.skillName + " (P)"
                    : skill.skillName;
            }

            if (manaCostText != null)
            {
                manaCostText.text = skill.manaCost.ToString();
            }

            if (typeText != null)
            {
                typeText.text = skill.elementType == ElementType.None ? "" : skill.elementType.ToString();
            }

            skillButton.onClick.RemoveAllListeners();

            if (skill.skillType == SkillType.XPassive)
            {
                skillButton.interactable = false;

                if (GetComponent<CanvasGroup>() != null)
                {
                    GetComponent<CanvasGroup>().alpha = passiveAlpha;
                }
                else
                {
                    if (skillIcon != null) skillIcon.color = new Color(1, 1, 1, passiveAlpha);
                }
            }
            else
            {
                skillButton.interactable = true;
                if (GetComponent<CanvasGroup>() != null) GetComponent<CanvasGroup>().alpha = 1f;

                skillButton.onClick.AddListener(() => clickAction.Invoke(skillData));
            }
        }

        public void SelectThisSkill()
        {
            if (skillButton.interactable)
            {
                skillButton.onClick.Invoke();
            }
        }
    }
}