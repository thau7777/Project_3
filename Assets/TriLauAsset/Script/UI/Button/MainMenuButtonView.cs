using MyRule.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule.UI
{
    public class MainMenuButtonView : ButtonView
    {
        [SerializeField] private GameObject highlightButton;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Color selectColor = Color.white;
        [SerializeField] private Color deselectColor = Color.gray;

        private void Start()
        {
            buttonText.color = deselectColor;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            highlightButton.SetActive(true);
            buttonText.color = selectColor;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            highlightButton.SetActive(false);
            buttonText.color = deselectColor;
        }
    }
}