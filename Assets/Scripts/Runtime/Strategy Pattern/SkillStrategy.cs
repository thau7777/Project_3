using System;
using System.Collections.Generic;
using UnityEngine;

public enum AimType
{
    None,
    Below,
    Straight,
    Above
}
[Serializable]
public struct SkillDataForClass
{
    public CharacterClass characterClass;
    public string animName;
    public VFXSpawnLocation spawnLocation;
    public bool isDashForward;
    public float dashForce;
    public AimType aimType;
}
public abstract class SkillStrategy : ScriptableObject, IStrategy
{
    [SerializeField]
    protected List<SkillDataForClass> dataForClasses;

    [SerializeField]
    protected FlyweightSettings flyweightSettings;
    public string SkillName { get; private set; }

    [SerializeField]
    protected float _cooldown;
    public float Cooldown => _cooldown;

    [SerializeField]
    protected bool _needHoldStill;
    public bool NeedHoldStill => _needHoldStill;
    public abstract void Execute(IStrategyContext context);

    public SkillDataForClass? GetSkillDataByClass(CharacterClass characterClass)
    {
        bool hasThatClass = false;
        foreach (var dataForClass in dataForClasses)
        {
            if (dataForClass.characterClass == characterClass)
            {
                hasThatClass = true;
                break;
            }
        }
        if (!hasThatClass) return null;
        
        foreach (var dataForClass in dataForClasses)
        {
            if(dataForClass.characterClass == characterClass)
                return dataForClass;
        }
        return null;
    }

    protected void SpawnProjectile(SkillStrategyContext context)
    {
        Flyweight projectile = FlyweightFactory.Spawn(flyweightSettings);
        projectile.transform.position = context.spawnPoint;
        projectile.transform.rotation = context.origin.rotation;
        if(projectile is StraightProjectile)
        {
            var straightProjectile = projectile as StraightProjectile;
            straightProjectile.Initialize(straightProjectile.transform.forward);
        }
    }
}
