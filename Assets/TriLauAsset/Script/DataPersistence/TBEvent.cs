using UnityEngine;

public struct TBVictoryEvent : IEvent
{
    public readonly bool isVictory;
    public TBVictoryEvent(bool result)
    {
        this.isVictory = result;
    }
}
