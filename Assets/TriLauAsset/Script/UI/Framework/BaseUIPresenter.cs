using UnityEngine;

namespace MyRule.UI
{
    public class BaseUIPresenter
    {
        IBaseUIView view;

        private EventBinding<SwitchPanelEvent> switchPanelEventBinding;

        public BaseUIPresenter(IBaseUIView panelView)
        {
            view = panelView;

            switchPanelEventBinding = new EventBinding<SwitchPanelEvent>(OnSwitchPanel);
            EventBus<SwitchPanelEvent>.Register(switchPanelEventBinding);
        }

        public void Cleanup()
        {
            view = null;

            EventBus<SwitchPanelEvent>.Deregister(switchPanelEventBinding);
        }

        private void OnSwitchPanel(SwitchPanelEvent e)
        {
            if (e.Type == view.Type)
                view.Show();
            else
                view.Hide();
        }
    }
}