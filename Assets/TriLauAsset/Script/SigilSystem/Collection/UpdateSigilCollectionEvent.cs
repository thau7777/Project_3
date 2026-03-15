using UnityEngine;

namespace MyRule.Event
{
    public struct UpdateSigilCollectionEvent : IEvent
    {
        public readonly SigilCollectionData data;

        public UpdateSigilCollectionEvent(SigilCollectionData data)
        {
            this.data = data;
        }
    }
}