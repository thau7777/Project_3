using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace MyRule
{
    public class BlackFade : Singleton<BlackFade>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.4f;

        private CancellationTokenSource cts;

        private async void Start()
        {
            await FadeOut();
        }

        public async UniTask FadeIn()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            canvasGroup.alpha = 0f;

            canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.Linear);

            await UniTask.Delay((int)(fadeDuration * 1000), cancellationToken: cts.Token);
        }

        public async UniTask FadeIn(float duration)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            canvasGroup.alpha = 0f;

            canvasGroup.DOFade(1f, duration).SetEase(Ease.Linear);

            await UniTask.Delay((int)(duration * 1000), cancellationToken: cts.Token);
        }

        public async UniTask FadeOut()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            canvasGroup.alpha = 1f;

            canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear);

            await UniTask.Delay((int)(fadeDuration * 1000), cancellationToken: cts.Token);
        }

        public async UniTask FadeOut(float duration)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            canvasGroup.alpha = 1f;

            canvasGroup.DOFade(0f, duration).SetEase(Ease.Linear);

            await UniTask.Delay((int)(duration * 1000), cancellationToken: cts.Token);
        }
    }
}