using UnityEngine;
using MyRule.UI;
using MyRule.CommandPattern;

namespace MyRule
{
    public static class Navigator
    {
        public static void OnSubmitPress(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.NewGameButton:
                    ICommand newGameCommand = new SceneCommand(Loader.Scene.SpaceStationScene);
                    CommandInvoker.ExecuteCommand(newGameCommand);
                    break;
                case ButtonType.SettingsButton:
                    ICommand settingCommand = new SceneCommand(Loader.Scene.SettingsScene);
                    CommandInvoker.ExecuteCommand(settingCommand);
                    break;
                case ButtonType.QuitButton:
                    Application.Quit();
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