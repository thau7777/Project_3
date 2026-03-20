using Cysharp.Threading.Tasks;
using UnityEngine;
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
            TopDown,
            TurnBase,
            GreenlandScene,
            DesertScene,
            IcelandScene,
        }

        private static EScene targetScene = EScene.MainMenuScene;

        public static void SetTargetScene(EScene scene) => targetScene = scene;

        public static async UniTask LoadSceneDirect(EScene scene)
        {
            GameSystemManager.Instance.SaveData();

            await SceneManager.LoadSceneAsync(scene.ToString());
        }

        public static async UniTask LoadSceneWithLoading(EScene scene)
        {
            GameSystemManager.Instance.SaveData();

            targetScene = scene;

            await SceneManager.LoadSceneAsync(EScene.LoadingScene.ToString());
        }

        public static async UniTask LoadSceneAdditive(EScene scene)
        {
            GameSystemManager.Instance.SaveData();
            string sceneName = scene.ToString();

            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                    await UniTask.Yield();
            }
        }

        public static async UniTask UnloadSceneAdditive(EScene scene)
        {
            string sceneName = scene.ToString();
            Scene target = SceneManager.GetSceneByName(sceneName);

            if (target.isLoaded)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
                while (!asyncUnload.isDone)
                    await UniTask.Yield();
            }
            else
            {
                Debug.LogWarning($"Scene {sceneName} chưa được load, không thể unload.");
            }
        }

        public static EScene GetTargetScene()
        {
            return targetScene;
        }
    }
}