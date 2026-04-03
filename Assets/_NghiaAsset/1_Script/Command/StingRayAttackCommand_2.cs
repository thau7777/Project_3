using System;
using System.Collections;
using UnityEngine;
using PixPlays.ElementalVFX;

namespace Turnbase
{
    public class StingRayAttackCommand_2 : SkillCommand
    {
        private BattleManager battleManager;
        private float rotationDuration = 0.25f;
        private Quaternion targetLookRotation;
        private bool projectileHit = false;

        public StingRayAttackCommand_2(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            user.parryMissCount = 0;
            targetLookRotation = GetTargetLookRotation();
            yield return RotateTowardsTarget();

            yield return PerformProjectileAttack();

            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        private IEnumerator PerformProjectileAttack()
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

            projectileHit = false;
            bool effectTriggered = false;

            Action spawnAction = () =>
            {
                if (effectTriggered) return;

                Transform spawnPoint = user.SkillSpawnPoint != null ? user.SkillSpawnPoint : user.transform;
                if (skill.projectileSettings != null)
                {
                    Flyweight_TB vfxFlyweight = FlyweightFactory_TB.Spawn(skill.projectileSettings);
                    if (vfxFlyweight != null)
                    {
                        vfxFlyweight.Initialize(spawnPoint.position, spawnPoint.rotation);
                        vfxFlyweight.transform.SetParent(null);

                        ProjectileVfx projectileVfx = vfxFlyweight.GetComponent<ProjectileVfx>();
                        if (projectileVfx != null)
                        {
                            // We use the target's position for the projectile destination
                            Vector3 targetPos = target.initialPosition + Vector3.up * 1f;

                            // Calculate approximate flight time.
                            // ProjectileVfx has its own _flightDuration, but VfxData also takes a duration.
                            // BaseVfx uses the VfxData duration for ScheduleStop.
                            float flightTime = 1.0f; // Default flight time.
                            
                            var vfxData = new VfxData(spawnPoint.position, targetPos, flightTime, 0.2f, Vector3.zero);
                            projectileVfx.Play(vfxData);

                            // In ProjectileVfx, the projectile move logic is inside Coroutine_Projectile.
                            // We'll wait based on the estimated flight time before applying damage.
                            battleManager.StartCoroutine(WaitAndHit(flightTime));
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

            // Wait for projectile hit (damage applied)
            float startTime = Time.time;
            float timeout = 4.0f;
            while (!projectileHit && Time.time < startTime + timeout)
            {
                yield return null;
            }

            // Hit logic
            for (int i = 0; i < hits; i++)
            {
                if (!target.isAlive) break;

                if (i > 0) yield return new WaitForSeconds(delayBetweenHits);

                if (target.isAttackBlocked)
                {
                    SpawnImpactEffect(target.initialPosition, skill);
                    continue;
                }

                if (i == 0) ApplyStatusEffectsAndStacks(user, target, skill);

                int currentHitDamage = baseDamagePerHit + (i == hits - 1 ? damageRemainder : 0);
                target.TakeDamage(user, currentHitDamage, skill.elementType);
                EventBusUI<CameraShakeEvent>.Raise(new CameraShakeEvent(0.15f, 0.3f));
                
                // Spawn impact VFX for each hit
                SpawnImpactEffect(target.initialPosition + Vector3.up * 1f, skill);
            }

            // Wait for animation to finish
            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(skill.animationTriggerName))
            {
                float timeLeft = stateInfo.length * (1f - (stateInfo.normalizedTime % 1f));
                if (timeLeft > 0) yield return new WaitForSeconds(timeLeft);
            }

            user.animator.Play("Idle");
        }

        private IEnumerator WaitAndHit(float duration)
        {
            yield return new WaitForSeconds(duration);
            projectileHit = true;
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