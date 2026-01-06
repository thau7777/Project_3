using UnityEngine;

namespace MyRule
{
    public struct ShowSigilCardEvent : IEvent
    {
        public readonly bool showing;

        public ShowSigilCardEvent(bool showing)
        {
            this.showing = showing;
        }
    }

    public struct RollSigilCardEvent : IEvent
    {
    }

    public struct HoverSigilCardEvent : IEvent
    {
        public readonly Card card;

        public HoverSigilCardEvent(Card card)
        {
            this.card = card;
        }
    }
}