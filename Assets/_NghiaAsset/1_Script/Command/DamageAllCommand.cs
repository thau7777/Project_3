using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turnbase
{
    public class DamageAllCommand : ICommand
    {
        private Character user;
        private Skill skill;
        private BattleManager battleManager;
        private Character mainTarget;
        private bool damageApplied = false;

        private const float TARGET_DELAY = 0.05f;

        public DamageAllCommand(Character user, Skill skill, BattleManager battleManager, Character mainTarget = null)
        {
            this.user = user;
            this.skill = skill;
            this.battleManager = battleManager;
            this.mainTarget = mainTarget;
        }

        public IEnumerator Execute()
        {
            damageApplied = false;

            Action hitAction = () =>
            {
                if (damageApplied) return;

                battleManager.StartCoroutine(ApplyDamageSequence());
                damageApplied = true;
            };

            user.PrepareHitCallBack(hitAction);

            if (!string.IsNullOrEmpty(skill.animationTriggerName))
            {
                user.isLastHit = true;
                user.animator.Play(skill.animationTriggerName, 0, 0f);
            }

            while (!damageApplied) yield return null;

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

            if (battleManager != null)
            {
                battleManager.EndTurn(user);
            }
        }

        private IEnumerator ApplyDamageSequence()
        {
            List<Character> targets = GetTargets();
            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            float delayBetweenHits = skill.delayBetweenHits;

            for (int i = 0; i < hits; i++)
            {
                foreach (Character aoeTarget in targets)
                {
                    if (aoeTarget == null || !aoeTarget.isAlive) continue;

                    int finalDamage = DamageCalculator.GetFinalDamage(user, aoeTarget, skill, battleManager);

                    if (skill.skillType == SkillType.MeleeAttack && aoeTarget != mainTarget)
                    {
                        finalDamage = Mathf.RoundToInt(finalDamage * user.buffManager.splashDamagePercentage);
                    }

                    int currentHitDamage = finalDamage / hits;
                    if (i == hits - 1) currentHitDamage += (finalDamage % hits);

                    if (i == 0)
                    {
                        if (skill.debuffProperties.statToModify != DebuffType.None)
                            aoeTarget.debuffManager.ApplyDebuff(user, skill.debuffProperties);

                        if (skill.stackApplicationTarget == StackApplicationTarget.Target)
                            user.buffManager.ProcessSkillStacks(skill, aoeTarget);

                        aoeTarget.TakeDamage(user, currentHitDamage, skill.elementType);
                        SpawnImpactEffect(aoeTarget.transform.position);
                    }
                    else
                    {
                        aoeTarget.TakeDamage(user, currentHitDamage, skill.elementType);
                    }

                    yield return new WaitForSeconds(TARGET_DELAY);
                }

                if (i < hits - 1) yield return new WaitForSeconds(delayBetweenHits);
            }
        }

        private List<Character> GetTargets()
        {
            return battleManager.allCombatants.FindAll(c =>
                c != null && c.isAlive && !c.isVirtualTracker && (c.isPlayer != user.isPlayer));
        }

        private void SpawnImpactEffect(Vector3 position)
        {
            if (skill.impactVFXPrefab != null)
            {
                Flyweight_TB effectInstance = FlyweightFactory_TB.Spawn(skill.impactVFXPrefab);
                if (effectInstance != null) effectInstance.Initialize(position, Quaternion.identity);
            }
        }
    }
}