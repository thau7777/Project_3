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

        private Quaternion targetLookRotation;


        public RangedAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            targetLookRotation = GetTargetLookRotation();

            yield return PerformRangedAttack();

            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        private Quaternion GetTargetLookRotation()
        {
            Vector3 direction = (target.transform.position - user.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);
            return lookRotation;
        }

        private IEnumerator PerformRangedAttack()
        {
            int offensiveStat;
            int defensiveStat;

            switch (skill.elementType)
            {
                case ElementType.Magical:
                case ElementType.Fire:
                case ElementType.Ice:
                case ElementType.Poison:
                case ElementType.Lightning:
                case ElementType.Dark:
                    offensiveStat = user.stats.magicAttack;
                    defensiveStat = target.stats.magicDefense;
                    break;

                case ElementType.Physical:
                case ElementType.None:
                default:
                    offensiveStat = user.stats.physicalAttack;
                    defensiveStat = target.stats.physicalDefense;
                    break;
            }

            int rawDamage = offensiveStat * skill.damage;

            float defenseMultiplier = 100f / (defensiveStat + 100f);

            float damageBase = rawDamage * defenseMultiplier;

            float elementMultiplier = GetElementMultiplier();

            float PreCritDamageFloat = damageBase * elementMultiplier;

            bool isCrit = UnityEngine.Random.Range(0, 100) < user.stats.crit;

            if(isCrit)
            {
                float critMultiplier = (float)user.stats.critDamage / 100f;
                PreCritDamageFloat *= critMultiplier;
            }


            finalDamage = Mathf.RoundToInt(PreCritDamageFloat);

            if (rawDamage > 0)
            {
                finalDamage = Mathf.Max(1, finalDamage);
            }

            damageApplied = false;
            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    target.TakeDamage(finalDamage);
                    damageApplied = true;
                    SpawnImpactEffect(target.transform.position, skill);

                    if (skill.debuffProperties.debuffType != DebuffType.None)
                    {
                        target.debuffManager.ApplyDebuff(skill.debuffProperties);
                    }
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
        }

        private float GetElementMultiplier()
        {
            if (battleManager != null && battleManager.elementChart != null)
            {
                return battleManager.elementChart.GetMultiplier(skill.elementType, target.characterElement);
            }

            return 1.0f;
        }

        private IEnumerator RotateBackToInitial()
        {
            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;
            Quaternion endRotation = user.initialRotation;

            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = endRotation;
        }
    }
}