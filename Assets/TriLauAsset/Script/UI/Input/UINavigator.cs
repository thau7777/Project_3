using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MyRule.UI
{
    public class UINavigator : MonoBehaviour
    {
        public ButtonView _currentButton;

        private EventBinding<SelectButtonEvent> selectButtonEventBinding;
        private EventBinding<SubmitPressEvent> submitPressEventBinding;

        private void OnEnable()
        {
            selectButtonEventBinding = new EventBinding<SelectButtonEvent>(OnSelectEvent);
            EventBus<SelectButtonEvent>.Register(selectButtonEventBinding);

            submitPressEventBinding = new EventBinding<SubmitPressEvent>(OnSubmitPress);
            EventBus<SubmitPressEvent>.Register(submitPressEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SelectButtonEvent>.Deregister(selectButtonEventBinding);
            selectButtonEventBinding = null;

            EventBus<SubmitPressEvent>.Deregister(submitPressEventBinding);
            submitPressEventBinding = null;
        }

        public void SetCurrentButton(ButtonView button)
        {
            _currentButton = button;
        }

        public async void OnSubmitPress(SubmitPressEvent submitPressEvent)
        {
            if (_currentButton == null) return;

            switch (_currentButton.Type)
            {
                case ButtonType.NewGameButton:
                    //EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(UIPanelType.Settings));
                    break;
                case ButtonType.LoadGameButton:
                    {
                        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.SaveFiles));

                        await LoadGame();

                        break;
                    }
                case ButtonType.SettingsButton:
                    {
                        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Settings));
                        EventBus<SwitchCamEvent>.Raise(new SwitchCamEvent(2));
                        break;
                    }
                case ButtonType.CreditsButton:
                    {
                        EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Credits));
                        EventBus<SwitchCamEvent>.Raise(new SwitchCamEvent(2));
                        break;
                    }
                case ButtonType.QuitButton:
                    Application.Quit();
                    break;
            }
        }

        private void OnSelectEvent(SelectButtonEvent selectButtonEvent)
        {
            _currentButton = selectButtonEvent.Button;
        }

        private UniTask LoadGame()
        {
            Loader.Load(Loader.Scene.SpaceStationScene);
            return UniTask.CompletedTask;
        }
    }
}