using System;
using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class LaserAttackCommand : SkillCommand
    {
        private BattleManager battleManager;
        private float rotationDuration = 0.25f;
        private Quaternion targetLookRotation;

        public LaserAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            targetLookRotation = GetTargetLookRotation();
            yield return RotateTowardsTarget();

            yield return PerformLaserAttack();

            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        private IEnumerator PerformLaserAttack()
        {
            int finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);

            user.animator.Play(skill.animationTriggerName);

            float animationLeadIn = 0.5f;
            yield return new WaitForSeconds(animationLeadIn);

            Transform spawnPoint = user.projectileSpawnPoint != null ? user.projectileSpawnPoint : user.transform;
            Vector3 startPos = user.projectileSpawnPoint != null ?
                               user.projectileSpawnPoint.position :
                               user.transform.position + Vector3.up * 1.2f;

            GameObject laserInstance = null;
            if (skill.projectileSettings != null && skill.projectileSettings.prefab != null)
            {
                laserInstance = UnityEngine.Object.Instantiate(skill.projectileSettings.prefab.gameObject);

                battleManager.StartCoroutine(UpdateLaserPositions(laserInstance, spawnPoint, target.transform));
            }

            ApplyStatusEffectsAndStacks(user, target, skill);
            target.TakeDamage(finalDamage, skill.elementType);

            EventBusUI<CameraShakeEvent>.Raise(new CameraShakeEvent(0.2f, 0.4f));

            SpawnImpactEffect(target.transform.position + Vector3.up * 1f, skill);

            float laserDuration = skill.impactVFXDuration > 0 ? skill.impactVFXDuration : 1.0f;
            yield return new WaitForSeconds(laserDuration);

            if (laserInstance != null) UnityEngine.Object.Destroy(laserInstance);
        }

        private IEnumerator UpdateLaserPositions(GameObject laser, Transform start, Transform end)
        {
            LineRenderer lr = laser.GetComponentInChildren<LineRenderer>();
            if (lr == null) yield break;

            float elapsed = 0;
            float duration = skill.impactVFXDuration > 0 ? skill.impactVFXDuration : 1.0f;

            while (elapsed < duration && laser != null)
            {
                lr.SetPosition(0, start.position);
                lr.SetPosition(1, end.position + Vector3.up * 1f);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        #region Rotation Logic (Giống ProjectileCommand)
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