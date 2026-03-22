using Cysharp.Threading.Tasks;
using MyRule.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule
{
    public class StoreView : BaseUIView, IStoreView
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI runeTxt;
        [SerializeField] private float fadeDuration = 0.2f;

        [SerializeField] private Transform[] spawnPoint;
        [SerializeField] private GameObject passiveCardsCons;
        [SerializeField] private Card[] passiveCards;
        [SerializeField] private StoreItemView[] items;

        private CancellationTokenSource cts;

        private bool isShowing = false;
        private List<Card> gameObjects = new List<Card>();

        protected override void Start()
        {
            base.Start();

            cts = new CancellationTokenSource();

            inputReader.diceRollActions.onEsc += Hide;
        }

        public override async void Show()
        {
            if (isShowing) return;
            
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
            

            await UniTask.Delay((int)fadeDuration * 1000);

            SpawnActiveSigil();
            
            passiveCardsCons.SetActive(true);

            await ShowingPassiveSigilCard(true);

            isShowing = true;

            CardTracker.Instance.canInteract = true;
            CardTracker.Instance.isReward = false;
        }

        public override async void Hide() 
        {
            if (!isShowing) return;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            await ShowingPassiveSigilCard(false);

            DesTroyCard();

            passiveCardsCons?.SetActive(false);

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();

            isShowing = false;

            CardTracker.Instance.canInteract = false;

            RTSCameraController.Instance.CanInteract = true;

            DialogueManager.Instance.CanContinueDialogue = true;

            DialogueManager.Instance.ContinueStoryOrExitStory();
        }

        private UniTask ShowingPassiveSigilCard(bool showing)
        {
            List<SigilData> sigils = MatchManager.Instance.MatchData.SigilPool.GetPassiveSigils(passiveCards.Length);

            for (int i = 0; i < passiveCards.Length; i++)
            {
                SigilData sigilData = sigils[i];

                if (sigilData == null) break;
                
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                passiveCards[i].SetSigil(sigilData, sigilSO);
                passiveCards[i].IsShowing = showing;
                passiveCards[i].ShowPrice(showing);
            }

            return UniTask.CompletedTask;
        }

        private void SpawnActiveSigil()
        {
            List<SigilData> sigils = MatchManager.Instance.MatchData.SigilPool.GetActiveSigils(spawnPoint.Length);

            for (int i = 0; i < spawnPoint.Length; i++) 
            {
                SigilData sigilData = sigils[i];
                
                if (sigilData == null) break;

                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                if (sigilSO == null) continue;

                var cardObj = Instantiate(sigilSO.sigilPreb, spawnPoint[i]);
                Card card = cardObj.GetComponent<Card>();
                gameObjects.Add(card);
                card.SetSigil(sigilData, sigilSO);
                card.IsShowing = true;
                card.ShowPrice(true);
            }
        }

        private void ShowingItem()
        {
            for (int i = 0; i < items.Length; i++)
            {
                ItemSO itemSO = ItemManager.Instance.GetRandomItem();
                items[i].SetUp(itemSO);
            }

            items[0].Select();
        }

        private void DesTroyCard()
        {
            foreach (var card in gameObjects)
            {
                Destroy(card.gameObject);
            }

            gameObjects.Clear();
        }
    }
}