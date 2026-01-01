using UnityEngine;

namespace MyRule
{
    public struct FirstShapeEvent : IEvent
    {
        public readonly ShapeInfo shape;

        public FirstShapeEvent(ShapeInfo shape)
        {
            this.shape = shape;
        }
    }
    public struct MazeMoveEvent : IEvent
    {
        public readonly int steps;

        public MazeMoveEvent(int steps)
        {
            this.steps = steps;
        }
    }
}