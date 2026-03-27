using DG.Tweening;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class LobbyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goldTxt;
        [SerializeField] private TextMeshProUGUI crystalTxt;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float transitionDuration = 0.4f;

        private LobbyPresenter lobbyPresenter;

        private void OnEnable()
        {
            lobbyPresenter = new LobbyPresenter(goldTxt, crystalTxt, transitionDuration, this);
        }

        private void OnDisable()
        {
            lobbyPresenter.Clearup();
        }

        public void Show()
        {
            transform.DOLocalMoveX(-1420, transitionDuration).SetEase(Ease.Linear);
            canvasGroup.DOFade(1f, transitionDuration);
        }

        public void Hide()
        {
            transform.DOLocalMoveX(-2880, transitionDuration).SetEase(Ease.Linear);
            canvasGroup.DOFade(0f, transitionDuration);
        }
    }
}