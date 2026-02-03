using UnityEngine;

public struct SummonerTargetEvent : IEvent
{
    public Transform target;
    public SummonerTargetEvent(Transform newTarget)
    {
        target = newTarget;
    }
}
public struct TopDownStartGameEvent : IEvent { }
public struct TopDownPlayerDeadEvent : IEvent { }
public struct TopDownInitializeSkillsEvent : IEvent 
{
    public SkillRuntimeInstance[] skillRuntimeInstances;
    public TopDownInitializeSkillsEvent(SkillRuntimeInstance[] instances)
    {
        skillRuntimeInstances = instances;
    }
}
