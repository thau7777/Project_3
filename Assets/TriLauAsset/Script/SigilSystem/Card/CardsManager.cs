using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class CardsManager : MonoBehaviour
    {
        [SerializeField] private List<Card> cards;
        [SerializeField] private int delayTime = 1;

        private EventBinding<ShowSigilCardEvent> showSigilCardEventBinding;

        private void OnEnable()
        {
            showSigilCardEventBinding = new EventBinding<ShowSigilCardEvent>(ShowCardsHandler);
            EventBus<ShowSigilCardEvent>.Register(showSigilCardEventBinding);
        }

        private void OnDisable()
        {
            EventBus<ShowSigilCardEvent>.Deregister(showSigilCardEventBinding);
        }

        private void Start()
        {
            EventBus<ShowSigilCardEvent>.Raise(new ShowSigilCardEvent(true));
        }

        private async void ShowCardsHandler(ShowSigilCardEvent evt)
        {
            foreach (var card in cards)
            {
                card.Showing = evt.showing;
                await UniTask.Delay(delayTime);
            }
        }
    }
}