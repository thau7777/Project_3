using UnityEngine;

public interface IEvent { }

public struct TestEvent : IEvent { }

public struct PlayerEvent : IEvent
{
    public int health;
    public int mana;
}

public struct SummonerTargetEvent : IEvent 
{
    public Transform target;
    public SummonerTargetEvent(Transform newTarget)
    {
        target = newTarget;
    }
}
