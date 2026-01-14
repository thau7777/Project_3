using UnityEngine;

namespace MyRule
{
    public struct SigilBoardExitEvent : IEvent
    {
        
    }

    public struct PlayerStatsShowEvent : IEvent
    {
        
    }

    public struct SigilChosenEvent : IEvent
    {
        public readonly SigilSO normalSigilSO;

        public SigilChosenEvent(SigilSO normalSigilSO)
        {
            this.normalSigilSO = normalSigilSO;
        }
    }

    public struct MazeGameplayEvent : IEvent
    {
        public readonly ShapeType shapeType;

        public MazeGameplayEvent(ShapeType shapeType)
        {
            this.shapeType = shapeType;
        }
    }
}