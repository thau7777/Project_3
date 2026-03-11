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

        public SigilChosenEvent(SigilSO normalSigilSO)
        {
            this.sigilSO = normalSigilSO;
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