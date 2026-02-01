using UnityEngine;

namespace Turnbase
{
    // Chuyển từ MonoBehaviour sang SkillPassive (ScriptableObject)
    [CreateAssetMenu(fileName = "NewKillStreak", menuName = "Battle/Passive/Kill Streak")]
    public class Passive_KillStreak : SkillPassive
    {
        [Header("Cấu hình Skill")]
        public string skillName = "Sát Thủ";

        [Tooltip("Phần trăm Attack tăng thêm (0.15 = 15%)")]
        public float attackPercentBonus = 0.15f;

        public int durationTurns = 2;

        [Header("Visual")]
        public Sprite skillIcon;
        public Flyweight_TB buffVFX;

        // Override lại hàm OnKill từ class cha SkillPassive
        public override void OnKill(Character killer, Character victim)
        {
            if (killer == null || killer.stats == null || killer.buffManager == null)
                return;

            // 1. Tính toán giá trị buff dựa trên Physical Attack của killer
            int totalBuffAmount = Mathf.RoundToInt(killer.stats.physicalAttack * attackPercentBonus);

            // 2. Áp dụng Buff
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