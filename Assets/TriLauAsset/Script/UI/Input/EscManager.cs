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
            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.MainMenu));
            //var currentPanel = UIStateMachine.Current;
            //switch (currentPanel)
            //{
            //    case PanelType.Settings:
            //        UIStateMachine.Pop();
            //        Debug.Log("Popped Settings Panel");
            //        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(currentPanel));
            //        break;
            //    case PanelType.SaveFiles:
            //        UIStateMachine.Pop();
            //        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(currentPanel));
            //        break;
            //    case PanelType.Credits:
            //        break;
            //    case PanelType.PauseMenu:
            //        break;
            //    case PanelType.Inventory:
            //        break;
            //    case PanelType.CharacterStats:
            //        break;
            //    default:
            //        return; // No actionButton for other panels
            //}
        }
    }
}