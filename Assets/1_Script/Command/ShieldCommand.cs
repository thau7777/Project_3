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

            yield return AnimateShieldUser();

            List<Character> targetsToShield = FindTargets();

            yield return ApplyShieldEffects(targetsToShield);

            yield return new WaitForSeconds(0.5f);

            battleManager.EndTurn(user);
        }

        private IEnumerator AnimateShieldUser()
        {
            user.animator.Play(skill.animationTriggerName);
            yield return new WaitForSeconds(1f);
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

        private IEnumerator ApplyShieldEffects(List<Character> targetsToShield)
        {
            int shieldAmount = skill.damage;

            foreach (var charTarget in targetsToShield)
            {
                charTarget.AddShield(shieldAmount);

                SpawnImpactEffect(charTarget.transform.position, skill);

                Debug.Log($"{charTarget.name} đã nhận {shieldAmount} Shield.");

                yield return null;
            }
        }
    }
}