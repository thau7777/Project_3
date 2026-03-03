using UnityEngine.SceneManagement;

namespace MyRule
{
    public static class Loader
    {
        public enum EScene
        {
            MainMenuScene,
            SpaceStationScene,
            MazeScene,
            LoadingScene,
            SettingsScene,
        }

        public enum ELoadMode
        {
            Normal,
            WithLoadingScreen,
        }

        private static EScene targetScene;
        private static LoadSceneMode targetLoadMode;

        public static void Load(EScene scene, ELoadMode eLoadMode = ELoadMode.Normal)
        {
            targetScene = scene;
            targetLoadMode = LoadSceneMode.Single;

            if (eLoadMode == ELoadMode.Normal)
            {
                SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
            }
            else if (eLoadMode == ELoadMode.WithLoadingScreen)
            {
                SceneManager.LoadScene(EScene.LoadingScene.ToString(), LoadSceneMode.Single);
            }
        }

        public static void LoadAdditive(EScene scene, ELoadMode eLoadMode = ELoadMode.Normal)
        {
            targetScene = scene;
            targetLoadMode = LoadSceneMode.Additive;

            if (eLoadMode == ELoadMode.Normal)
            {
                SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
            }
            else if (eLoadMode == ELoadMode.WithLoadingScreen)
            {
                SceneManager.LoadScene(EScene.LoadingScene.ToString(), LoadSceneMode.Single);
            }
        }

        public static void Unload(EScene scene)
        {
            SceneManager.UnloadSceneAsync(scene.ToString());
        }

        public static void SetActiveScene(EScene scene)
        {
            UnityEngine.SceneManagement.Scene target = SceneManager.GetSceneByName(scene.ToString());

            SceneManager.SetActiveScene(target);
        }

        public static void LoaderCallback()
        {
            SceneManager.LoadSceneAsync(Loader.targetScene.ToString(), targetLoadMode);
        }
    }
}