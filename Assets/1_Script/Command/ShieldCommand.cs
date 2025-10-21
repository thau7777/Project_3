using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Turnbase
{
    public class ShieldCommand : SkillCommand
    {
        private BattleManager battleManager;

        public ShieldCommand(Character user, Character target, Skill skill, BattleManager battleManager) : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            Debug.Log($"{user.name} dung skill lá chắn ");

            user.animator.Play(skill.animationTriggerName);

            yield return new WaitForSeconds(1f);


            var targetsToShield = new List<Character>();

            int shieldAmount = skill.damage;

            if (skill.targetType == SkillTargetType.Ally || skill.targetType == SkillTargetType.Self)
            {
                if (target != null && target.isAlive)
                {
                    targetsToShield.Add(target);
                }
            }
            else if (skill.targetType == SkillTargetType.Allies)
            {
                targetsToShield = battleManager.allCombatants
                    .Where(c => c.isPlayer == user.isPlayer && c.isAlive)
                    .ToList();
            }


            foreach (var charTarget in targetsToShield)
            {
                charTarget.AddShield(shieldAmount);

                SpawnImpactEffect(charTarget.transform.position, skill);

                Debug.Log($"{charTarget.name} đã nhận {shieldAmount} Shield.");
            }

            yield return new WaitForSeconds(0.5f);

            battleManager.EndTurn(user);

        }

    }

}

