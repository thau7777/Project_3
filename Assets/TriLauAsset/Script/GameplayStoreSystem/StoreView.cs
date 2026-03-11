using Cysharp.Threading.Tasks;
using MyRule.UI;
using System;
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

        [SerializeField] private Transform[] spawnPoint;
        [SerializeField] private GameObject passiveCardsCons;
        [SerializeField] private Card[] passiveCards;
        [SerializeField] private StoreItemView[] items;

        [SerializeField] private GameObject cam;

        private CancellationTokenSource cts;

        private bool isShowing = false;
        private List<GameObject> gameObjects = new List<GameObject>();

        protected override void Start()
        {
            base.Start();

            cts = new CancellationTokenSource();

            inputReader.diceRollActions.onEsc += Hide;
        }

        public override async void Show()
        {
            if (isShowing) return;
            
            cam.gameObject.SetActive(true);

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

            RTSCameraController.Instance.CanInteract = false;

            CardTracker.Instance.canInteract = true;
            CardTracker.Instance.isReward = false;
        }

        public override async void Hide() 
        {
            if (!isShowing) return;

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

            cam.gameObject.SetActive(false);

            RTSCameraController.Instance.CanInteract = true;
        }

        private UniTask ShowingPassiveSigilCard(bool showing)
        {
            for (int i = 0; i < passiveCards.Length; i++)
            {
                SigilData sigilData = MatchManager.Instance.GetRandomPassiveSigilInMatch();

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
            for (int i = 0; i < spawnPoint.Length; i++) 
            {
                SigilData sigilData = MatchManager.Instance.GetRandomActiveSigilInMatch();
                
                if (sigilData == null) break;

                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                var cardObj = Instantiate(sigilSO.sigilPreb, spawnPoint[i]);

                gameObjects.Add(cardObj);

                Card card = cardObj.GetComponent<Card>();
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