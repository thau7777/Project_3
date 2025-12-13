using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Turnbase
{
    public class DamageAllCommand : ICommand
    {
        private Character user;
        private Skill skill;
        private BattleManager battleManager;

        private const float TARGET_DELAY = 0.05f;

        public DamageAllCommand(Character user, Skill skill, BattleManager battleManager)
        {
            this.user = user;
            this.skill = skill;
            this.battleManager = battleManager;
        }

        public IEnumerator Execute()
        {
            if (!string.IsNullOrEmpty(skill.animationTriggerName))
            {
                user.animator.Play(skill.animationTriggerName);
            }

            yield return new WaitForSeconds(1.5f);

            List<Character> allTargets = GetTargets();

            yield return ApplyDamageToTargets(allTargets);

            float totalAnimationDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(totalAnimationDuration);

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
            target.TakeDamage(damage, element);
        }

        private void ApplySingleHitDamageAndEffect(Character target, int damage)
        {
            ApplySingleHitDamage(target, damage);
            SpawnImpactEffect(target.transform.position);
        }

        private IEnumerator ApplyDamageToTargets(List<Character> targets)
        {
            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            float delayBetweenHits = skill.delayBetweenHits;

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
                            aoeTarget.debuffManager.ApplyDebuff(skill.debuffProperties);
                        }

                        if (skill.stackApplicationTarget == StackApplicationTarget.Target)
                        {
                            user.buffManager.ProcessSkillStacks(skill, aoeTarget);
                        }

                        ApplySingleHitDamageAndEffect(aoeTarget, currentHitDamage);
                    }
                    else
                    {
                        ApplySingleHitDamage(aoeTarget, currentHitDamage);
                    }


                    yield return new WaitForSeconds(TARGET_DELAY);
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