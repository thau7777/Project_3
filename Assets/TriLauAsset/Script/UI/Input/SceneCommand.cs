using UnityEngine.UI;

namespace MyRule.CommandPattern
{
    public class SceneCommand : ICommand
    {
        private Button btnSubmit;
        private Loader.EScene targetScene;
        private Loader.EScene prevScene;

        public SceneCommand(Button btnSubmit, Loader.EScene prevScene, Loader.EScene targetScene)
        {
            this.btnSubmit = btnSubmit;
            this.prevScene = prevScene;
            this.targetScene = targetScene;
        }

        public void Execute()
        {
            switch (targetScene)
            {
                case Loader.EScene.MazeScene:
                case Loader.EScene.SpaceStationScene:
                    LoadWithLoading();
                    break;
                case Loader.EScene.SettingsScene:
                    LoadAdditive();
                    break;
            }
        }

        public void Undo()
        {
            switch (targetScene)
            {
                case Loader.EScene.SpaceStationScene:
                    UndoLoadWithLoading();
                    break;
                case Loader.EScene.SettingsScene:
                    CloseSetting();
                    break;
            }
        }

        private async void LoadWithLoading()
        {
            await Loader.LoadSceneWithLoading(targetScene);
        }

        private async void LoadAdditive()
        {
            await Loader.LoadSceneAdditive(targetScene);
        }

        private async void CloseSetting()
        {
            await Loader.UnloadSceneAdditive(Loader.EScene.SettingsScene);

            Loader.SetTargetScene(prevScene);

            btnSubmit.Select();
        }

        private async void UndoLoadWithLoading()
        {
            await Loader.LoadSceneWithLoading(prevScene);
        }
    }
}