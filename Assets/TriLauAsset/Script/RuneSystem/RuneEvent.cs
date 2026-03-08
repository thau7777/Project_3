using UnityEngine;

namespace MyRule
{
    public struct ReceiveRuneEvent : IEvent
    {
        public readonly int runeAmount;

        public ReceiveRuneEvent(int runeAmount)
        {
            this.runeAmount = runeAmount;
        }
    }

    public struct SendUIRuneEvent : IEvent
    {
        public readonly int runAmount;

        public SendUIRuneEvent(int runeAmount)
        {
            this.runAmount = runeAmount;
        }
    }
}