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
                skillNameText.text = skill.skillName;
            }

            if (manaCostText != null)
            {
                manaCostText.text = skill.manaCost.ToString();
            }

            if (typeText != null)
            {
                typeText.text = skill.elementType.ToString();

                if(skill.elementType == ElementType.None)
                {
                    typeText.text = "";
                }
            }



            skillButton.onClick.RemoveAllListeners();
            skillButton.onClick.AddListener(() => clickAction.Invoke(skillData));
        }
    }
}