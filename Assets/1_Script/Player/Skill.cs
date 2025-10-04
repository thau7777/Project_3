using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/New Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public int damage;
    public int manaCost;
    public Sprite icon;

    public SkillTargetType targetType;
    public SkillType skillType;
    public ElementType elementType;

}

// Giữ nguyên các enum
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
    Damage,
    DamageAll,
    Heal,
    Buff,
    Debuff,
    Special
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