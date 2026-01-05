using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class CardsManager : MonoBehaviour
    {
        [SerializeField] private int delayTime = 1;
        [SerializeField] private GroupSigil groupSigil;

        private List<Card> cards = new List<Card>(3);

        private EventBinding<ShowSigilCardEvent> showSigilCardEventBinding;
        private EventBinding<RollSigilCardEvent> rollSigilCardEventBinding;
        private EventBinding<DeleteSigilCardEvent> deleteSigilCardEventBinding;

        private void OnEnable()
        {
            showSigilCardEventBinding = new EventBinding<ShowSigilCardEvent>(ShowCardsHandler);
            EventBus<ShowSigilCardEvent>.Register(showSigilCardEventBinding);

            rollSigilCardEventBinding = new EventBinding<RollSigilCardEvent>(RollSigilHandler);
            EventBus<RollSigilCardEvent>.Register(rollSigilCardEventBinding);

            deleteSigilCardEventBinding = new EventBinding<DeleteSigilCardEvent>(Delete);
            EventBus<DeleteSigilCardEvent>.Register(deleteSigilCardEventBinding);
        }

        private void OnDisable()
        {
            EventBus<ShowSigilCardEvent>.Deregister(showSigilCardEventBinding);
            EventBus<RollSigilCardEvent>.Deregister(rollSigilCardEventBinding);
            EventBus<DeleteSigilCardEvent>.Deregister(deleteSigilCardEventBinding);
        }

        private void ShowCardsHandler(ShowSigilCardEvent evt)
        {
            SpawnCard(evt.showing);
        }

        private async void SpawnCard(bool evt)
        {
            int x = -500;

            for (int i = 0; i < 3; i++)
            {
                NormalSigilSO sigilSO = GetWeightedRandom();
                Card card = Instantiate(sigilSO.sigilPreb, transform).GetComponent<Card>();
                cards.Add(card);
                card.transform.localPosition = new Vector3(x, -1000, 0);
                card.Showing = evt;
                card.SetSigil(sigilSO);
                await UniTask.Delay(delayTime);
                x += 500;
            }
        }

        private async void Delete()
        {
            await DeleteCard();
        }

        private async UniTask DeleteCard()
        {
            foreach (var card in cards)
            {
                card.Showing = false;
                await UniTask.Delay(delayTime);
                Destroy(card.gameObject);
            }

            cards.Clear();
        }

        private async void RollSigilHandler(RollSigilCardEvent evt)
        {
            await DeleteCard();

            SpawnCard(true);
        }

        private NormalSigilSO GetWeightedRandom()
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