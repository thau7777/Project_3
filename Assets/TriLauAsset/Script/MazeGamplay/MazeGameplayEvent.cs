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
        public readonly SigilSO sigilSO;
        public readonly int index;

        public SigilChosenEvent(SigilSO normalSigilSO, int index)
        {
            this.sigilSO = normalSigilSO;
            this.index = index;
        }
    }

    public struct MazeGameplayEvent : IEvent
    {
        public readonly NodeType nodeType;

        public MazeGameplayEvent(NodeType shapeType)
        {
            this.nodeType = shapeType;
        }
    }
}