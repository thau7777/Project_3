using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyRule.Event;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule.UI
{
    public class HUDView : BaseUIView
    {
        [SerializeField] private SigilStorageSO sigilStorage;
        [SerializeField] private SigilView[] sigilViews;
        [SerializeField] private ItemView[] itemViews;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.2f;

        private bool isShowing = true;

        private HUDPresenter hudPresenter;
        private CancellationTokenSource cts;

        protected override void Start()
        {
            base.Start();

            cts = new CancellationTokenSource();

            hudPresenter = new HUDPresenter(this, sigilViews, itemViews);

            inputReader.diceRollActions.onEsc += Show;

            SceneManager.LoadScene("CharacterScene", LoadSceneMode.Additive);

            LoadSigils();
        }

        private void OnDestroy()
        {
            cts.Cancel();
            hudPresenter.Clearup();
        }

        public override void Hide()
        {
            if (!isShowing) return;

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            isShowing = false;
        }

        public override void Show()
        {
            if (isShowing) return;

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();

            isShowing = true;
        }

        private void LoadSigils()
        {
            foreach (var sigilSO in sigilStorage.activeSigils)
            {
                if (sigilSO != null)
                {
                    EventBus<AddSigilEvent>.Raise(new AddSigilEvent(sigilSO));
                }
            }
        }
    }
}