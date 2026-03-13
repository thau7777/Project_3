using DG.Tweening;
using MyRule.Event;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule.UI
{
    public class HUDView : MonoBehaviour, IHUDView
    {
        [SerializeField] private SigilView[] sigilViews;
        [SerializeField] private ItemView[] itemViews;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.2f;

        private bool isShowing = true;

        private HUDPresenter hudPresenter;

        private void Start()
        {
            hudPresenter = new HUDPresenter(this, sigilViews, itemViews);

            SceneManager.LoadScene("CharacterScene", LoadSceneMode.Additive);
        }

        private void OnDestroy()
        {
            hudPresenter.Clearup();
        }

        public void HideHUD()
        {
            if (!isShowing) return;

            transform.DOLocalMoveY(-1200f, fadeDuration).SetEase(Ease.Linear);

            canvasGroup.DOFade(0, fadeDuration);

            isShowing = false;
        }

        public void ShowHUD()
        {
            if (isShowing) return;

            transform.DOLocalMoveY(-840f, fadeDuration).SetEase(Ease.Linear);
            canvasGroup?.DOFade(1, fadeDuration);

            isShowing = true;
        }
    }
}