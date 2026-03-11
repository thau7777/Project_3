using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class LoadingController : MonoBehaviour
    {
        private async void Start()
        {
            Loader.EScene scene = Loader.GetTargetScene();

            await LoadTargetScene(scene.ToString());
        }

        private async UniTask LoadTargetScene(string scene)
        {
            var op = SceneManager.LoadSceneAsync(scene);

            while (!op.isDone)
            {
                await UniTask.Yield();
            }

            await WaitGameDataLoaded();
        }

        private async UniTask WaitGameDataLoaded()
        {
            await UniTask.WaitUntil(() =>
                GameSystemManager.Instance.IsLoadCompleted
            );

            await UniTask.Yield();
        }
    }
}