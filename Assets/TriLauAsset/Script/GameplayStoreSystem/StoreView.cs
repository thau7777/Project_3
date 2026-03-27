using Cysharp.Threading.Tasks;
using MyRule.CommandPattern;
using MyRule.UI;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace MyRule
{
    public class StoreView : BaseUIView, IStoreView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI runeTxt;
        [SerializeField] private float fadeDuration = 0.2f;

        [SerializeField] private Transform[] activePoints;
        [SerializeField] private Transform[] passivePoints;
        [SerializeField] private StoreItemView[] items;

        private CancellationTokenSource cts;

        private bool isShowing = false;

        private List<Card> cards = new();

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();

            cts = new CancellationTokenSource();
        }

        private void InitCommand()
        {
            ICommand command = new GameplayStoreCommand(this);
            CommandInvoker.ExecuteCommand(command);
        }

        public override async void Show()
        {
            if (isShowing) return;

            InitCommand();
            
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            DialogueManager.Instance.CanContinueDialogue = false;
            RTSCameraController.Instance.CanInteract = false;

            await UniTask.Delay(800);

            ShowingItem();

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();


            await UniTask.Delay((int)(fadeDuration * 1000));

            SpawnActiveSigil();
            
            SpawnPassiveSigil();

            isShowing = true;

            CardTracker.Instance.UnlockInteract(true);
        }

        public override async void Hide() 
        {
            if (!isShowing) return;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            await ReleaseSigilCard();

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();

            isShowing = false;

            CardTracker.Instance.UnlockInteract(false);

            RTSCameraController.Instance.CanInteract = true;

            DialogueManager.Instance.CanContinueDialogue = true;

            DialogueManager.Instance.ContinueStoryOrExitStory();
        }

        private void SpawnPassiveSigil()
        {
            List<SigilData> sigils = MatchManager.Instance.MatchData.SigilPool.GetPassiveSigils(passivePoints.Length);

            for (int i = 0; i < passivePoints.Length; i++)
            {
                SigilData sigilData = sigils[i];

                if (sigilData == null) continue;
                
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                if (sigilSO == null) continue;

                Card card = CardPoolManager.Instance.Spawn(sigilSO.id);
                card.transform.SetParent(passivePoints[i]);
                card.transform.position = passivePoints[i].transform.position;
                card.SetSigil(sigilData, sigilSO, CardGameplayType.StoreItem);
                cards.Add(card);
            }
        }

        private void SpawnActiveSigil()
        {
            List<SigilData> sigils = MatchManager.Instance.MatchData.SigilPool.GetActiveSigils(activePoints.Length);

            for (int i = 0; i < activePoints.Length; i++) 
            {
                SigilData sigilData = sigils[i];
                
                if (sigilData == null) continue;

                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                if (sigilSO == null) continue;

                Card card = CardPoolManager.Instance.Spawn(sigilSO.id);
                card.transform.SetParent(activePoints[i]);
                card.transform.position = activePoints[i].transform.position;
                card.SetSigil(sigilData, sigilSO, CardGameplayType.StoreItem);
                cards.Add(card);
            }
        }

        private void ShowingItem()
        { 
            for (int i = 0; i < items.Length; i++)
            {
                ItemSO itemSO = ItemManager.Instance.GetRandomItem();
                items[i].SetUp(itemSO);
            }
        }

        private UniTask ReleaseSigilCard()
        {
            foreach (Card card in cards)
            {
                card.ReleasePool();
            }

            cards.Clear();

            return UniTask.CompletedTask;
        }
    }
}