using MyRule.UI;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;

namespace MyRule
{
    public class MazeGameplayRewardView : BaseUIView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform[] spawnPoint;
        [SerializeField] private float fadeDuration = 0.2f;

        private List<Card> gameObjects = new List<Card>();

        private CancellationTokenSource cts;

        private bool isShowing = false;

        protected override void Start()
        {
            if (MazeGameplayRewardManager.Instance.HasRewards)
            {
                Show();
            }

            cts = new CancellationTokenSource();

            inputReader.diceRollActions.onEsc += Hide;
        }

        public override async void Hide()
        {
            if (!isShowing) return;

            Transition.TransitionValue(
                            setter: value => canvasGroup.alpha = value,
                            from: canvasGroup.alpha,
                            to: 0f,
                            duration: fadeDuration,
                            cts.Token).Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();

            await HideAllSigil();

            DesTroyCard();

            isShowing = false;

            RTSCameraController.Instance.CanInteract = true;
        }

        public override void Show()
        {
            if (isShowing) return;

            VolumeController.Instance.AdjustUIVolumeWeight();

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();

            SpawnRewardSigil();

            RTSCameraController.Instance.CanInteract = false;

            isShowing = true;
        }

        private async void SpawnRewardSigil()
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                SigilSO sigilSO = SigilCollectionInGame.Instance.GetRandomSigil();

                if (sigilSO == null) break;

                var cardObj = Instantiate(sigilSO.sigilPreb, spawnPoint[i]);
                Card card = cardObj.GetComponent<Card>();
                gameObjects.Add(card);
                card.SetSigil(sigilSO);
                card.transform.DOMoveY(-1999.4f, 0.2f).SetEase(Ease.OutElastic);
                await UniTask.Delay(200);
                card.IsShowing = true;
            }
        }

        private async UniTask HideAllSigil()
        {
            foreach (var card in gameObjects)
            {
                card.IsShowing = false;
                await UniTask.Delay(400);
                card.transform.DOMoveY(-2200.4f, 0.2f).SetEase(Ease.OutElastic);
            }
        }

        private void DesTroyCard()
        {
            foreach (var card in gameObjects)
            {
                Destroy(card);
            }
        }
    }
}