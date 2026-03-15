using UnityEngine;

namespace MyRule.Event
{
    public struct TriggerMiniGameEvent : IEvent
    {

    }

    public struct MiniGameResultEvent : IEvent
    {
        public readonly bool result;

        public MiniGameResultEvent(bool result)
        {
            this.result = result;
        }
    }
}