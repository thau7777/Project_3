using UnityEngine;

namespace MyRule
{
    public struct AddItemEvent : IEvent
    {
        public readonly int index;
        public readonly ItemSO item;

        public AddItemEvent(int index, ItemSO item)
        {
            this.index = index;
            this.item = item;
        }
    }
}