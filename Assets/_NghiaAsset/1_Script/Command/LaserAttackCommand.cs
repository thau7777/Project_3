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
        private bool damageApplied = false;

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
            damageApplied = false;

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    ApplyStatusEffectsAndStacks(user, target, skill);
                    target.TakeDamage(finalDamage, skill.elementType);

                    EventBusUI<CameraShakeEvent>.Raise(new CameraShakeEvent(0.2f, 0.4f));
                    SpawnImpactEffect(target.transform.position + Vector3.up * 1f, skill);

                    damageApplied = true;
                }
            };

            user.PrepareHitCallBack(hitAction);
            user.animator.Play(skill.animationTriggerName);

            

            Transform spawnPoint = user.SkillSpawnPoint != null ? user.SkillSpawnPoint : user.transform;
            Flyweight_TB laserFlyweight = null;

            if (skill.lazerSettings != null)
            {
                laserFlyweight = FlyweightFactory_TB.Spawn(skill.lazerSettings);

                if (laserFlyweight != null)
                {
                    laserFlyweight.Initialize(spawnPoint.position, spawnPoint.rotation);

                    laserFlyweight.transform.SetParent(spawnPoint);
                    laserFlyweight.transform.localPosition = Vector3.zero;
                    laserFlyweight.transform.localRotation = Quaternion.identity;

                    battleManager.StartCoroutine(UpdateLaserPositions(laserFlyweight.gameObject, spawnPoint, target.transform));
                }
            }

            float delay = 0f;
            if (skill.lazerSettings is OneShotVFXSettings_TB vfxSettings)
            {
                delay = vfxSettings.DespawnDelay;
            }

            if (delay > 0) yield return new WaitForSeconds(delay);

            float startTime = Time.time;
            float timeout = 2.0f;

            while (!damageApplied && Time.time < startTime + timeout)
            {
                yield return null;
            }

            if (!damageApplied) hitAction.Invoke();

            float laserDuration = skill.laserVFXDuration;
            yield return new WaitForSeconds(laserDuration);


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

        #region Rotation Logic (Giữ nguyên)
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