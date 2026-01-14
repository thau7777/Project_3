using UnityEngine;


namespace MyRule
{

    public struct ScifiMouseMoveEvent : IEvent
    {
        public readonly Vector2 mousePosition;
        
        public ScifiMouseMoveEvent(Vector2 mousePosition)
        {
            this.mousePosition = mousePosition;
        }
    }

    public struct ScifitableInteractEvent : IEvent
    {
    }

    public struct ScifitableExitEvent : IEvent
    {
    }

    public struct ScifitableActiveEvent : IEvent
    {
    }
}