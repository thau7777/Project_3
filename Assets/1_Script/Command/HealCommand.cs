using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Turnbase
{
    public class HealCommand : SkillCommand
    {
        private BattleManager battleManager;

        public HealCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            Debug.Log($"{user.name} dùng skill hồi máu!");

            yield return AnimateHealUser();

            List<Character> targetsToHeal = FindTargets();

            yield return ApplyHealEffects(targetsToHeal);

            yield return new WaitForSeconds(0.5f);

            battleManager.EndTurn(user);
        }

        private IEnumerator AnimateHealUser()
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

        private IEnumerator ApplyHealEffects(List<Character> targetsToHeal)
        {
            foreach (var charTarget in targetsToHeal)
            {
                charTarget.Heal(skill.damage);

                SpawnImpactEffect(charTarget.transform.position, skill);

                yield return null;
            }
        }
    }
}