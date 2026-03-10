namespace MyRule.CommandPattern
{
    public class SceneCommand : ICommand
    {
        private Loader.EScene targetScene;
        private Loader.EScene prevScene;

        public SceneCommand(Loader.EScene prevScene, Loader.EScene targetScene)
        {
            this.prevScene = prevScene;
            this.targetScene = targetScene;
        }

        public void Execute()
        {
            switch (targetScene)
            {
                case Loader.EScene.SpaceStationScene:
                    NewGame();
                    break;
                case Loader.EScene.SettingsScene:
                    OpenSetting();
                    break;
            }
        }

        public void Undo()
        {
            switch (targetScene)
            {
                case Loader.EScene.SpaceStationScene:
                    MainMenu();
                    break;
                case Loader.EScene.SettingsScene:
                    CloseSetting();
                    break;
            }
        }

        private async void NewGame()
        {
            await Loader.LoadSceneWithLoading(Loader.EScene.SpaceStationScene);
        }

        private async void OpenSetting()
        {
            await Loader.LoadSceneAdditive(Loader.EScene.SettingsScene);
        }

        private async void CloseSetting()
        {
            await Loader.UnloadSceneAdditive(Loader.EScene.SettingsScene);

            Loader.SetTargetScene(Loader.EScene.MainMenuScene);

            EventBus<MainMenuButtonSelectedEvent>.Raise(new MainMenuButtonSelectedEvent(UI.ButtonType.SystemButton));
        }

        private async void MainMenu()
        {
            await Loader.LoadSceneWithLoading(Loader.EScene.MainMenuScene);
        }
    }
}