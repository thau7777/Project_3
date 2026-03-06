using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace MyRule.UI
{
    public class HUDView : BaseUIView
    {
        [SerializeField] private SigilView[] sigilViews;
        [SerializeField] private ItemView[] itemViews;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.2f;

        private HUDPresenter hudPresenter;
        private CancellationTokenSource cts;

        protected override void Start()
        {
            base.Start();

            cts = new CancellationTokenSource();

            hudPresenter = new HUDPresenter(this, sigilViews, itemViews);
        }

        public override void Hide()
        {
            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();
        }

        public override void Show()
        {
            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();
        }
    }
}