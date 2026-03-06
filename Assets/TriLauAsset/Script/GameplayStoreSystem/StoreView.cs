using System.Threading;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MyRule
{
    public class StoreView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI runeTxt;
        [SerializeField] private float fadeDuration = 0.2f;

        [SerializeField] private GroupSigil groupSigil;
        [SerializeField] private Transform[] spawnPoint;
        [SerializeField] private GameObject passiveCardsCons;
        [SerializeField] private Card[] passiveCards;

        private CancellationTokenSource cts;

        private bool isShowing = false;
        private List<GameObject> gameObjects = new List<GameObject>();

        private void Start()
        {
            cts = new CancellationTokenSource();
            
            Show();
        }

        public async void Show()
        {
            if (isShowing) return;

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

            await ShowingCard(true);

            isShowing = true;
        }

        public async void Hide() 
        {
            if (!isShowing) return;

            await ShowingCard(false);

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
        }

        private UniTask ShowingCard(bool showing)
        {
            for (int i = 0; i < passiveCards.Length; i++)
            {
                passiveCards[i].IsShowing = showing;
            }

            return UniTask.CompletedTask;
        }

        private void SpawnActiveSigil()
        {
            for (int i = 0; i < spawnPoint.Length; i++) 
            {
                SigilSO sigilSO = GetWeightedRandom();
                var cardObj = Instantiate(sigilSO.sigilPreb, spawnPoint[i]);

                gameObjects.Add(cardObj);

                Card card = cardObj.GetComponent<Card>();
                card.IsShowing = true;
            }
        }

        private void DesTroyCard()
        {
            foreach (var card in gameObjects)
            {
                Destroy(card);
            }
        }

        private SigilSO GetWeightedRandom()
        {
            int totalWeight = 0;
            foreach (var s in groupSigil.normalSigil)
                totalWeight += s.rarity;

            int random = Random.Range(0, totalWeight);
            int current = 0;

            foreach (var s in groupSigil.normalSigil)
            {
                current += s.rarity;
                if (random < current)
                    return s;
            }

            return groupSigil.normalSigil[0];
        }
    }
}