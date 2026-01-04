using UnityEngine;

namespace MyRule
{
    public struct ShowSigilCardEvent : IEvent
    {
        public readonly bool showing;

        public ShowSigilCardEvent(bool showing)
        {
            this.showing = showing;
        }
    }


}