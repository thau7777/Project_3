using UnityEngine;

namespace MyRule
{
    public struct ShowCardEvent : IEvent
    {
        public readonly bool showing;

        public ShowCardEvent(bool showing)
        {
            this.showing = showing;
        }
    }

    public struct RollCardEvent : IEvent
    {
    }

    public struct HoverCardEvent : IEvent
    {
        public readonly Card card;

        public HoverCardEvent(Card card)
        {
            this.card = card;
        }
    }

    public struct CardDetailLockEvent : IEvent
    {
        public readonly bool locking;

        public CardDetailLockEvent(bool locking)
        {
            this.locking = locking;
        }
    }

    public struct ShowCardDetailEvent : IEvent
    {
        public readonly SigilSO sigilSO;
        public readonly SigilData sigilData;

        public ShowCardDetailEvent(SigilSO sigilSO, SigilData sigilData)
        {
            this.sigilSO = sigilSO;
            this.sigilData = sigilData;
        }
    }

    public struct CheckSigilReplaced : IEvent
    {
        
    }
}