using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;


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
            float attackDistance = 2f;
            float direction = Mathf.Sign(target.transform.position.x - user.transform.position.x);
            destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);

            yield return MoveToTarget(destination);

            int totalAttacks = skill.attackCount > 0 ? skill.attackCount : 1;

            for (int i = 0; i < totalAttacks; i++)
            {
                if (!target.isAlive) break;

                yield return PerformAttack();

                if (i < totalAttacks - 1 && target.isAlive)
                {
                    if (target.isAttackBlocked)
                    {
                        user.animator.Play("Hit", 0, 0f);
                        yield return new WaitForSeconds(0.6f); 
                        target.isAttackBlocked = false; 
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.2f);
                    }

                    user.animator.Play("Idle");
                    yield return new WaitForSeconds(0.1f);
                }
            }

            yield return MoveBackToInitialPosition(initialPosition);
            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        private IEnumerator PerformAttack()
        {
            ApplyStatusEffectsAndStacks(user, target, skill);

            ElementType element = skill.elementType;
            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            float delayBetweenHits = skill.delayBetweenHits;

            int totalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);
            int baseDamagePerHit = totalDamage / hits;
            int damageRemainder = totalDamage % hits;

            damageApplied = false;

            Action hitAction = () =>
            {
                if (damageApplied) return;

                if (target.isAttackBlocked)
                {
                    damageApplied = true;
                    return;
                }

                target.TakeDamage(baseDamagePerHit, element);
                SpawnImpactEffect(target.transform.position, skill);
                damageApplied = true;

                if (skill.meleeSettings != null)
                {
                    Flyweight_TB effectInstance = FlyweightFactory_TB.Spawn(skill.meleeSettings);
                    if (effectInstance != null)
                    {
                        Vector3 spawnPos = user.SkillSpawnPoint != null ? user.SkillSpawnPoint.position : user.transform.position;
                        effectInstance.Initialize(spawnPos, user.transform.rotation);
                    }
                }
            };

            user.PrepareHitCallBack(hitAction);
            user.animator.Play(skill.animationTriggerName, 0, 0f);

            while (!damageApplied)
            {
                yield return null;
            }

            if (target.isAttackBlocked) yield break;

            for (int i = 1; i < hits; i++)
            {
                if (!target.isAlive) yield break;
                yield return new WaitForSeconds(delayBetweenHits);

                int currentHitDamage = baseDamagePerHit + (i == hits - 1 ? damageRemainder : 0);
                target.TakeDamage(currentHitDamage, element);
                SpawnImpactEffect(target.transform.position, skill);
            }

            yield return new WaitForSeconds(0.3f);
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