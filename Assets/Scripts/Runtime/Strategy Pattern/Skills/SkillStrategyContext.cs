using UnityEngine;

public class SkillStrategyContext : IStrategyContext
{
    public Transform origin;
    public Transform spawnTransform;
    public Vector3 positionOffset;
    public Quaternion rotationOffset;
    public Flyweight chargedSkillFlyweight; 
    public SkillStrategyContext(Transform origin, Transform spawnTransform, Vector3 positionOffset,Quaternion rotationOffset, Flyweight chargedSkillFlyweight = null)
    {
        this.origin = origin;
        this.spawnTransform = spawnTransform;
        this.chargedSkillFlyweight = chargedSkillFlyweight;
        this.positionOffset = positionOffset;
        this.rotationOffset = rotationOffset;
    }
}
