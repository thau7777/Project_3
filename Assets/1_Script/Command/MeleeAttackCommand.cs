using System.Collections;
using UnityEngine;


namespace Turnbase
{
    public class MeleeAttackCommand : SkillCommand
    {
        private float moveSpeed = 5f;
        private float attackDuration = 0f;
        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        public MeleeAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            Vector3 initialPosition = user.initialPosition;
            float attackDistance = 1.5f;

            float direction = Mathf.Sign(target.transform.position.x - user.transform.position.x);
            Vector3 destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);

            user.animator.Play("Run");
            user.animator.SetBool("IsRunning", true);
            while (Vector3.Distance(user.transform.position, destination) > 0.1f)
            {
                user.transform.position = Vector3.MoveTowards(user.transform.position, destination, moveSpeed * Time.deltaTime);

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
            user.transform.position = destination;

            Vector3 finalLookDirection = (target.transform.position - user.transform.position).normalized;
            user.transform.rotation = Quaternion.LookRotation(new Vector3(finalLookDirection.x, 0, finalLookDirection.z));

            yield return null;

            attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;

            user.animator.SetTrigger("Attack");


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
            int finalDamage = Mathf.RoundToInt(rawDamage * damageMultiplier);

            if (rawDamage > 0)
            {
                finalDamage = Mathf.Max(1, finalDamage);
            }


            yield return new WaitForSeconds(attackDuration);

            target.TakeDamage(finalDamage);

            yield return new WaitForSeconds(0.2f);

            user.animator.SetBool("IsRunning", true);
            while (Vector3.Distance(user.transform.position, initialPosition) > 0.1f)
            {
                user.transform.position = Vector3.MoveTowards(user.transform.position, initialPosition, moveSpeed * Time.deltaTime);

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
            user.transform.position = initialPosition;

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

            battleManager.EndTurn(user);
        }
    }
}
