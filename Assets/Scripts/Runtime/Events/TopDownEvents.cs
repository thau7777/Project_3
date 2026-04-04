using UnityEngine;

public struct SummonerTargetEvent : IEvent
{
    public Transform target;
    public SummonerTargetEvent(Transform newTarget)
    {
        target = newTarget;
    }
}
public struct TopdownStartGameEvent : IEvent 
{
    public bool isBossFight;
    public TopdownStartGameEvent(bool isBossFight)
    {
        this.isBossFight = isBossFight;
    }
}
public struct TopDownEndGameEvent : IEvent 
{ 
    public UIEndGameExecuteState endGameExecuteState;
    public TopDownEndGameEvent(UIEndGameExecuteState endGameExecuteState)
    {
        this.endGameExecuteState = endGameExecuteState;
    }
}
public struct TopdownOnEndGameContinueEvent : IEvent { }
public struct TopdownInitializeSkillsEvent : IEvent
{
    public SkillRuntimeInstance[] skillRuntimeInstances;
    public TopdownInitializeSkillsEvent(SkillRuntimeInstance[] instances)
    {
        skillRuntimeInstances = instances;
    }
}
public enum SkillOnUseState
{
    Reset,
    Use,
    OnCooldown,
    NotEnoughMana
}
public struct TopdownSkillOnUseEvent : IEvent
{
    public SkillOnUseState skillOnUseState;
    public int skillIndex;
    public TopdownSkillOnUseEvent(SkillOnUseState skillOnUseState, int skillIndex)
    {
        this.skillOnUseState = skillOnUseState;
        this.skillIndex = skillIndex;
    }
}
public struct TopdownInitializeItemsEvent : IEvent
{
    public ItemRuntimeInstance[] items;
    public TopdownInitializeItemsEvent(ItemRuntimeInstance[] items)
    {
        this.items = items;
    }
}
