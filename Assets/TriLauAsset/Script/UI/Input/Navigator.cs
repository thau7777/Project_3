using UnityEngine;
using MyRule.UI;
using MyRule.CommandPattern;

namespace MyRule
{
    public static class Navigator
    {
        public static void OnSubmitPress(ButtonType buttonType)
        {
            Loader.EScene currentScene = Loader.GetTargetScene();

            switch (buttonType)
            {
                case ButtonType.NewGameButton:
                    ICommand newGameCommand = new SceneCommand(currentScene, Loader.EScene.SpaceStationScene);
                    CommandInvoker.ExecuteCommand(newGameCommand);
                    break;
                case ButtonType.SystemButton:
                    ICommand settingCommand = new SceneCommand(currentScene, Loader.EScene.SettingsScene);
                    CommandInvoker.ExecuteCommand(settingCommand);
                    break;
                case ButtonType.QuitButton:
                    Application.Quit();
                    break;
                case ButtonType.ProfileButton:
                    EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Profile));
                    break;
                case ButtonType.DiaryButton:
                    // Implement diary button action here
                    break;
                case ButtonType.ShopButton:
                    // Implement shop button action here
                    break;
                default:
                    return;
            }
        }

        public static void OnCancelPress()
        {
            CommandInvoker.UndoCommand();
        }
    }
}