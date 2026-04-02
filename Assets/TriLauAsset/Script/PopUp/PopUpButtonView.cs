using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class PopUpButtonView : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private GameObject select;
        [SerializeField] private Button button;

        private void Start()
        {
            select.SetActive(false);
        }

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
            button.onClick.Invoke();
        }
    }
}