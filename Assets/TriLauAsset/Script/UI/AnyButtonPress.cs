using MyRule.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class AnyButtonPress : MonoBehaviour
    {
        [SerializeField] private Button anyButton;

        private EventBinding<AnyButtonPressEvent> anyEventBinding;

        private void OnEnable()
        {
            anyEventBinding = new EventBinding<AnyButtonPressEvent>(Press);
            EventBus<AnyButtonPressEvent>.Register(anyEventBinding);
        }

        private void OnDisable()
        {
            EventBus<AnyButtonPressEvent>.Deregister(anyEventBinding);
        }

        private void Press()
        {
            anyButton.onClick.Invoke();
        }
    }
}