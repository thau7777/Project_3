using UnityEngine;

namespace MyRule.Event
{
    public struct HUDEvent : IEvent
    {
      
    }

    public struct AddSigilEnvet : IEvent
    {
        public readonly SigilSO sigilSO;

        public AddSigilEnvet(SigilSO sigilSO)
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