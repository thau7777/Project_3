using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class LoadingController : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.Delay(1000);
            Loader.LoaderCallback();
        }
    }
}