using UnityEngine;

namespace MyRule.Event
{
    public struct TriggerMiniGameEvent : IEvent
    {
        public readonly string name;

        public TriggerMiniGameEvent(string name)
        {
            this.name = name;
        }
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