using System;
using System.Collections;
using UnityEngine;


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
            CalculateFinalDamage();

            user.animator.Play(skill.animationTriggerName);

            float animationStartDelay = 0.5f;
            yield return new WaitForSeconds(animationStartDelay);

            if (skill.projectileSettings != null)
            {
                SpawnProjectile();

                while (!projectileHit)
                {
                    yield return null;
                }

                if (skill.impactVFXDuration > 0)
                {
                    yield return new WaitForSeconds(skill.impactVFXDuration);
                }
            }
            else
            {
                ApplyDamageInstant();
            }

            float attackDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(attackDuration);
        }

        private void SpawnProjectile()
        {
            Flyweight projectileInstance = FlyweightFactory.Spawn(skill.projectileSettings);

            if (projectileInstance != null)
            {
                projectileInstance.Initialize(user.transform.position, targetLookRotation);

                ProjectileTurnBase projectileScript = projectileInstance.GetComponent<ProjectileTurnBase>();

                if (projectileScript != null)
                {
                    Action hitCallback = () => { projectileHit = true; };

                    projectileScript.Setup(target, skill, finalDamage, hitCallback);
                }
                else
                {
                    Debug.LogError("Projectile prefab thiếu component ProjectileTurnBase.cs!");
                    projectileHit = true;
                }
            }
            else
            {
                Debug.LogError("Không thể spawn projectile. Kiểm tra FlyweightSettings và Factory.");
                projectileHit = true;
            }
        }

        private void ApplyDamageInstant()
        {
            target.TakeDamage(finalDamage);
            SpawnImpactEffect(target.transform.position, skill);
            projectileHit = true;
        }

        private void CalculateFinalDamage()
        {
            finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);
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