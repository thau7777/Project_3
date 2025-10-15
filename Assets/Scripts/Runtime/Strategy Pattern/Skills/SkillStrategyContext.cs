using UnityEngine;

public class SkillStrategyContext : IStrategyContext
{
    public Transform origin;
    public Vector3 spawnPoint;
    public Flyweight chargedSkillFlyweight; 
    public SkillStrategyContext(Transform origin, Vector3 spawnPoint, Flyweight chargedSkillFlyweight = null)
    {
        this.origin = origin;
        this.spawnPoint = spawnPoint;
        this.chargedSkillFlyweight = chargedSkillFlyweight;
    }
}
