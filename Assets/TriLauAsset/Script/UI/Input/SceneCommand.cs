namespace MyRule.CommandPattern
{
    public class SceneCommand : ICommand
    {
        private Loader.EScene currentScene;

        public SceneCommand(Loader.EScene currentScene)
        {
            this.currentScene = currentScene;
        }

        public void Execute()
        {
            switch (currentScene)
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
            switch (currentScene)
            {
                case Loader.EScene.SpaceStationScene:
                    MainMenu();
                    break;
                case Loader.EScene.SettingsScene:
                    CloseSetting();
                    break;
            }
        }

        private void NewGame()
        {
            Loader.Load(Loader.EScene.SpaceStationScene, Loader.ELoadMode.WithLoadingScreen);
        }

        private void OpenSetting()
        {
            Loader.LoadAdditive(Loader.EScene.SettingsScene);
        }

        private void CloseSetting()
        {
            Loader.Unload(Loader.EScene.SettingsScene);

            Loader.SetActiveScene(Loader.EScene.MainMenuScene);

            EventBus<MainMenuButtonSelectedEvent>.Raise(new MainMenuButtonSelectedEvent(UI.ButtonType.SystemButton));
        }

        private void MainMenu()
        {
            Loader.Load(Loader.EScene.MainMenuScene);
        }
    }
}