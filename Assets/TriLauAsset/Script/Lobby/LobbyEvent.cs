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

    public struct ShowLobbyEvent : IEvent
    {
        public readonly bool show;

        public ShowLobbyEvent(bool shown)
        {
            this.show = shown;
        }
    }

    public struct ReceiveGoldEvent : IEvent
    {
        public readonly int value;
        public ReceiveGoldEvent(int value)
        {
            this.value = value;
        }
    }

    public struct ReceiveCrystalEvent : IEvent
    {
        public readonly int value;
        public ReceiveCrystalEvent(int value)
        {
            this.value = value;
        }
    }
}