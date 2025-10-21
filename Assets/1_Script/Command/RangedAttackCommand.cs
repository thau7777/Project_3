using System;
using System.Collections;
using UnityEngine;


namespace Turnbase
{
    public class RangedAttackCommand : SkillCommand
    {
        private int finalDamage;
        private bool damageApplied = false;

        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        public RangedAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            Vector3 direction = (target.transform.position - user.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);

            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;

            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, lookRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = lookRotation;

            int offensiveStat = user.stats.attack;
            int defensiveStat = target.stats.defense;

            switch (skill.elementType)
            {
                case ElementType.Magical:
                case ElementType.Fire:
                case ElementType.Ice:
                case ElementType.Poison:
                case ElementType.Lightning:
                case ElementType.Holy:
                case ElementType.Dark:
                    offensiveStat = user.stats.magicAttack;
                    defensiveStat = target.stats.magicDefense;
                    break;

                case ElementType.Physical:
                case ElementType.None:
                default:
                    offensiveStat = user.stats.attack;
                    defensiveStat = target.stats.defense;
                    break;
            }

            int rawDamage = offensiveStat * skill.damage;

            float damageMultiplier = 100f / (defensiveStat + 100f);
            finalDamage = Mathf.RoundToInt(rawDamage * damageMultiplier);

            if (rawDamage > 0)
            {
                finalDamage = Mathf.Max(1, finalDamage);
            }

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    target.TakeDamage(finalDamage);
                    damageApplied = true;
                    SpawnImpactEffect(target.transform.position, skill);
                }
            };

            user.PrepareHitCallBack(hitAction);

            user.animator.Play(skill.animationTriggerName);

            while (!damageApplied)
            {
                yield return null;
            }

            float attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(attackDuration);

            elapsed = 0f;
            startRotation = user.transform.rotation;
            Quaternion endRotation = user.initialRotation;

            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = endRotation;

            battleManager.EndTurn(user);
        }

    }
}


