using UnityEngine;

namespace MyRule.UI
{
    public class SettingsPresenter
    {
        ISettingsView view;
        IBaseUIView baseView;

        private EventBinding<SwitchPanelEvent> switchPanelEventBinding;

        public SettingsPresenter(ISettingsView view, IBaseUIView baseUIView)
        {
            this.view = view;
            this.baseView = baseUIView;

            switchPanelEventBinding = new EventBinding<SwitchPanelEvent>(OnSwitchPanel);
            EventBus<SwitchPanelEvent>.Register(switchPanelEventBinding);
        }

        private void OnSwitchPanel(SwitchPanelEvent evt)
        {
            if (evt.Type == baseView.Type) baseView.Show();
            else baseView.Hide();
        }

        public void Cleanup()
        {
            EventBus<SwitchPanelEvent>.Deregister(switchPanelEventBinding);
        }
    }
}