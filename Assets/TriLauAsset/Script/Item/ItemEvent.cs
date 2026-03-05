using UnityEngine;

namespace MyRule
{
    public struct AddItemEvent : IEvent
    {
        public readonly int index;
        public readonly Item item;

        public AddItemEvent(int index, Item item)
        {
            this.index = index;
            this.item = item;
        }
    }
}