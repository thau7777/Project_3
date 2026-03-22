using DG.Tweening;
using MyRule.Event;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule.UI
{
    public class HUDView : MonoBehaviour, IHUDView
    {
        [Header("Input")]
        [SerializeField] private InputReader inputReader; 
        
        [Header("Sigil Storage")]
        [SerializeField] private SigilSlotView[] activeSigilViews;
        [SerializeField] private SigilSlotView[] passiveSigilView;
        [SerializeField] private float showStorageDuration = 0.2f;
        [SerializeField] private ItemView[] itemViews;
        [SerializeField] private GameObject storageContainer;
        [SerializeField] private CanvasGroup passiveCanvasGroup;

        [Header("Panel")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.2f;

        private bool isShowing = true;

        private HUDPresenter hudPresenter;

        private void Start()
        {
            hudPresenter = new HUDPresenter(this, activeSigilViews, passiveSigilView, itemViews, inputReader);

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

        public void ShowStorage()
        {
            storageContainer.transform.DOLocalMoveY(250f, showStorageDuration).SetEase(Ease.Linear);
        }

        public void HideStorage()
        {
            storageContainer.transform.DOLocalMoveY(10f, showStorageDuration).SetEase(Ease.Linear);
        }

        public void ShowPassiveSigilStorage()
        {
            passiveCanvasGroup.DOFade(1, showStorageDuration);
            transform.DOLocalMoveY(-360, showStorageDuration).SetEase(Ease.Linear);
        }

        public void HidePassiveSigilStorage()
        {
            passiveCanvasGroup.DOFade(0, showStorageDuration);
            transform.DOLocalMoveY(-840, showStorageDuration).SetEase(Ease.Linear);
        }
    }
}