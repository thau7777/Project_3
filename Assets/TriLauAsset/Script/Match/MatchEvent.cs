using UnityEngine;

namespace MyRule.Event
{
    public struct UpdateMatchResultEvent : IEvent
    {
        public readonly EMatchResult eMatchResult;

        public UpdateMatchResultEvent(EMatchResult eMatchResult)
        {
            this.eMatchResult = eMatchResult;
        }
    }
}