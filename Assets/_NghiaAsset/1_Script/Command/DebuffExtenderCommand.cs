using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turnbase
{
    public class DebuffExtenderCommand : SkillCommand
    {
        private int finalDamage;
        private bool damageApplied = false;
        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        private int extensionTurns = 2;

        public DebuffExtenderCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            yield return StartRotation();
            yield return PerformExtensionAttack();
            yield return RotateBackToInitial();
            battleManager.EndTurn(user);
        }

        private IEnumerator PerformExtensionAttack()
        {
            finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);

            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            int baseDamagePerHit = finalDamage / hits;
            int damageRemainder = finalDamage % hits;
            int firstHitDamage = baseDamagePerHit + damageRemainder;

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    ApplySingleHitDamageAndEffect(firstHitDamage);

                    ExtendTargetDebuffs(target.debuffManager);

                    damageApplied = true;
                }
            };

            if (skill.cameraTimeline != null && battleManager.mainDirector != null)
            {
                battleManager.mainDirector.playableAsset = skill.cameraTimeline;
                battleManager.mainDirector.Play();
            }

            user.PrepareHitCallBack(hitAction);
            user.animator.Play(skill.animationTriggerName);

            float startTime = Time.time;
            while (!damageApplied && Time.time < startTime + 2.0f)
            {
                yield return null;
            }

            if (!damageApplied) ApplySingleHitDamageAndEffect(firstHitDamage);

            for (int i = 1; i < hits; i++)
            {
                yield return new WaitForSeconds(skill.delayBetweenHits);
                ApplySingleHitDamage(baseDamagePerHit);
            }

            yield return WaitUntilAnimationEnds();
            user.animator.Play("Idle");
        }

        private void ExtendTargetDebuffs(CharacterDebuffManager dbf)
        {
            if (dbf == null) return;

            bool extended = false;

            if (dbf.burnTurnsRemaining > 0) { dbf.burnTurnsRemaining += extensionTurns; extended = true; }
            if (dbf.poisonTurnsRemaining > 0) { dbf.poisonTurnsRemaining += extensionTurns; extended = true; }
            if (dbf.stunTurnsRemaining > 0) { dbf.stunTurnsRemaining += extensionTurns; extended = true; }
            if (dbf.defReductionTurnsRemaining > 0) { dbf.defReductionTurnsRemaining += extensionTurns; extended = true; }
            if (dbf.speedReductionTurnsRemaining > 0) { dbf.speedReductionTurnsRemaining += extensionTurns; extended = true; }
            if (dbf.breakTurnsRemaining > 0) { dbf.breakTurnsRemaining += extensionTurns; extended = true; }
            if (dbf.paralysisTurnsRemaining > 0) { dbf.paralysisTurnsRemaining += extensionTurns; extended = true; }

            if (extended)
            {
                Debug.Log($"<color=yellow>Debuffs extended by {extensionTurns} turns!</color>");
            }
        }

        private void ApplySingleHitDamage(int damage)
        {
            target.TakeDamage(user, damage, skill.elementType);
        }

        private void ApplySingleHitDamageAndEffect(int damage)
        {
            target.TakeDamage(user, damage, skill.elementType);
            SpawnImpactEffect(target.transform.position, skill);
        }

        private IEnumerator StartRotation()
        {
            Vector3 direction = (target.transform.position - user.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            float elapsed = 0f;
            Quaternion startRot = user.transform.rotation;
            while (elapsed < rotationDuration)
            {
                user.transform.rotation = Quaternion.Slerp(startRot, lookRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            user.transform.rotation = lookRotation;
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

        private IEnumerator WaitUntilAnimationEnds()
        {
            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(skill.animationTriggerName))
            {
                float timeLeft = stateInfo.length * (1f - (stateInfo.normalizedTime % 1f));
                if (timeLeft > 0) yield return new WaitForSeconds(timeLeft);
            }
        }
    }
}