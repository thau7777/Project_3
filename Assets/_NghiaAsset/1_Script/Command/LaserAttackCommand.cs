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
                    SpawnImpactEffect(target.transform.position + Vector3.up * 1f, skill);

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
                    Flyweight_TB laserFlyweight = FlyweightFactory_TB.Spawn(skill.lazerSettings);
                    if (laserFlyweight != null)
                    {
                        laserFlyweight.Initialize(spawnPoint.position, spawnPoint.rotation);
                        laserFlyweight.transform.SetParent(spawnPoint);
                        laserFlyweight.transform.localPosition = Vector3.zero;
                        laserFlyweight.transform.localRotation = Quaternion.identity;

                        battleManager.StartCoroutine(UpdateLaserPositions(laserFlyweight.gameObject, spawnPoint, target.transform));
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

            yield return null;

            float startTime = Time.time;
            float timeout = 2.5f;
            while (!damageApplied && Time.time < startTime + timeout)
            {
                yield return null;
            }

            if (!damageApplied) hitAction.Invoke();

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
                SpawnImpactEffect(target.transform.position + Vector3.up * 1f, skill);
            }

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

            user.animator.Play("Idle");
        }

        private IEnumerator UpdateLaserPositions(GameObject laser, Transform start, Transform end)
        {
            LineRenderer lr = laser.GetComponentInChildren<LineRenderer>();
            if (lr == null) yield break;

            float elapsed = 0;
            float duration = skill.impactVFXDuration > 0 ? skill.impactVFXDuration : 1.0f;

            while (elapsed < duration && laser != null)
            {
                if (start != null && end != null)
                {
                    lr.SetPosition(0, start.position);
                    lr.SetPosition(1, end.position + Vector3.up * 1f);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        #region Rotation 
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