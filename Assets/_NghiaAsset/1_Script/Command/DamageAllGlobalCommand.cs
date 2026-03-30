using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyRule.Audio;
using UnityEngine;
using UnityEngine.Playables;


namespace Turnbase
{
    public class DamageAllGlobalCommand : ICommand
    {
        private Character user;
        private Skill skill;
        private BattleManager battleManager;

        private const float POST_ANIMATION_DELAY = 0.5f;

        public DamageAllGlobalCommand(Character user, Skill skill, BattleManager battleManager)
        {
            this.user = user;
            this.skill = skill;
            this.battleManager = battleManager;
        }

        public IEnumerator Execute()
        {
            if (!string.IsNullOrEmpty(skill.animationTriggerName))
            {
                user.isLastHit = true;
                user.animator.Play(skill.animationTriggerName);
            }

            if (skill.cameraTimeline != null && battleManager.mainDirector != null)
            {
                battleManager.mainDirector.playableAsset = skill.cameraTimeline;
                battleManager.mainDirector.Play();
            }

            SpawnImpactEffect(new Vector3(0f, 0f, 0f));

            Action damageLogicCallback = () =>
            {
                List<Character> allTargets = GetTargets();

                battleManager.StartCoroutine(ApplyDamageToTargets(allTargets));
            };

            user.PrepareHitCallBack(damageLogicCallback);

            yield return null;

            var stateInfo = user.animator.GetCurrentAnimatorStateInfo(0);
            float charAnimDuration = stateInfo.length;

            float timelineDuration = (skill.cameraTimeline != null) ? (float)skill.cameraTimeline.duration : 0f;

            float maxDuration = Mathf.Max(charAnimDuration, timelineDuration);
            float elapsed = 0f;

            while (elapsed < maxDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            user.animator.Play("Idle");

            float animationDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;

            if (skill.cameraTimeline != null && battleManager.mainDirector != null)
            {
                animationDuration = Mathf.Max(animationDuration, (float)skill.cameraTimeline.duration);
            }

            yield return new WaitForSeconds(animationDuration + POST_ANIMATION_DELAY);

            user.PrepareHitCallBack(null);

            if (battleManager != null)
            {
                battleManager.EndTurn(user);
            }
        }

        private List<Character> GetTargets()
        {
            List<Character> targets;

            if (user.isPlayer)
            {
                targets = battleManager.allCombatants.FindAll(
                    c => c != null &&
                            !c.isPlayer &&
                            c.isAlive &&
                            !c.isVirtualTracker
                );
            }
            else
            {
                targets = battleManager.allCombatants.FindAll(
                    c => c != null &&
                            c.isPlayer &&
                            c.isAlive &&
                            !c.isVirtualTracker
                );
            }

            return targets;
        }

        private void ApplySingleHitDamage(Character target, int damage)
        {
            ElementType element = skill.elementType;
            target.TakeDamage(user,damage, element);
        }

        private IEnumerator ApplyDamageToTargets(List<Character> targets)
        {
            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            float delayBetweenHits = skill.delayBetweenHits;
            float targetDelay = 0.05f;

            for (int i = 0; i < hits; i++)
            {

                foreach (Character aoeTarget in targets)
                {
                    if (aoeTarget == null || !aoeTarget.isAlive) continue;

                    int finalDamage = DamageCalculator.GetFinalDamage(user, aoeTarget, skill, battleManager);
                    int baseDamagePerHit = finalDamage / hits;
                    int damageRemainder = finalDamage % hits;
                    int currentHitDamage = baseDamagePerHit;

                    if (i == hits - 1)
                    {
                        currentHitDamage += damageRemainder;
                    }


                    if (i == 0)
                    {
                        if (skill.debuffProperties.statToModify != DebuffType.None)
                        {
                            aoeTarget.debuffManager.ApplyDebuff(user, skill.debuffProperties);
                        }

                        if (skill.stackApplicationTarget == StackApplicationTarget.Target)
                        {
                            user.buffManager.ProcessSkillStacks(skill, aoeTarget);
                        }
                    }

                    ApplySingleHitDamage(aoeTarget, currentHitDamage);

                    yield return new WaitForSeconds(targetDelay);
                }


                if (i < hits - 1)
                {
                    yield return new WaitForSeconds(delayBetweenHits);
                }
            }
        }

        private void SpawnImpactEffect(Vector3 position)
        {
            FlyweightSettings_TB effectToSpawn = skill.impactVFXPrefab;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(skill.impactSFXType);
            }

            if (effectToSpawn != null)
            {
                Flyweight_TB effectInstance = FlyweightFactory_TB.Spawn(effectToSpawn);

                if (effectInstance != null)
                {
                    effectInstance.Initialize(position, Quaternion.identity);
                }
            }
        }
    }
}