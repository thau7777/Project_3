using UnityEngine;

namespace MyRule.Event
{
    public struct UpdateLobbyGoldUIEvent : IEvent
    {
        public readonly int value;

        public UpdateLobbyGoldUIEvent(int value)
        {
            this.value = value;
        }
    }

    public struct UpdateLobbyCrystalUIEvent : IEvent
    {
        public readonly int value;

        public UpdateLobbyCrystalUIEvent(int value)
        {
            this.value = value;
        }
    }
}