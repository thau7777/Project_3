using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;


namespace Turnbase
{
    public class RangedAttackCommand : SkillCommand
    {
        private int finalDamage;
        private bool damageApplied = false;

        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        private Quaternion targetLookRotation;


        public RangedAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            user.parryMissCount = 0;
            targetLookRotation = GetTargetLookRotation();

            yield return PerformRangedAttack();

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

        private void ApplySingleHitDamage(int damage)
        {
            if (target.isAttackBlocked)
            {
                SpawnImpactEffect(target.initialPosition, skill);
                return;
            }
            ElementType element = skill.elementType;
            target.TakeDamage(user, damage, element);
        }

        private void ApplySingleHitDamageAndEffect(int damage)
        {
            if (target.isAttackBlocked)
            {
                SpawnImpactEffect(target.initialPosition, skill);
                return;
            }
            ElementType element = skill.elementType;
            target.TakeDamage(user, damage, element);
            SpawnImpactEffect(target.transform.position, skill);
        }

        private IEnumerator PerformRangedAttack()
        {
            ElementType element = skill.elementType;

            finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);

            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            user.totalHitsInSequence = hits;
            user.currentHitInSequence = 0;
            float delayBetweenHits = skill.delayBetweenHits;

            int baseDamagePerHit = finalDamage / hits;
            int damageRemainder = finalDamage % hits;


            int firstHitDamage = baseDamagePerHit;
            if (hits == 1)
            {
                firstHitDamage += damageRemainder;
            }


            ApplyStatusEffectsAndStacks(user, target, skill);

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    ApplySingleHitDamageAndEffect(firstHitDamage);
                    damageApplied = true;
                }
            };


            if (skill.cameraTimeline != null && battleManager.mainDirector != null)
            {
                PlayableDirector director = battleManager.mainDirector;
                director.playableAsset = skill.cameraTimeline;

                director.Play();
            }


            user.PrepareHitCallBack(hitAction);

            user.isLastHit = true;
            user.animator.Play(skill.animationTriggerName);

            float startTime = Time.time;
            float timeout = 2.0f;


            while (!damageApplied && Time.time < startTime + timeout)
            {
                yield return null;
            }

            if (!damageApplied)
            {
                Debug.LogWarning("First hit callback failed or timed out. Forcing damage.");
                ApplySingleHitDamageAndEffect(firstHitDamage);
            }



            for (int i = 1; i < hits; i++)
            {
                yield return new WaitForSeconds(delayBetweenHits);

                int currentHitDamage = baseDamagePerHit;

                if (i == hits - 1)
                {
                    currentHitDamage += damageRemainder;
                }


                ApplySingleHitDamage(currentHitDamage);
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