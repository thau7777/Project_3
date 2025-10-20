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
    public GameObject impactVFXPrefab;

    [Header("Buff/Debuff Properties")]
    public StatType statToModify;
    public int durationTurns = 2;
    public bool isStackable = false;

}

public enum StatType
{
    None,
    Attack,
    Defense,
    Agility,
    MaxHP,

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
}

public enum ElementType
{
    None,
    Physical,
    Magical,
    Fire,
    Ice,
    Poison,
    Lightning,
    Holy,
    Dark,

}