using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Turnbase
{
    public struct StatusEffectData
    {
        public string Name;
        public int TurnsRemaining;
        public string Detail;
        public bool IsBuff;
        public Sprite Icon;
    }


    public class StatusEffectEntry : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI detailText;
        public Image iconImage;

        public void Setup(string name, string detail)
        {
            if (nameText != null)
            {
                nameText.text = name;
            }
            if (detailText != null)
            {
                detailText.text = detail;
            }
        }

        public void UpdateVisuals(StatusEffectData data)
        {
            Color displayColor = data.IsBuff ? Color.green : Color.red;

            if (nameText != null)
            {
                nameText.color = displayColor;
            }
            if (detailText != null)
            {
                detailText.color = displayColor;
            }

            if (iconImage != null)
            {
                if (data.Icon != null)
                {
                    iconImage.sprite = data.Icon;
                    iconImage.enabled = true;
                    iconImage.color = Color.white;
                }
                else
                {
                    iconImage.enabled = false;
                }
            }
        }

    }
}