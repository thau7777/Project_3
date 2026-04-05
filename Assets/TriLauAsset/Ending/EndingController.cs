using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyRule
{
    public class EndingController : MonoBehaviour
    {
        [SerializeField] private DialougeTrigger dialougeTrigger;
        [SerializeField] private GameObject cam1;
        [SerializeField] private GameObject cam2;

        private CancellationTokenSource cts;

        private void Start()
        {
            cam1.SetActive(false);
            cam2.SetActive(false);
        }

        private async void EnterEnding()
        {
            await BlackFade.Instance.FadeOut(1f);

            cam1.SetActive(true);

            await UniTask.Delay(300);

            dialougeTrigger.Trigger();
        }
    }
}