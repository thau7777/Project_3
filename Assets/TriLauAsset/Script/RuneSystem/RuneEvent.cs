using UnityEngine;

namespace MyRule
{
    public struct ReceiveRuneEvent : IEvent
    {
        public readonly int runeCount;

        public ReceiveRuneEvent(int runeCount)
        {
            this.runeCount = runeCount;
        }
    }
}