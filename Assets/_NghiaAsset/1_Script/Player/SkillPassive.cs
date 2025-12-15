using UnityEngine;

namespace Turnbase
{
    public enum PassiveEffectType
    {
        None,
        HealPerTurn,
        IncreasePermanentMaxHP,
        ManaPerTurn,
        IncreasePermanentMaxMP,
        BonusPhysicalAttack,
        BonusMagicAttack,

        SpawnMinionsOnDeath,


    }

    public enum PassiveTiming
    {
        OnBattleStart,
        OnTurnStart,

        OnDeath,

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

        [Header("Spawn Minion Settings (If PassiveEffectType is SpawnMinionsOnDeath)")]
        public Character minionPrefab;
        public int minionCount = 1;
    }


}