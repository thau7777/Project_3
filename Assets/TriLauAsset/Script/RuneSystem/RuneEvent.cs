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

    public struct SpendRuneEvent : IEvent
    {
        public readonly int runeAmount;
        public SpendRuneEvent(int runeAmount)
        {
            this.runeAmount = runeAmount;
        }
    }

    public struct SendUIRuneEvent : IEvent
    {
        public readonly int runAmount;
        public readonly int runeLockAmount;

        public SendUIRuneEvent(int runeAmount, int runeLock)
        {
            this.runAmount = runeAmount;
            this.runeLockAmount = runeLock;
        }
    }
}