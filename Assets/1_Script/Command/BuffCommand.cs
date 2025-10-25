using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


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

            yield return AnimateBuffUser();

            List<Character> targetsToBuff = FindTargets();

            ApplyBuffEffects(targetsToBuff);

            yield return new WaitForSeconds(0.5f);

            battleManager.EndTurn(user);
        }

        private IEnumerator AnimateBuffUser()
        {
            user.animator.Play(skill.animationTriggerName);
            yield return new WaitForSeconds(1.5f);
        }

        private List<Character> FindTargets()
        {
            if (skill.targetType == SkillTargetType.Ally || skill.targetType == SkillTargetType.Self)
            {
                if (target != null && target.isAlive)
                {
                    return new List<Character> { target };
                }
            }
            else if (skill.targetType == SkillTargetType.Allies)
            {
                return battleManager.allCombatants
                    .Where(c => c.isPlayer == user.isPlayer && c.isAlive)
                    .ToList();
            }
            return new List<Character>();
        }

        private void ApplyBuffEffects(List<Character> targetsToBuff)
        {
            int buffAmount = skill.damage;
            int buffDuration = skill.durationTurns;

            foreach (var charTarget in targetsToBuff)
            {
                GameObject activeVFX = SpawnContinuousEffect(charTarget.transform.position, charTarget, skill);

                switch (skill.statToModify)
                {
                    case StatType.Attack:
                        charTarget.ApplyAttackBuff(buffAmount, buffDuration, activeVFX);
                        break;

                    case StatType.MaxHP:
                        charTarget.ApplyMaxHPBuff(buffAmount, buffDuration, activeVFX);
                        break;

                    case StatType.Defense:
                        charTarget.ApplyDefenseBuff(buffAmount, buffDuration, activeVFX);
                        break;

                    case StatType.Agility:
                        charTarget.ApplyAgilityBuff(buffAmount, buffDuration, activeVFX);
                        break;

                    case StatType.MagicalAttack:
                        charTarget.ApplyMagicAttackBuff(buffAmount, buffDuration, activeVFX);
                        break;

                    case StatType.MagicalDefense:
                        charTarget.ApplyMagicDefenseBuff(buffAmount, buffDuration, activeVFX);
                        break;

                    default:
                        Debug.LogWarning($"Skill '{skill.skillName}' có StatType là {skill.statToModify}. StatType này chưa được hỗ trợ trong BuffCommand.");
                        break;
                }
            }
        }
    }
}