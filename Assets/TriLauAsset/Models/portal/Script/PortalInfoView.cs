using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MyRule.UI
{
    public class PortalInfoView : BaseUIView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;

        private CancellationTokenSource cts;

        public override void Hide()
        {
            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();
        }

        public override void Show()
        {
            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();
        }
    }
}