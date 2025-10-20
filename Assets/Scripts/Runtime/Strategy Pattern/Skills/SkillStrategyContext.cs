using UnityEngine;

public class SkillStrategyContext : IStrategyContext
{
    public Transform origin;
    public Vector3 spawnPos;
    public Flyweight chargedSkillFlyweight; 
    public SkillStrategyContext(Transform origin, Vector3 spawnPos, Flyweight chargedSkillFlyweight = null)
    {
        this.origin = origin;
        this.spawnPos = spawnPos;
        this.chargedSkillFlyweight = chargedSkillFlyweight;
    }
}
