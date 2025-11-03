using System.Collections.Generic;
using UnityEngine;
using static Skill;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public int damage;
    public int manaCost;
    public Sprite icon;
    public string animationTriggerName;

    public SkillTargetType targetType;
    public SkillType skillType;
    public ElementType elementType;

    [ShowIfEnumValue("skillType", SkillType.Summon)]
    public List<GameObject> summonPrefab;


    [Header("Visual Effects")]
    public FlyweightSettings impactVFXPrefab;

    [Header("Projectile & VFX")]
    [ShowIfEnumValue("skillType", SkillType.RangedProjectile)]
    public FlyweightSettings projectileSettings;

    public float impactVFXDuration = 1.0f;


    [Header("Buff Properties")]
    [ShowIfEnumValue("skillType", SkillType.Buff, SkillType.Shield)]
    public BuffSettings buffProperties;

    [Header("Stack Properties")]
    public StackApplicationTarget stackApplicationTarget;
    public StackSetting stackSetting;

    [ShowIfEnumValue("stackApplicationTarget", StackApplicationTarget.Self)]
    public BuffSettings activatedBuff;
    [ShowIfEnumValue("stackApplicationTarget", StackApplicationTarget.Target)]
    public DebuffSettings activatedDebuff;


    [System.Serializable]
    public struct StackSetting 
    {
        public bool isStackBuilder;
        public int stackAmountPerUse;

        public bool isStackFinisher;
        public int stackThreshold;

    }



    [System.Serializable]
    public struct BuffSettings
    {
        public StatType statToModify;
        public int durationTurns;
        public Sprite icon;
    }


    public DebuffSettings debuffProperties;
    [System.Serializable]
    public struct DebuffSettings 
    {
        public DebuffType debuffType;
        public int durationTurns;
        public int baseDamagePerTurn;
        public FlyweightSettings debuffEffect;
        public Sprite icon;
    }


}

public enum StackApplicationTarget
{
    None,
    Self,
    Target,

}
 

public enum StatType
{
    None,
    Attack,
    Defense,
    Agility,
    MaxHP,
    MagicalAttack,
    MagicalDefense,

}

public enum SkillTargetType
{
    Self,      
    Ally,      
    Enemy,     
    Enemies,   
    Allies,    

}

public enum SkillType
{
    MeleeAttack,
    RangedAttack,
    DamageAll,
    Heal,
    Buff,
    Shield,
    Special, 
    Summon,
    RangedProjectile,
}

public enum ElementType
{
    None,
    Physical,
    Magical,
    Fire,
    Water,  
    Ice,
    Poison,
    Lightning,
    Dark,
    Frost,
    Holy,

}

public enum  DebuffType
{
    None,
    Burn,
    Poison,
    Stun,
}

