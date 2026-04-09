using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

namespace MyRule
{
    public class BlackFade : Singleton<BlackFade>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.4f;
        private CancellationTokenSource cts;

        private async void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();
            try
            {
                await UniTask.Delay(400, cancellationToken: token);
                await FadeOut();
            }
            catch (Exception)
            {
            }
        }

        public async UniTask FadeIn()
        {
            CancelAndRenew();
            if (canvasGroup == null) return;
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.Linear);
            await UniTask.Delay((int)(fadeDuration * 1000), cancellationToken: cts.Token);
        }

        public async UniTask FadeIn(float duration)
        {
            CancelAndRenew();
            if (canvasGroup == null) return;
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, duration).SetEase(Ease.Linear);
            await UniTask.Delay((int)(duration * 1000), cancellationToken: cts.Token);
        }

        public async UniTask FadeOut()
        {
            CancelAndRenew();
            if (canvasGroup == null) return;
            canvasGroup.alpha = 1f;
            canvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.Linear);
            await UniTask.Delay((int)(fadeDuration * 1000), cancellationToken: cts.Token);
        }

        public async UniTask FadeOut(float duration)
        {
            CancelAndRenew();
            if (canvasGroup == null) return;
            canvasGroup.alpha = 1f;
            canvasGroup.DOFade(0f, duration).SetEase(Ease.Linear);
            await UniTask.Delay((int)(duration * 1000), cancellationToken: cts.Token);
        }

        public async void FadeThisFrame(float duration)
        {
            await FadeIn(duration);
            await FadeOut(duration);
        }

        private void CancelAndRenew()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}