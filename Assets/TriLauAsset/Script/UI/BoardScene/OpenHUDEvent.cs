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
        public readonly SigilData sigilData;

        public AddActiveSigilEvent(int index, SigilSO sigilSO, SigilData sigilData)
        {
            this.index = index;
            this.sigilSO = sigilSO;
            this.sigilData = sigilData;
        }
    }

    public struct AddPassiveSigilEvent : IEvent
    {
        public readonly int index;
        public readonly SigilSO sigilSO;
        public readonly SigilData sigilData;

        public AddPassiveSigilEvent(int index, SigilSO sigilSO, SigilData sigilData)
        {
            this.index = index;
            this.sigilSO = sigilSO;
            this.sigilData = sigilData;
        }
    }

    public struct RemoveActiveSigilEvent : IEvent
    {
        public readonly int index;

        public RemoveActiveSigilEvent(int index)
        {
            this.index = index;
        }
    }

    public struct RemovePassiveSigilEvent : IEvent
    {
        public readonly int index;

        public RemovePassiveSigilEvent(int index)
        {
            this.index = index;
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