using UnityEngine;

public class SkillStrategyContext : IStrategyContext
{
    public Transform origin;
    public Transform spawnTransform;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Flyweight chargedSkillFlyweight; 
    public SkillStrategyContext(Transform origin, Transform spawnTransform, Vector3 positionOffset, Vector3 rotationOffset, Flyweight chargedSkillFlyweight = null)
    {
        this.origin = origin;
        this.spawnTransform = spawnTransform;
        this.chargedSkillFlyweight = chargedSkillFlyweight;
        this.positionOffset = positionOffset;
        this.rotationOffset = rotationOffset;
    }
}
