using UnityEngine;

namespace MyRule.Event
{
    public struct HUDEvent : IEvent
    {
      
    }

    public struct AddSigilEvent : IEvent
    {
        public readonly SigilSO sigilSO;

        public AddSigilEvent(SigilSO sigilSO)
        {
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