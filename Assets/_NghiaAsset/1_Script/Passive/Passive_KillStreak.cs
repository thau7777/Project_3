using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "NewKillStreak", menuName = "Battle/Passive/Kill Streak")]
    public class Passive_KillStreak : SkillPassive
    {
        [Tooltip("Phần trăm PhysicalAttack tăng thêm (0.15 = 15%)")]
        public float attackPercentBonus = 0.15f;

        public int durationTurns = 2;

        [Header("Visual")]
        public Sprite skillIcon;
        public Flyweight_TB buffVFX;

        public override void OnKill(Character killer, Character victim)
        {
            if (killer == null || killer.stats == null || killer.buffManager == null)
                return;

            int totalBuffAmount = Mathf.RoundToInt(killer.stats.physicalAttack * attackPercentBonus);

            killer.buffManager.ApplyAttackBuff(
                totalBuffAmount,
                durationTurns,
                buffVFX,
                skillIcon
            );

            Debug.Log($"<color=red>[PASSIVE]</color> {killer.name} hạ gục {victim.name}! " +
                      $"Tăng +{totalBuffAmount} ATK trong {durationTurns} lượt.");
        }
    }
}