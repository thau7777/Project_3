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

        public void OnSelect(BaseEventData eventData)
        {
            select.SetActive(true);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            EventBus<UpdateChoiceIndexEvent>.Raise(new UpdateChoiceIndexEvent(choiceIndex));
        }
    }
}