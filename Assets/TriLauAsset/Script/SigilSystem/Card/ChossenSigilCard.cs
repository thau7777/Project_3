using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class ChosenSigilCard : MonoBehaviour
    {
        public GroupSigil groupSigil;
        public RuneSO runeSO;

        private Card hoverCard;

        private EventBinding<HoverSigilCardEvent> hoverSigilCardEventBinding;

        private void OnEnable()
        {
            hoverSigilCardEventBinding = new EventBinding<HoverSigilCardEvent>(OnHoverSigilCardEvent);
            EventBus<HoverSigilCardEvent>.Register(hoverSigilCardEventBinding);
        }

        private void OnDisable()
        {
            EventBus<HoverSigilCardEvent>.Deregister(hoverSigilCardEventBinding);
        }

        private void OnHoverSigilCardEvent(HoverSigilCardEvent evt)
        {
            hoverCard = evt.card;
        }

        private void Update()
        {
            if (hoverCard == null) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                EventBus<DeleteSigilCardEvent>.Raise(new DeleteSigilCardEvent());

                hoverCard = null;
            }
        }
    }
}
