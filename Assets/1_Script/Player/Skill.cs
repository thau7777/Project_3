using System.Collections.Generic;
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
    public FlyweightSettings impactVFXPrefab;

    [Header("Projectile & VFX")]
    [ShowIfEnumValue("skillType", SkillType.RangedProjectile)]
    public FlyweightSettings projectileSettings;

    public float impactVFXDuration = 1.0f;


    [Header("Buff/Debuff Properties")]
    [ShowIfEnumValue("skillType", SkillType.Buff)]
    public BuffSettings buffProperties;

    [System.Serializable]
    public struct BuffSettings
    {
        public StatType statToModify;
        public int durationTurns;
    }


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
    Debuff,
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

}