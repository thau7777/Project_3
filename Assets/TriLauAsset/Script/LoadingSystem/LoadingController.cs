using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class LoadingController : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.Delay(4000);
            Loader.LoaderCallback();
        }
    }
}