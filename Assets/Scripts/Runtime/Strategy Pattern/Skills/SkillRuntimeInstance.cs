using UnityEngine;
public class SkillRuntimeInstance
{
    public readonly SkillStrategy Definition;
    public float lastCastTime;

    public SkillRuntimeInstance(SkillStrategy definition)
    {
        Definition = definition;
        lastCastTime = -definition.Cooldown; // allow immediate first use
    }

    public bool IsOnCooldown => Time.time < lastCastTime + Definition.Cooldown;

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

