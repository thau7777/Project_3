using UnityEngine;

public struct SummonerTargetEvent : IEvent
{
    public Transform target;
    public SummonerTargetEvent(Transform newTarget)
    {
        target = newTarget;
    }
}
public struct TopdownStartGameEvent : IEvent { }
public struct TopDownPlayerDeadEvent : IEvent { }
public struct TopDownInitializeSkillsEvent : IEvent 
{
    public SkillRuntimeInstance[] skillRuntimeInstances;
    public TopDownInitializeSkillsEvent(SkillRuntimeInstance[] instances)
    {
        skillRuntimeInstances = instances;
    }
}
public struct TopDownInitializeItemsEvent : IEvent
{
    public ItemRuntimeInstance[] items;
    public TopDownInitializeItemsEvent(ItemRuntimeInstance[] items)
    {
        this.items = items;
    }
}
