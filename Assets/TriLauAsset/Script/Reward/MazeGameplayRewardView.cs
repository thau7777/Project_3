using MyRule.UI;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
        [SerializeField] private ParticleSystem runeParticles;

        private List<Card> cards = new List<Card>();

        private CancellationTokenSource cts;

        private bool isShowing = false;

        private int locking = 1;

        private MazeGameplayReward reward;

        private EventBinding<SigilChosenEvent> evtBinhding;
        private EventBinding<CardDetailLockEvent> evtCardLocking;

        protected override void OnEnable()
        {
            base.OnEnable();
            evtBinhding = new EventBinding<SigilChosenEvent>(Hide);
            EventBus<SigilChosenEvent>.Register(evtBinhding);

            evtCardLocking = new EventBinding<CardDetailLockEvent>(LockReward);
            EventBus<CardDetailLockEvent>.Register(evtCardLocking);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventBus<SigilChosenEvent>.Deregister(evtBinhding);
            EventBus<CardDetailLockEvent>.Deregister(evtCardLocking);
        }

        protected async override void Start()
        {
            cts = new CancellationTokenSource();

            inputReader.diceRollActions.onEsc += HandleEscBtn;
            continueBtn.onClick.AddListener(OnClick);

            //EMatchResult matchResult = MatchManager.Instance.MatchData.Result;

            //if (matchResult == EMatchResult.Win || matchResult == EMatchResult.Lose) return;

            await UniTask.WaitUntil(() => MazeGameplayRewardManager.Instance != null);

            reward = MazeGameplayRewardManager.Instance.GetReward();

            if (reward != null)
            {
                locking--;
                Show();
            }
        }

        private void LockReward(CardDetailLockEvent evt)
        {
            locking++;
        }

        private void HandleEscBtn()
        {
            if (locking > 0) return;
            Hide();
        }

        public override async void Hide()
        {
            if (!isShowing) return;
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            CardTracker.Instance.UnlockInteract(false);

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();

            await HideAllSigil();

            isShowing = false;

            RTSCameraController.Instance.UnlockInteract();
            MapPlayerTracker.Instance.UnlockMapTracker();

            if (CombatManager.Instance.CombatData.CombatType == CombatType.BossFigihting)
            {
                BlackFade.Instance.FadeIn();
                await UniTask.Delay(1000);
                await MatchManager.Instance.MatchData.MoveToNextMap();
            }

            locking = 1;
        }

        public override void Show()
        {
            if (isShowing || locking > 0) return;

            if (reward != null) runeRewardTxt.text = reward.RuneAmount.ToString();

            VolumeController.Instance.AdjustUIVolumeWeight();

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            continueBtn.Select();

            RTSCameraController.Instance.LockInteract();

            CardTracker.Instance.UnlockInteract(true);

            MapPlayerTracker.Instance.LockMapTracker();

            isShowing = true;
        }

        private async void OnClick()
        {
            if (!isShowing) return;

            runeParticles.Play();
            
            await UniTask.Delay(300);

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
            await UniTask.Delay(800);

            winningTxtObj.DOLocalMoveY(800, 0.2f).SetEase(Ease.Linear);

            Transition.TransitionValue(
                setter: value => rewardCanvasGroup.alpha = value,
                from: rewardCanvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            List<SigilData> sigils = MatchManager.Instance.MatchData.SigilPool.GetMixedSigils(spawnPoint.Length);

            for (int i = 0; i < spawnPoint.Length; i++)
            {
                SigilData sigilData = sigils[i];

                if (sigilData == null) break;

                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);
                
                if (sigilSO == null) continue;

                Card card = CardPoolManager.Instance.Spawn(sigilSO.id);
                card.SetSigil(sigilData, sigilSO, CardGameplayType.Reward);
                card.transform.SetParent(spawnPoint[i]);
                card.transform.position = spawnPoint[i].position;
                card.transform.DOMoveY(-2000f, 0.2f).SetEase(Ease.Linear);
                cards.Add(card);
                await UniTask.Delay(200);
            }
        }

        private async UniTask HideAllSigil()
        {
            foreach (var card in cards)
            {
                card.ReleasePool();
                await UniTask.Delay(200);
                card.transform.DOMoveY(-2200.4f, 0.2f).SetEase(Ease.Linear);
            }

            cards.Clear();
        }
    }
}