using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MyRule
{
    public class BlackFade : Singleton<BlackFade>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.4f;

        private CancellationTokenSource cts = new CancellationTokenSource();

        private void Start()
        {
            FadeOut();
        }

        public void FadeIn()
        {
            canvasGroup.alpha = 0f;

            Transition.TransitionValue(
                    setter: value => canvasGroup.alpha = value,
                    from: canvasGroup.alpha,
                    to: 1f,
                    duration: fadeDuration,
                    cts.Token).Forget();
        }

        public void FadeOut()
        {
            canvasGroup.alpha = 1f;

            Transition.TransitionValue(
                    setter: value => canvasGroup.alpha = value,
                    from: canvasGroup.alpha,
                    to: 0f,
                    duration: fadeDuration,
                    cts.Token).Forget();
        }
    }
}