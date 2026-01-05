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
    public struct MazeStepEvent : IEvent
    {
        public readonly int steps;

        public MazeStepEvent(int steps)
        {
            this.steps = steps;
        }
    }

    public struct MazeSetMovePosEvent : IEvent
    {
        public readonly Transform target;

        public MazeSetMovePosEvent(Transform target)
        {
            this.target = target;
        }
    }

    public struct MazeMoveEvent : IEvent
    {
        public readonly ShapeInfo shapeInfo;

        public MazeMoveEvent(ShapeInfo shapeInfo)
        {
            this.shapeInfo = shapeInfo;
        }
    }

    public struct MazeJumpEvent : IEvent
    {
        
    }

    public struct CamTargetEvent : IEvent
    {
        public readonly Transform target;

        public CamTargetEvent(Transform target)
        {
            this.target = target;
        }
    }
}