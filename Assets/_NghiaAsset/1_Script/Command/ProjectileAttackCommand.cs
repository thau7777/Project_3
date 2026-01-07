using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;


namespace Turnbase
{
    public class ProjectileAttackCommand : SkillCommand
    {
        private int finalDamage;
        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        private Quaternion targetLookRotation;
        private bool projectileHit = false;

        public ProjectileAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            targetLookRotation = GetTargetLookRotation();

            yield return RotateTowardsTarget();

            yield return PerformProjectileAttack();

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

        private IEnumerator PerformProjectileAttack()
        {
            finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);

            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            float delayBetweenHits = skill.delayBetweenHits;

            int baseDamagePerHit = finalDamage / hits;
            int damageRemainder = finalDamage % hits;

            bool isProjectile = skill.projectileSettings != null;
            bool statusEffectsApplied = false;

            user.animator.Play(skill.animationTriggerName);

            float animationStartDelay = 0.5f;
            yield return new WaitForSeconds(animationStartDelay);

            for (int i = 0; i < hits; i++)
            {
                int currentHitDamage = baseDamagePerHit;
                if (i == hits - 1)
                {
                    currentHitDamage += damageRemainder;
                }

                if (i == 0 && !statusEffectsApplied)
                {
                    ApplyStatusEffectsAndStacks(user, target, skill);
                    statusEffectsApplied = true;
                }

                if (isProjectile)
                {
                    if (i == 0) 
                    {
                        projectileHit = false;
                        SpawnProjectile(currentHitDamage);

                        float startTime = Time.time;
                        float timeout = 5.0f;
                        while (!projectileHit && Time.time < startTime + timeout)
                        {
                            yield return null;
                        }

                        if (Time.time >= startTime + timeout)
                        {
                            Debug.LogError($"Projectile hit timed out for hit {i + 1}/{hits}. Forcing break.");
                            break;
                        }

                        if (skill.cameraTimeline != null && battleManager.mainDirector != null)
                        {
                            PlayableDirector director = battleManager.mainDirector;
                            director.playableAsset = skill.cameraTimeline;
                            director.Play();
                        }
                    }
                    else 
                    {
                        ApplySingleHitDamage(currentHitDamage);
                        SpawnImpactEffect(target.transform.position, skill);
                    }
                }
                else 
                {
                    ApplySingleHitDamage(currentHitDamage);
                    SpawnImpactEffect(target.transform.position, skill);
                }

                if (i < hits - 1)
                {
                    yield return new WaitForSeconds(delayBetweenHits);
                }
            }

            if (isProjectile && skill.impactVFXDuration > 0)
            {
                yield return new WaitForSeconds(skill.impactVFXDuration);
            }

            float attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(Mathf.Max(0.5f, attackDuration));
        }

        private void SpawnProjectile(int damageForThisHit)
        {
            Flyweight_TB projectileInstance = FlyweightFactory_TB.Spawn(skill.projectileSettings);
            ElementType element = skill.elementType;

            if (projectileInstance != null)
            {
                Vector3 spawnPosition;

                if (skill.useSkillSpawnPoint2 && user.SkillSpawnPoint2 != null)
                {
                    spawnPosition = user.SkillSpawnPoint2.position;
                }
                else if (user.SkillSpawnPoint != null)
                {
                    spawnPosition = user.SkillSpawnPoint.position;
                }
                else
                {
                    spawnPosition = user.transform.position + user.transform.forward * 0.5f + Vector3.up * 1f;
                }

                projectileInstance.Initialize(spawnPosition, targetLookRotation);

                ProjectileTurnBase projectileScript = projectileInstance.GetComponent<ProjectileTurnBase>();
                if (projectileScript != null)
                {
                    Action hitCallback = () => { projectileHit = true; };
                    projectileScript.Setup(target, skill, damageForThisHit, element, hitCallback);
                }
            }
        }


        private void ApplySingleHitDamage(int damage)
        {
            ElementType element = skill.elementType;
            target.TakeDamage(damage, element);
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