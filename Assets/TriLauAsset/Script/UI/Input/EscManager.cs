using MyRule.UI;
using UnityEngine;

namespace MyRule
{
    public class EscManager : MonoBehaviour
    {
        private EventBinding<CancelPressEvent> canceclPressEventBinding;
        
        private void OnEnable()
        {
            canceclPressEventBinding = new EventBinding<CancelPressEvent>(OnCancelPress);
            EventBus<CancelPressEvent>.Register(canceclPressEventBinding);
        }

        private void OnDisable()
        {
            EventBus<CancelPressEvent>.Deregister(canceclPressEventBinding);
        }

        private void OnCancelPress(CancelPressEvent evt)
        {
            PanelType panelType = UIStateMachine.Pop();

            Debug.Log("EscManager OnCancelPress Pop panelType: " + panelType);
            if (panelType == PanelType.None)
                return;

            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(panelType));

            EventBus<SwitchCamEvent>.Raise(new SwitchCamEvent(1));
        }
    }
}