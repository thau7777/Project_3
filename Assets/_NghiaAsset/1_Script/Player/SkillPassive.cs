using UnityEngine;

public enum PassiveEffectType
{
    None,
    HealPerTurn,
    IncreasePermanentMaxHP,
    ManaPerTurn,
    IncreasePermanentMaxMP,
    BonusPhysicalAttack,
    BonusMagicAttack,

}

public enum PassiveTiming
{
    OnBattleStart,
    OnTurnStart   
}

[CreateAssetMenu(fileName = "New Passive Skill", menuName = "Passive Skills/New Passive Skill")]
public class SkillPassive : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    
    [Header("Passive Effect")]
    public PassiveTiming applicationTiming; 
    public PassiveEffectType effectType;
    public float effectValue;
    public float effectValuePercentage;
}

