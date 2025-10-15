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

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Objects/StrategyPattern/Skill")]
public class SkillStrategy : ScriptableObject, IStrategy
{
    [SerializeField]
    private List<SkillDataForClass> dataForClasses;

    [field: SerializeField]
    public FlyweightSettings FlyweightSettings { get; set; }

    [field: SerializeField]
    public float Cooldown { get; private set; }

    [field: SerializeField]
    public bool NeedHoldStill { get; private set; }

    [field: SerializeField]
    public bool CanCharge { get; private set; }
    public void Execute(IStrategyContext context)
    {
        SpawnSkillVFX((SkillStrategyContext)context);
    }

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

    protected void SpawnSkillVFX(SkillStrategyContext context)
    {
        Flyweight flyweightObj = FlyweightFactory.Spawn(FlyweightSettings);
        flyweightObj.Initialize(context.spawnPoint, context.origin.rotation);
        if(flyweightObj is StraightProjectile)
        {
            var straightProjectile = flyweightObj as StraightProjectile;
            straightProjectile.Initialize(context.origin.forward, 10);
        }
    }
}
