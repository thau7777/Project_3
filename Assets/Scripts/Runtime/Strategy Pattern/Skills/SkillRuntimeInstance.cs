using UnityEngine;
public class SkillRuntimeInstance
{
    public readonly SkillStrategy Definition;
    public int SlotIndex;
    public float lastCastTime;

    public SkillRuntimeInstance(SkillStrategy strategy, int index)
    {
        SlotIndex = index;
        Definition = strategy;
        lastCastTime = strategy ? -strategy.Cooldown : 0;
    }
    public float CurrentCooldownRemaining => Mathf.Max(0, (lastCastTime + Definition.Cooldown) - Time.time);
    public float CurrentCooldownNormalized => Mathf.Clamp01((Time.time - lastCastTime) / Definition.Cooldown);
    public bool IsOnCooldown => CurrentCooldownRemaining > 0;

    public void MarkUsed()
    {
        lastCastTime = Time.time;
    }
    public void Cast(SkillStrategyContext context)
    {
        MarkUsed();
        Definition.Execute(context);
    }
}

