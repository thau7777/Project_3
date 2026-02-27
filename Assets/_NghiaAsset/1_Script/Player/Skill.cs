using System.Collections.Generic;
using MyRule.Audio;
using Turnbase;
using UnityEngine;

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
    public FlyweightSettings_TB impactVFXPrefab;

    [Header("Melee & VFX")]
    [ShowIfEnumValue("skillType", SkillType.MeleeAttack)]
    public FlyweightSettings_TB meleeSettings;

    [Header("Projectile & VFX")]
    [ShowIfEnumValue("skillType", SkillType.RangedProjectile)]
    public FlyweightSettings_TB projectileSettings;
    [ShowIfEnumValue("skillType", SkillType.RangedProjectile)]
    public bool useSkillSpawnPoint2;

    [Header("Lazer & VFX")]
    [ShowIfEnumValue("skillType", SkillType.LaserAttack)]
    public FlyweightSettings_TB lazerSettings;
    [ShowIfEnumValue("skillType", SkillType.LaserAttack)]
    public float laserVFXDuration = 5;

    public float impactVFXDuration = 1.0f;


    [Header("Buff Properties")]
    [ShowIfEnumValue("skillType", SkillType.Buff, SkillType.Shield)]
    public BuffSettings buffProperties;

    [Header("Stack Properties")]
    public StackApplicationTarget stackApplicationTarget;

    [ShowIfEnumValue("stackApplicationTarget", StackApplicationTarget.Self, StackApplicationTarget.Target)]
    public StackSetting stackSetting;
    [ShowIfEnumValue("stackApplicationTarget", StackApplicationTarget.Self)]
    public BuffSettings activatedBuff;
    [ShowIfEnumValue("stackApplicationTarget", StackApplicationTarget.Target)]
    public DebuffSettings activatedDebuff;

    [Header("Color Lookup")]
    public ElementColorMap elementColorMap;

    [Header("Multi-Hit")]
    public int numberOfHits = 1;
    public float delayBetweenHits = 0.1f;
    [ShowIfEnumValue("skillType", SkillType.MeleeAttack)]
    public int attackCount = 1;

    [Header("Sound Effects")]
    public SFXType castSFXType;  
    public SFXType impactSFXType;


    [System.Serializable]
    public struct StackSetting 
    {
        public bool isStackBuilder;
        public int stackAmountPerUse;

        public string stackId;
        public Sprite iconStack;

        public bool isStackFinisher;
        public int stackThreshold;

    }

    [System.Serializable]
    public struct BuffSettings
    {
        public StatType statToModify;
        public int amount;
        public int durationTurns;
        public Sprite icon;
    }

    public DebuffSettings debuffProperties;
    [System.Serializable]
    public struct DebuffSettings 
    {
        public DebuffType statToModify;
        public int durationTurns;
        public int baseDamagePerTurn;
        public float debuffValue;
        public FlyweightSettings_TB debuffEffect;
        public Sprite icon;
    }

    [Header("Timeline")]
    public UnityEngine.Playables.PlayableAsset cameraTimeline;

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
    IgnoreDefense,
    BasicAttackDamage,
    SplashDamage,
    DivineShield,
    Purify,

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
    DamageGlobal,
    LaserAttack,

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
    Normal,

}

public enum  DebuffType
{
    None,
    Burn,
    Poison,
    Stun,
    DefReduction,
    Break,
    SpeedReduction,


}

