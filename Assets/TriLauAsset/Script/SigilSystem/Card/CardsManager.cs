using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class CardsManager : MonoBehaviour
    {
        [SerializeField] private int delayTime = 1;
        [SerializeField] private GroupSigil groupSigil;
        [SerializeField] private List<Card> cards = new List<Card>();

        private EventBinding<ShowSigilCardEvent> showSigilCardEventBinding;
        private EventBinding<RollSigilCardEvent> rollSigilCardEventBinding;

        private void OnEnable()
        {
            showSigilCardEventBinding = new EventBinding<ShowSigilCardEvent>(ShowCardsHandler);
            EventBus<ShowSigilCardEvent>.Register(showSigilCardEventBinding);

            rollSigilCardEventBinding = new EventBinding<RollSigilCardEvent>(RollSigilHandler);
            EventBus<RollSigilCardEvent>.Register(rollSigilCardEventBinding);
        }

        private void OnDisable()
        {
            EventBus<ShowSigilCardEvent>.Deregister(showSigilCardEventBinding);
            EventBus<RollSigilCardEvent>.Deregister(rollSigilCardEventBinding);
        }

        private async void ShowCardsHandler(ShowSigilCardEvent evt)
        {
            if (evt.showing)
            {
                SpawnCard(evt.showing);
            }
            else
            {
                await DeleteCard();
            }
        }

        private async void SpawnCard(bool evt)
        {
            int x = -500;

            for (int i = 0; i < 3; i++)
            {
                SigilSO sigilSO = GetWeightedRandom();
                Card card = Instantiate(sigilSO.sigilPreb, transform).GetComponent<Card>();
                cards.Add(card);
                card.transform.localPosition = new Vector3(x, -1000, 0);
                card.IsShowing = evt;
                card.SetSigil(sigilSO);
                await UniTask.Delay(delayTime);
                x += 500;
            }
        }

        private async UniTask DeleteCard()
        {
            foreach (var card in cards)
            {
                card.IsShowing = false;
                await UniTask.Delay(delayTime);
                Destroy(card.gameObject);
            }

            cards.Clear();
        }

        private async void RollSigilHandler(RollSigilCardEvent evt)
        {
            Debug.Log("roll");

            await DeleteCard();

            SpawnCard(true);
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