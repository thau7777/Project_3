using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class SettingDescriptionView : Singleton<SettingDescriptionView>
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image img;

        [SerializeField] private GameObject container;

        public void SetInfo(string name, string description, Sprite sprite = null)
        {
            nameText.text = name;
            descriptionText.text = description;
            
            if (sprite != null)
            {
                img.sprite = sprite;
            }
        }

        public void SetActive(bool isActive)
        {
            container.SetActive(isActive);
        }
    }
}