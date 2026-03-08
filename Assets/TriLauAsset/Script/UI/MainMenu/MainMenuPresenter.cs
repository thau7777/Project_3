using UnityEngine;

namespace MyRule.UI
{
    public class MainMenuPresenter
    {
        private IBaseUIView baseView;
        
        private EventBinding<SwitchPanelEvent> switchPanelEventBinding;

        public MainMenuPresenter(IBaseUIView baseView)
        {
            this.baseView = baseView;

            switchPanelEventBinding = new EventBinding<SwitchPanelEvent>(OnSwitchPanel);
            EventBus<SwitchPanelEvent>.Register(switchPanelEventBinding);
            this.baseView = baseView;
        }

        private void OnSwitchPanel(SwitchPanelEvent evt)
        {
            if (evt.Type == PanelType.MainMenu)
            {
                baseView.Show();
            }
            else
            {
                baseView.Hide();

                UIStateMachine.Push(baseView.Type);
            }
        }

        public void CleanUp()
        {
            baseView = null;

            EventBus<SwitchPanelEvent>.Deregister(switchPanelEventBinding);
        }
    }
}