namespace MyRule.CommandPattern
{
    public class SceneCommand : ICommand
    {
        private Loader.Scene currentScene;

        public SceneCommand(Loader.Scene currentScene)
        {
            this.currentScene = currentScene;
        }

        public void Execute()
        {
            switch (currentScene)
            {
                case Loader.Scene.SpaceStationScene:
                    NewGame();
                    break;
                case Loader.Scene.SettingsScene:
                    OpenSetting();
                    break;
            }
        }

        public void Undo()
        {
            switch (currentScene)
            {
                case Loader.Scene.SpaceStationScene:
                    MainMenu();
                    break;
                case Loader.Scene.SettingsScene:
                    CloseSetting();
                    break;
            }
        }

        private void NewGame()
        {
            Loader.Load(Loader.Scene.SpaceStationScene);
        }

        private void OpenSetting()
        {
            Loader.LoadAdditive(Loader.Scene.SettingsScene);
        }

        private void CloseSetting()
        {
            Loader.Unload(Loader.Scene.SettingsScene);

            Loader.SetActiveScene(Loader.Scene.MainMenuScene);

            EventBus<MainMenuButtonSelectedEvent>.Raise(new MainMenuButtonSelectedEvent(UI.ButtonType.SettingsButton));
        }

        private void MainMenu()
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        }
    }
}