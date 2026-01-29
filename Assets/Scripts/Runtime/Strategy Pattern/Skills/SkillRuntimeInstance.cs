using UnityEngine;
public class SkillRuntimeInstance
{
    public readonly SkillStrategy Definition;
    public float lastCastTime;
    public int slotIndex;

    public SkillRuntimeInstance(SkillSlotInfo skillInfo)
    {
        slotIndex = skillInfo.slotIndex;
        Definition = skillInfo.skillStrategy;
        lastCastTime = Definition ? - skillInfo.skillStrategy.Cooldown : 0; // allow immediate first use
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

