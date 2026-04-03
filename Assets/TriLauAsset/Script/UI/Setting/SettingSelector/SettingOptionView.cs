using MyRule.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule.UI
{
    public class SettingOptionView : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [Header("Info")]
        [SerializeField] protected string settingName = "Text Language";
        [TextArea(3, 10)][SerializeField] protected string settingDesc = "Set the language for in-game text.";

        [Header("UI Elements")]
        [SerializeField] protected GameObject selectedBG;
        [SerializeField] protected GameObject deselectedBG;

        [Header("Data")]
        [SerializeField] protected string settingKey = "TextLanguage";

        protected virtual void Start()
        {

        }

        public void SetFirstSelect()
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        private void SetSelected(bool isSelected)
        {
            if (selectedBG != null)
                selectedBG.SetActive(isSelected);
            if (deselectedBG != null)
                deselectedBG.SetActive(!isSelected);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetSelected(true);

            SettingDescriptionView.Instance.SetInfo(settingName, settingDesc);

            AudioManager.Instance.PlaySound("UIButtonClick");
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetSelected(false);
        }
    }
}