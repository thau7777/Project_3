using MyRule.UI;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace MyRule
{
    public class MazeGameplayRewardView : BaseUIView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform winningTxtObj;
        [SerializeField] private CanvasGroup rewardCanvasGroup;
        [SerializeField] private TextMeshProUGUI runeRewardTxt;
        [SerializeField] private Button continueBtn;
        [SerializeField] private Transform[] spawnPoint;
        [SerializeField] private float fadeDuration = 0.2f;

        private List<Card> gameObjects = new List<Card>();

        private CancellationTokenSource cts;

        private bool isShowing = false;

        private MazeGameplayReward reward;

        private EventBinding<SigilChosenEvent> evtBinhding;

        protected override void OnEnable()
        {
            base.OnEnable();
            evtBinhding = new EventBinding<SigilChosenEvent>(Hide);
            EventBus<SigilChosenEvent>.Register(evtBinhding);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<SigilChosenEvent>.Deregister(evtBinhding);
        }

        protected override void Start()
        {
            cts = new CancellationTokenSource();

            inputReader.diceRollActions.onEsc += Hide;
            continueBtn.onClick.AddListener(OnClick);

            reward = MazeGameplayRewardManager.Instance.GetReward();

            if (reward != null)
            {
                Show();
            }
        }

        public override async void Hide()
        {
            if (!isShowing) return;
            
            CardTracker.Instance.canInteract = false;

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

            if (reward != null) runeRewardTxt.text = reward.RuneAmount.ToString();

            VolumeController.Instance.AdjustUIVolumeWeight();

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();

            continueBtn.Select();

            RTSCameraController.Instance.CanInteract = false;

            CardTracker.Instance.canInteract = true;
            CardTracker.Instance.isReward = true;

            isShowing = true;
        }

        private void OnClick()
        {
            if (!isShowing) return;

            if (reward != null)
            {
                EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent(reward.RuneAmount));
            }
            else
            {
                EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent(10));
            }

            SpawnRewardSigil();
        }

        private async void SpawnRewardSigil()
        {
            winningTxtObj.DOLocalMoveY(800, 0.2f).SetEase(Ease.Linear);

            Transition.TransitionValue(
                setter: value => rewardCanvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            for (int i = 0; i < spawnPoint.Length; i++)
            {
                SigilSO sigilSO = MatchManager.Instance.GetRandomSigilInMatch();

                if (sigilSO == null) break;

                var cardObj = Instantiate(sigilSO.sigilPreb, spawnPoint[i]);
                Card card = cardObj.GetComponent<Card>();
                gameObjects.Add(card);
                card.SetSigil(sigilSO);
                card.transform.DOMoveY(-2000f, 0.2f).SetEase(Ease.Linear);
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
                card.transform.DOMoveY(-2200.4f, 0.2f).SetEase(Ease.Linear);
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