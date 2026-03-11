using UnityEngine;
using MyRule.UI;
using MyRule.CommandPattern;
using UnityEngine.UI;

namespace MyRule
{
    public static class Navigator
    {
        public static void OnSubmitPress(Button btnSubmit, ButtonType buttonType)
        {
            Loader.EScene currentScene = Loader.GetTargetScene();

            switch (buttonType)
            {
                case ButtonType.ContinueButton:
                    {
                        Loader.EScene eScene;

                        if (GameSystemManager.Instance.GameData.MatchData != null)
                        {
                            eScene = Loader.EScene.MazeScene;    
                        }
                        else
                        {
                            eScene = Loader.EScene.SpaceStationScene;
                        }

                        ICommand continueCommand = new SceneCommand(btnSubmit, currentScene, Loader.EScene.MazeScene);
                        CommandInvoker.ExecuteCommand(continueCommand);
                        break;
                    }
                case ButtonType.NewGameButton:
                    ICommand newGameCommand = new SceneCommand(btnSubmit, currentScene, Loader.EScene.SpaceStationScene);
                    CommandInvoker.ExecuteCommand(newGameCommand);
                    break;
                case ButtonType.SystemButton:
                    ICommand settingCommand = new SceneCommand(btnSubmit, currentScene, Loader.EScene.SettingsScene);
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