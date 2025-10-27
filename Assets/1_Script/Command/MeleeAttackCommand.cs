using System.Collections;
using UnityEngine;
using System;


namespace Turnbase
{
    public class MeleeAttackCommand : SkillCommand
    {
        private float moveSpeed = 30f;
        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        private bool damageApplied = false;
        private int finalDamage;

        private Vector3 initialPosition;
        private Vector3 destination;

        public MeleeAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            initialPosition = user.initialPosition;
            float attackDistance = 1.5f;
            float direction = Mathf.Sign(target.transform.position.x - user.transform.position.x);
            destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);

            yield return MoveToTarget(destination);
            yield return PerformAttack();
            yield return MoveBackToInitialPosition(initialPosition);
            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }
        private IEnumerator MoveToTarget(Vector3 dest)
        {
            user.animator.SetBool("IsRunning", true);
            while (Vector3.Distance(user.transform.position, dest) > 0.1f)
            {
                user.transform.position = Vector3.MoveTowards(user.transform.position, dest, moveSpeed * Time.deltaTime);

                Vector3 lookDirection = (target.transform.position - user.transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));

                user.transform.rotation = Quaternion.Slerp(
                    user.transform.rotation,
                    targetRotation,
                    Time.deltaTime * (1f / rotationDuration) * 5f
                );

                yield return null;
            }

            user.animator.SetBool("IsRunning", false);
            user.transform.position = dest;

            Vector3 finalLookDirection = (target.transform.position - user.transform.position).normalized;
            user.transform.rotation = Quaternion.LookRotation(new Vector3(finalLookDirection.x, 0, finalLookDirection.z));

            yield return null;
        }

        private IEnumerator PerformAttack()
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
                    offensiveStat = user.stats.attack;
                    defensiveStat = target.stats.defense;
                    break;
            }

            int rawDamage = offensiveStat * skill.damage;
            float defenseMultiplier = 100f / (defensiveStat + 100f);

            float damageBase = rawDamage * defenseMultiplier;

            float elementMultiplier = GetElementMultiplier();

            finalDamage = Mathf.RoundToInt(damageBase * elementMultiplier);

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

                    SpawnImpactEffect(target.transform.position, skill);

                    damageApplied = true;
                }
            };

            user.PrepareHitCallBack(hitAction);

            user.animator.Play("Attack");

            while (!damageApplied)
            {
                yield return null;
            }

            float calculatedDuration = 0.5f;
            AnimatorStateInfo stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            if (user.animator.HasState(0, Animator.StringToHash("Attack")))
            {
                calculatedDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            }

            yield return new WaitForSeconds(calculatedDuration);
            yield return new WaitForSeconds(0.2f);
        }

        private float GetElementMultiplier()
        {
            if (battleManager != null && battleManager.elementChart != null)
            {
                return battleManager.elementChart.GetMultiplier(skill.elementType, target.characterElement);
            }

            return 1.0f;
        }

        private IEnumerator MoveBackToInitialPosition(Vector3 initialPos)
        {
            user.animator.SetBool("IsRunning", true);
            while (Vector3.Distance(user.transform.position, initialPos) > 0.1f)
            {
                user.transform.position = Vector3.MoveTowards(user.transform.position, initialPos, moveSpeed * Time.deltaTime);

                Vector3 returnLookDirection = (target.transform.position - user.transform.position).normalized;
                Quaternion returnRotation = Quaternion.LookRotation(new Vector3(returnLookDirection.x, 0, returnLookDirection.z));

                user.transform.rotation = Quaternion.Slerp(
                    user.transform.rotation,
                    returnRotation,
                    Time.deltaTime * (1f / rotationDuration) * 5f
                );

                yield return null;
            }
            user.animator.SetBool("IsRunning", false);
            user.transform.position = initialPos;
        }

        private IEnumerator RotateBackToInitial()
        {
            Quaternion startRotation = user.transform.rotation;
            Quaternion endRotation = user.initialRotation;

            float elapsed = 0f;
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