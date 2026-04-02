using System;
using System.Collections;
using UnityEngine;
using PixPlays.ElementalVFX;

namespace Turnbase
{
    public class StingRayAttackCommand : SkillCommand
    {
        private BattleManager battleManager;
        private float rotationDuration = 0.25f;
        private Quaternion targetLookRotation;
        private bool damageApplied = false;
        private BeamVfx spawnedBeam;

        public StingRayAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            user.parryMissCount = 0;
            targetLookRotation = GetTargetLookRotation();
            yield return RotateTowardsTarget();

            yield return PerformBeamAttack();

            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        private IEnumerator PerformBeamAttack()
        {
            if (skill.cameraTimeline != null && battleManager.mainDirector != null)
            {
                battleManager.mainDirector.playableAsset = skill.cameraTimeline;
                battleManager.mainDirector.Play();
            }

            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            user.totalHitsInSequence = hits;
            user.currentHitInSequence = 0;
            int totalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);
            int baseDamagePerHit = totalDamage / hits;
            int damageRemainder = totalDamage % hits;
            float delayBetweenHits = skill.delayBetweenHits;

            damageApplied = false;
            bool effectTriggered = false;

            // Create a temporary anchor to allow the beam to "sweep up"
            GameObject targetAnchor = new GameObject("StingRay_TargetAnchor");
            Vector3 finalTargetPos = target.initialPosition + Vector3.up * 1f;
            Vector3 startTargetPos = target.initialPosition + Vector3.down * 4f; // Start below feet
            targetAnchor.transform.position = startTargetPos;

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    if (target.isAttackBlocked)
                    {
                        SpawnImpactEffect(target.initialPosition, skill);
                        damageApplied = true;
                        return;
                    }
                    ApplyStatusEffectsAndStacks(user, target, skill);

                    target.TakeDamage(user, baseDamagePerHit, skill.elementType);
                    EventBusUI<CameraShakeEvent>.Raise(new CameraShakeEvent(0.2f, 0.4f));
                    SpawnImpactEffect(targetAnchor.transform.position, skill);

                    damageApplied = true;
                }
            };
            user.PrepareHitCallBack(hitAction);

            Action spawnAction = () =>
            {
                if (effectTriggered) return;

                Transform spawnPoint = user.SkillSpawnPoint != null ? user.SkillSpawnPoint : user.transform;
                if (skill.lazerSettings != null)
                {
                    Flyweight_TB vfxFlyweight = FlyweightFactory_TB.Spawn(skill.lazerSettings);
                    if (vfxFlyweight != null)
                    {
                        vfxFlyweight.Initialize(spawnPoint.position, spawnPoint.rotation);
                        vfxFlyweight.transform.SetParent(null);

                        spawnedBeam = vfxFlyweight.GetComponent<BeamVfx>();
                        if (spawnedBeam != null)
                        {
                            var vfxData = new VfxData(spawnPoint, targetAnchor.transform, skill.laserVFXDuration, 0f, Vector3.zero);
                            spawnedBeam.Play(vfxData);
                            
                            // Start sweeping up
                            battleManager.StartCoroutine(SweepAnchorUp(targetAnchor.transform, startTargetPos, finalTargetPos, 0.5f));
                        }
                    }
                }
                effectTriggered = true;
            };

            if (user.TryGetComponent(out CharacterAnimationDispatcher dispatcher))
            {
                dispatcher.SetSpawnCallback(spawnAction);
            }
            else
            {
                spawnAction.Invoke();
            }

            user.isLastHit = true;
            user.animator.Play(skill.animationTriggerName);

            // Wait for first hit (damage applied callback)
            float startTime = Time.time;
            float timeout = 2.5f;
            while (!damageApplied && Time.time < startTime + timeout)
            {
                yield return null;
            }

            if (!damageApplied) hitAction.Invoke();

            // Multi-hit logic
            for (int i = 1; i < hits; i++)
            {
                if (!target.isAlive)
                    break;

                yield return new WaitForSeconds(delayBetweenHits);

                if (target.isAttackBlocked)
                {
                    SpawnImpactEffect(target.initialPosition, skill);
                    continue;
                }

                int currentHitDamage = baseDamagePerHit + (i == hits - 1 ? damageRemainder : 0);
                target.TakeDamage(user, currentHitDamage, skill.elementType);
                EventBusUI<CameraShakeEvent>.Raise(new CameraShakeEvent(0.1f, 0.2f));
                SpawnImpactEffect(targetAnchor.transform.position, skill);
            }

            // Wait for animation to finish
            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(skill.animationTriggerName))
            {
                float normalizedTime = stateInfo.normalizedTime % 1f;
                float timeLeft = stateInfo.length * (1f - normalizedTime);

                if (timeLeft > 0)
                {
                    yield return new WaitForSeconds(timeLeft);
                }
            }

            if (spawnedBeam != null)
            {
                spawnedBeam.Stop();
            }

            GameObject.Destroy(targetAnchor, 1f); // Destroy anchor after a small delay

            user.animator.Play("Idle");
        }

        private IEnumerator SweepAnchorUp(Transform anchor, Vector3 start, Vector3 end, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration && anchor != null)
            {
                anchor.position = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (anchor != null) anchor.position = end;
        }

        #region Rotation 
        private Quaternion GetTargetLookRotation()
        {
            Vector3 direction = (target.initialPosition - user.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);
            return lookRotation;
        }

        private IEnumerator RotateTowardsTarget()
        {
            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;
            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, targetLookRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = targetLookRotation;
        }

        private IEnumerator RotateBackToInitial()
        {
            float elapsed = 0f;
            Quaternion startRotation = user.transform.rotation;
            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRotation, user.initialRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = user.initialRotation;
        }
        #endregion
    }
}
