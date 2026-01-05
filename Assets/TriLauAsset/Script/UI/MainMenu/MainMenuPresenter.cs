using UnityEngine;

namespace MyRule.UI
{
    public class MainMenuPresenter
    {
        private IMainMenuPanel view;
        private IBaseUIView baseView;
        
        private EventBinding<SwitchPanelEvent> switchPanelEventBinding;

        public MainMenuPresenter(IMainMenuPanel view, IBaseUIView baseView)
        {
            this.view = view;
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

                UIStateMachine.Push(view.Type);
            }
        }

        public void CleanUp()
        {
            view = null;

            EventBus<SwitchPanelEvent>.Deregister(switchPanelEventBinding);
        }
    }
}