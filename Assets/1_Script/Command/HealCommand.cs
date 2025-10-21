using System.Collections;
using UnityEngine;
using System.Linq;


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
            user.animator.Play(skill.animationTriggerName);
            yield return new WaitForSeconds(1.5f);

            var targetsToHeal = new System.Collections.Generic.List<Character>();

            if (skill.targetType == SkillTargetType.Ally || skill.targetType == SkillTargetType.Self)
            {
                if (target != null && target.isAlive)
                {
                    targetsToHeal.Add(target);
                }
            }
            else if (skill.targetType == SkillTargetType.Allies)
            {
                targetsToHeal = battleManager.allCombatants
                    .Where(c => c.isPlayer == user.isPlayer && c.isAlive)
                    .ToList();
            }

            foreach (var charTarget in targetsToHeal)
            {
                charTarget.Heal(skill.damage);

                SpawnImpactEffect(charTarget.transform.position, skill);
            }

            yield return new WaitForSeconds(0.5f);

        }
    }

}

