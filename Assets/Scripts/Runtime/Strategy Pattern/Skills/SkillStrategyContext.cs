using UnityEngine;

public class SkillStrategyContext : IStrategyContext
{
    public Transform origin;
    public Transform spawnTransform;
    public Vector3 positionOffset;
    public Flyweight chargedSkillFlyweight; 
    public SkillStrategyContext(Transform origin, Transform spawnTransform, Vector3 positionOffset, Flyweight chargedSkillFlyweight = null)
    {
        this.origin = origin;
        this.spawnTransform = spawnTransform;
        this.chargedSkillFlyweight = chargedSkillFlyweight;
        this.positionOffset = positionOffset;
    }
}
