using MyRule.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule
{
    public class DialougeChoiceButtonView : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private int choiceIndex;
        [SerializeField] private TextMeshProUGUI choiceText;
        [SerializeField] private GameObject select;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool canSubmit = false;

        private void Start()
        {
            select.SetActive(false);
        }

        public void SetText(string text) => choiceText.text = text;

        public void SetIndex(in int index) => choiceIndex = index;

        public void OnDeselect(BaseEventData eventData)
        {
            select.SetActive(false);
        }

        public void SetCanSubmit(bool value)
        {
            canSubmit = value;
            if (canSubmit)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            select.SetActive(true);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!canSubmit) return;

            select.SetActive(false);
            EventBus<UpdateChoiceIndexEvent>.Raise(new UpdateChoiceIndexEvent(choiceIndex));
        }
    }
}