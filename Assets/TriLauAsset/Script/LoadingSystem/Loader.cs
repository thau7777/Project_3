using UnityEngine.SceneManagement;

namespace MyRule
{
    public static class Loader
    {
        public enum Scene
        {
            MainMenuScene,
            SpaceStationScene,
            BoardScene,
            LoadingScene,
            SettingsScene,
        }

        private static Scene targetScene;

        public static void Load(Scene scene)
        {
            targetScene = scene;

            SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
        }

        public static void LoadAdditive(Scene scene)
        {
            targetScene = scene;

            SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
        }

        public static void Unload(Scene scene)
        {
            SceneManager.UnloadSceneAsync(scene.ToString());
        }

        public static void SetActiveScene(Scene scene)
        {
            UnityEngine.SceneManagement.Scene target = SceneManager.GetSceneByName(scene.ToString());

            SceneManager.SetActiveScene(target);
        }

        public static void LoaderCallback()
        {
            SceneManager.LoadSceneAsync(Loader.targetScene.ToString());
        }
    }
}