using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Turnbase
{
    public class DebuffPunisherCommand : SkillCommand
    {
        private int finalDamage;
        private bool damageApplied = false;
        private float rotationDuration = 0.25f;
        private BattleManager battleManager;

        public DebuffPunisherCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            yield return StartRotation();

            yield return PerformPunisherAttack();

            yield return RotateBackToInitial();

            battleManager.EndTurn(user);
        }

        private IEnumerator PerformPunisherAttack()
        {
            bool hasDebuff = CheckForAnyDebuff(target.debuffManager);

            finalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);

            if (hasDebuff)
            {
                finalDamage *= 2;
                Debug.Log("<color=red>Debuff Punisher: X2 Damage Activated!</color>");
            }

            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            int baseDamagePerHit = finalDamage / hits;
            int damageRemainder = finalDamage % hits;
            int firstHitDamage = baseDamagePerHit + damageRemainder;

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    ApplySingleHitDamageAndEffect(firstHitDamage);

                    if (hasDebuff)
                    {
                        CleanseOneRandomDebuff(target.debuffManager);
                    }

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

        private bool CheckForAnyDebuff(CharacterDebuffManager dbf)
        {
            if (dbf == null) return false;
            return dbf.burnTurnsRemaining > 0 ||
                   dbf.poisonTurnsRemaining > 0 ||
                   dbf.stunTurnsRemaining > 0 ||
                   dbf.defReductionTurnsRemaining > 0 ||
                   dbf.speedReductionTurnsRemaining > 0 ||
                   dbf.breakTurnsRemaining > 0 ||
                   dbf.paralysisTurnsRemaining > 0;
        }

        private void CleanseOneRandomDebuff(CharacterDebuffManager dbf)
        {
            List<Action> cleansePool = new List<Action>();

            if (dbf.burnTurnsRemaining > 0) cleansePool.Add(() => dbf.burnTurnsRemaining = 0);
            if (dbf.poisonTurnsRemaining > 0) cleansePool.Add(() => dbf.poisonTurnsRemaining = 0);
            if (dbf.stunTurnsRemaining > 0) cleansePool.Add(() => dbf.stunTurnsRemaining = 0);
            if (dbf.defReductionTurnsRemaining > 0) cleansePool.Add(() => dbf.defReductionTurnsRemaining = 0);
            if (dbf.speedReductionTurnsRemaining > 0) cleansePool.Add(() => dbf.speedReductionTurnsRemaining = 0);
            if (dbf.breakTurnsRemaining > 0) cleansePool.Add(() => dbf.breakTurnsRemaining = 0);
            if (dbf.paralysisTurnsRemaining > 0) cleansePool.Add(() => dbf.paralysisTurnsRemaining = 0);

            if (cleansePool.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, cleansePool.Count);
                cleansePool[index].Invoke();
                Debug.Log("Debuff Cleansed!");
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