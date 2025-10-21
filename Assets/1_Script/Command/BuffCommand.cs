using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Turnbase
{
    public class BuffCommand : SkillCommand
    {
        private BattleManager battleManager;

        public BuffCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            Debug.Log($"{user.name} dùng skill Buff {skill.statToModify}!");

            user.animator.Play(skill.animationTriggerName);

            yield return new WaitForSeconds(1.5f);

            var targetsToBuff = new List<Character>();

            int buffAmount = skill.damage;
            int buffDuration = skill.durationTurns;

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

            foreach (var charTarget in targetsToBuff)
            {
                switch (skill.statToModify)
                {
                    case StatType.Attack:
                        charTarget.ApplyAttackBuff(buffAmount, buffDuration);
                        break;

                    case StatType.MaxHP:
                        charTarget.ApplyMaxHPBuff(buffAmount, buffDuration);
                        break;

                    case StatType.Defense:
                        charTarget.ApplyDefenseBuff(buffAmount, buffDuration);
                        break;

                    case StatType.Agility:
                        charTarget.ApplyAgilityBuff(buffAmount, buffDuration);
                        break;


                    default:
                        Debug.LogWarning($"Skill '{skill.skillName}' có StatType là {skill.statToModify}. StatType này chưa được hỗ trợ trong BuffCommand.");
                        break;
                }

                SpawnImpactEffect(charTarget.transform.position, skill);

            }

            yield return new WaitForSeconds(0.5f);

            battleManager.EndTurn(user);
        }
    }

}
