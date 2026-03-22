using UnityEngine;

namespace MyRule.Event
{
    public struct OpenHUDEvent : IEvent
    {
        public readonly bool show;

        public OpenHUDEvent(bool show)
        {
            this.show = show;
        }
    }

    public struct AddActiveSigilEvent : IEvent
    {
        public readonly int index;
        public readonly SigilSO sigilSO;

        public AddActiveSigilEvent(int index, SigilSO sigilSO)
        {
            this.index = index;
            this.sigilSO = sigilSO;
        }
    }

    public struct AddPassiveSigilEvent : IEvent
    {
        public readonly int index;
        public readonly SigilSO sigilSO;

        public AddPassiveSigilEvent(int index, SigilSO sigilSO)
        {
            this.index = index;
            this.sigilSO = sigilSO;
        }
    }

    public struct AddItemEvent : IEvent
    {
        public readonly ItemData item;

        public AddItemEvent(ItemData item)
        {
            this.item = item;
        }
    }
}