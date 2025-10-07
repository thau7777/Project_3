using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffAttackCommand : SkillCommand
{
    private BattleManager battleManager;

    public BuffAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
        : base(user, target, skill)
    {
        this.battleManager = battleManager;
    }

    public override IEnumerator Execute()
    {
        Debug.Log($"{user.name} dùng skill Buff {skill.statToModify}!");
        user.animator.Play("Buff");
        yield return new WaitForSeconds(1.5f);

        // Khởi tạo danh sách mục tiêu
        var targetsToBuff = new List<Character>();

        int buffAmount = skill.damage; // Dùng skill.damage làm giá trị buff
        int buffDuration = skill.durationTurns;

        // Xác định mục tiêu
        if (skill.targetType == SkillTargetType.Ally || skill.targetType == SkillTargetType.Self)
        {
            if (target != null && target.isAlive)
            {
                targetsToBuff.Add(target);
            }
        }
        else if (skill.targetType == SkillTargetType.Allies)
        {
            targetsToBuff = battleManager.allCombatants
                .Where(c => c.isPlayer == user.isPlayer && c.isAlive)
                .ToList();
        }

        // Áp dụng Buff cho mục tiêu
        foreach (var charTarget in targetsToBuff)
        {
            // ✅ SỬ DỤNG SWITCH ĐỂ XỬ LÝ TẤT CẢ CÁC LOẠI STATTYPE
            switch (skill.statToModify)
            {
                case StatType.Attack:
                    charTarget.ApplyAttackBuff(buffAmount, buffDuration);
                    break;

                case StatType.MaxHP:
                    // Lưu ý: Bạn cần tạo hàm ApplyMaxHPBuff trong Character.cs
                    charTarget.ApplyMaxHPBuff(buffAmount, buffDuration);
                    break;

                case StatType.Defense:
                    // Lưu ý: Bạn cần tạo hàm ApplyDefenseBuff trong Character.cs
                    charTarget.ApplyDefenseBuff(buffAmount, buffDuration);
                    break;

                case StatType.Agility:
                    // Lưu ý: Bạn cần tạo hàm ApplyAgilityBuff trong Character.cs
                    charTarget.ApplyAgilityBuff(buffAmount, buffDuration);
                    break;

                // Thêm các StatType khác nếu có (e.g., StatType.Magic, StatType.CriticalRate)

                default:
                    Debug.LogWarning($"Skill '{skill.skillName}' có StatType là {skill.statToModify}. StatType này chưa được hỗ trợ trong BuffCommand.");
                    break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        battleManager.EndTurn(user);
    }
}