using UnityEngine;

namespace MyRule.UI
{
    public class MainMenuPresenter
    {
        private IMainMenuPanel view;
        private IBaseUIView baseView;

        private EventBinding<AnyButtonPressEvent> pressAnytButtonEventBinding;
        private EventBinding<SubmitPressEvent> submitPressEventBinding;
        private EventBinding<SelectButtonEvent> selectButtonEventBinding;
        private EventBinding<SwitchPanelEvent> switchPanelEventBinding;

        public MainMenuPresenter(IMainMenuPanel view, IBaseUIView baseView)
        {
            this.view = view;
            this.baseView = baseView;

            pressAnytButtonEventBinding = new EventBinding<AnyButtonPressEvent>(OnShowMainMenuButtons);
            EventBus<AnyButtonPressEvent>.Register(pressAnytButtonEventBinding);

            submitPressEventBinding = new EventBinding<SubmitPressEvent>(HandleSubmitEvent);
            EventBus<SubmitPressEvent>.Register(submitPressEventBinding);

            selectButtonEventBinding = new EventBinding<SelectButtonEvent>(HandleSelectEvent);
            EventBus<SelectButtonEvent>.Register(selectButtonEventBinding);

            switchPanelEventBinding = new EventBinding<SwitchPanelEvent>(OnSwitchPanel);
            EventBus<SwitchPanelEvent>.Register(switchPanelEventBinding);
            this.baseView = baseView;
        }

        private void OnShowMainMenuButtons()
        {
            view.ShowMenuButtons();
            view.HideAnyButton();
        }

        private void HandleSubmitEvent(SubmitPressEvent submitPressEvent)
        {
            if (view.CurrentButton == null) return;

            view.CurrentButton.Submit();

            switch (view.CurrentButton.Type)
            {
                case ButtonType.NewGameButton:
                    //EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(UIPanelType.Settings));
                    break;
                case ButtonType.LoadGameButton:
                    {
                        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.SaveFiles));
                        UIStateMachine.Push(PanelType.SaveFiles);
                        break;
                    }
                case ButtonType.SettingsButton:
                    {
                        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Settings));
                        UIStateMachine.Push(PanelType.Settings);
                        break;
                    }
                case ButtonType.CreditsButton:
                    {
                        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Credits));
                        UIStateMachine.Push(PanelType.Credits);
                        break;
                    }
                case ButtonType.QuitButton:
                    Application.Quit();
                    break;
            }
        }

        private void HandleSelectEvent(SelectButtonEvent selectButtonEvent)
        {
            if (view.CurrentButton != null)
            {
                view.CurrentButton.Deselect();
            }

            view.CurrentButton = selectButtonEvent.Button;

            view.CurrentButton.Select();
        }

        private void OnSwitchPanel(SwitchPanelEvent evt)
        {
            if (evt.Type == PanelType.MainMenu) baseView.Show();
            else baseView.Hide();
        }

        public void CleanUp()
        {
            view = null;

            EventBus<AnyButtonPressEvent>.Deregister(pressAnytButtonEventBinding);
            EventBus<SubmitPressEvent>.Deregister(submitPressEventBinding);
            EventBus<SelectButtonEvent>.Deregister(selectButtonEventBinding);
            EventBus<SwitchPanelEvent>.Deregister(switchPanelEventBinding);
        }
    }
}