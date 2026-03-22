using UnityEngine;

public class SkillStrategyContext : IStrategyContext
{
    public Transform origin;
    public Vector3 spawnPos;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Flyweight chargedSkillFlyweight; 
    public SkillStrategyContext(Transform origin, Vector3 spawnPos, Vector3 positionOffset, Vector3 rotationOffset, Flyweight chargedSkillFlyweight = null)
    {
        this.origin = origin;
        this.spawnPos = spawnPos;
        this.chargedSkillFlyweight = chargedSkillFlyweight;
        this.positionOffset = positionOffset;
        this.rotationOffset = rotationOffset;
    }
}
