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

    [SerializeField]
    private bool _canCharge;
    public bool CanCharge { get => _canCharge; private set => _canCharge = value; }

    [ShowIf("_canCharge")]
    public FlyweightSettings chargingEffect;
    [ShowIf("_canCharge")]
    public int chargeLevel;
    public void Execute(IStrategyContext context)
    {
        var skillContext = context as SkillStrategyContext;
        if (!skillContext.chargedSkillFlyweight)
            SpawnSkillVFX(skillContext);
        else
            ExecuteChargedSkill(skillContext);
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

    private void SpawnSkillVFX(SkillStrategyContext context)
    {
        if (name == "Basic Dash") return;

        Flyweight flyweightObj = FlyweightFactory.Spawn(FlyweightSettings);
        flyweightObj.Initialize(context.spawnPos, context.origin.rotation);
        if (flyweightObj is StraightProjectile straightProjectile)
        {
            straightProjectile.InitializeMovement(context.origin.forward, 10);
        }

        

    }
    private void ExecuteChargedSkill(SkillStrategyContext context)
    {
        // initialize that charged skill direction and speed
        if (context.chargedSkillFlyweight is StraightProjectile chargedSkillProjectile)
        {
            context.chargedSkillFlyweight.transform.SetParent(null);
            chargedSkillProjectile.InitializeMovement(context.origin.forward, 10);
        }
    }
}
