using System;
using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class MeleeAttackCommand : SkillCommand
    {
        private float moveSpeed = 30f;
        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        private bool damageApplied = false;
        private bool statusEffectsApplied = false;
        private Vector3 initialPosition;
        private Vector3 destination;

        public MeleeAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            user.parryMissCount = 0;
            statusEffectsApplied = false;

            initialPosition = user.initialPosition;
            int totalAttacks = skill.attackCount > 0 ? skill.attackCount : 1;
            user.totalHitsInSequence = totalAttacks * (skill.numberOfHits > 0 ? skill.numberOfHits : 1);
            user.currentHitInSequence = 0;

            bool isPerfectParry = true;

            for (int i = 0; i < totalAttacks; i++)
            {
                if (!target.isAlive) break;

                float attackDistance = 2f;
                float direction = Mathf.Sign(target.transform.position.x - user.transform.position.x);
                destination = target.transform.position - new Vector3(direction * attackDistance, 0, 0);
                yield return MoveToTarget(destination);

                target.isAttackBlocked = false;
                target.isParrySuccessful = false;
                user.isAttackBlocked = false;

                user.isLastHit = (i == totalAttacks - 1);
                
                yield return PerformAttack();

                if (target.isAttackBlocked)
                {
                    if (!target.isParrySuccessful)
                    {
                        isPerfectParry = false;
                    }

                    if (user.isLastHit)
                    {
                        if (target.isParrySuccessful && isPerfectParry)
                        {
                            if (user == null || !user.isAlive)
                            {
                                battleManager.isProcessingTurn = false;
                                if (battleManager.turnHandler != null) battleManager.turnHandler.isProcessingTurn = false;
                                battleManager.activeCharacter = null;
                                battleManager.CheckWaveCondition();
                                yield break;
                            }

                            if (user.stateMachine != null)
                                user.stateMachine.SwitchState(new InterruptedState(user.stateMachine));
                        }
                        else
                        {
                            Debug.Log("<color=orange>[INFO]</color> Không phản đòn vì không Parry chuẩn toàn bộ combo.");
                            user.animator.Play("Idle");
                            yield return new WaitForSeconds(0.3f);
                        }
                        break;
                    }
                    else
                    {
                        // Không gọi animation Hit/Hurt nếu không phải đòn cuối
                    }
                }
                else
                {
                    isPerfectParry = false; 

                    if (user.isLastHit)
                    {
                        user.animator.Play("Idle");
                    }
                }
            }

            yield return MoveBackToInitialPosition(initialPosition);
            yield return RotateBackToInitial();
            battleManager.EndTurn(user);
        }
        private IEnumerator PerformAttack()
        {
            damageApplied = false;

            Action hitAction = () =>
            {
                if (damageApplied) return;

                if (target.isAttackBlocked)
                {
                    // If blocked (evaded/parried), spawn at slot position but don't take damage
                    SpawnImpactEffect(target.initialPosition, skill);
                    SpawnMeleeEffect(target.initialPosition + Vector3.up * 1f, skill);
                    damageApplied = true;
                    return;
                }

                if (!statusEffectsApplied)
                {
                    ApplyStatusEffectsAndStacks(user, target, skill);
                    statusEffectsApplied = true;
                }

                int totalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);
                int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
                int baseDamagePerHit = totalDamage / hits;

                target.TakeDamage(user, baseDamagePerHit, skill.elementType); 
                SpawnImpactEffect(target.transform.position, skill);
                SpawnMeleeEffect(target.transform.position + Vector3.up * 1f, skill);
                damageApplied = true;
            };

            user.PrepareHitCallBack(hitAction);

            string animationToPlay = skill.animationTriggerName;
            if (user.isLastHit && !string.IsNullOrEmpty(skill.animationLastHitName))
            {
                animationToPlay = skill.animationLastHitName;
                Debug.Log("<color=red>[INFO]</color> Last Hit");
            }            
            user.animator.Play(animationToPlay, 0, 0f);

            while (!damageApplied) yield return null;

            int extraHits = (skill.numberOfHits > 0 ? skill.numberOfHits : 1) - 1;
            if (extraHits > 0)
            {
                int totalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);
                int baseDamagePerHit = totalDamage / (extraHits + 1);
                int damageRemainder = totalDamage % (extraHits + 1);

                for (int j = 0; j < extraHits; j++)
                {
                    if (!target.isAlive) yield break;

                    yield return new WaitForSeconds(skill.delayBetweenHits);

                    if (target.isAttackBlocked)
                    {
                        SpawnImpactEffect(target.initialPosition, skill);
                        SpawnMeleeEffect(target.initialPosition + Vector3.up * 1f, skill);
                        continue;
                    }

                    int currentHitDamage = baseDamagePerHit + (j == extraHits - 1 ? damageRemainder : 0);
                    target.TakeDamage(user, baseDamagePerHit, skill.elementType);
                    SpawnImpactEffect(target.transform.position, skill);
                    SpawnMeleeEffect(target.transform.position + Vector3.up * 1f, skill);
                }
            }

            yield return new WaitForSeconds(0.3f);
        }

        private IEnumerator MoveToTarget(Vector3 dest)
        {
            user.animator.SetBool("IsRunning", true);
            while (Vector3.Distance(user.transform.position, dest) > 0.1f)
            {
                user.transform.position = Vector3.MoveTowards(user.transform.position, dest, moveSpeed * Time.deltaTime);

                Vector3 dir = (target.transform.position - user.transform.position).normalized;
                if (dir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
                    user.transform.rotation = Quaternion.Slerp(user.transform.rotation, lookRot, Time.deltaTime * 15f);
                }
                yield return null;
            }
            user.animator.SetBool("IsRunning", false);
            user.transform.position = dest;
        }

        private IEnumerator MoveBackToInitialPosition(Vector3 initialPos)
        {
            user.animator.SetBool("IsRunning", true);

            bool isPlayer = user.CompareTag("Player");

            while (Vector3.Distance(user.transform.position, initialPos) > 0.1f)
            {
                user.transform.position = Vector3.MoveTowards(user.transform.position, initialPos, moveSpeed * Time.deltaTime);
                if (!isPlayer)
                {
                    Vector3 dir = (initialPos - user.transform.position).normalized;
                    if (dir != Vector3.zero)
                    {
                        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
                        user.transform.rotation = Quaternion.Slerp(user.transform.rotation, lookRot, Time.deltaTime * 15f);
                    }
                }

                yield return null;
            }

            user.animator.SetBool("IsRunning", false);
            user.transform.position = initialPos;
        }

        private IEnumerator RotateBackToInitial()
        {
            float elapsed = 0f;
            Quaternion startRot = user.transform.rotation;
            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRot, user.initialRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = user.initialRotation;
        }
    }
}