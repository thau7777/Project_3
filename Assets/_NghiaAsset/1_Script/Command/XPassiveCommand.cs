using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Turnbase
{
    public class XPassiveCommand : SkillCommand
    {
        private BattleManager battleManager;
        private bool damageApplied = false;

        public XPassiveCommand(Character user, Skill skill, BattleManager battleManager)
            : base(user, null, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            var targets = battleManager.allCombatants
                .Where(c => c.isPlayer != user.isPlayer && c.isAlive && !c.isVirtualTracker)
                .ToList();

            if (targets.Count == 0) yield break;

            int hits = skill.numberOfHits > 0 ? skill.numberOfHits : 1;
            Dictionary<Character, int> baseDamageMap = new Dictionary<Character, int>();
            Dictionary<Character, int> remainderMap = new Dictionary<Character, int>();

            foreach (var target in targets)
            {
                int totalDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager);
                baseDamageMap[target] = totalDamage / hits;
                remainderMap[target] = totalDamage % hits;
            }

            Action hitAction = () =>
            {
                if (!damageApplied)
                {
                    Vector3 effectPos = user.buffEffectSpawnPoint != null ?
                                       user.buffEffectSpawnPoint.position :
                                       user.transform.position;

                    SpawnImpactEffect(effectPos, skill);

                    foreach (var targetEnemy in targets)
                    {
                        if (targetEnemy == null || !targetEnemy.isAlive) continue;

                        int damageToDeal = baseDamageMap[targetEnemy] + (hits == 1 ? remainderMap[targetEnemy] : 0);
                        targetEnemy.TakeDamage(user, damageToDeal, skill.elementType);

                        if (skill.stackApplicationTarget == StackApplicationTarget.Target && targetEnemy.buffManager != null)
                        {
                            targetEnemy.buffManager.ProcessSkillStacks(skill, targetEnemy);
                        }
                    }
                    damageApplied = true;
                }
            };

            if (skill.stackApplicationTarget == StackApplicationTarget.Self && user.buffManager != null)
            {
                user.buffManager.ProcessSkillStacks(skill, null);
            }

            user.PrepareHitCallBack(hitAction);
            user.animator.Play(skill.animationTriggerName);

            float startTime = Time.time;
            while (!damageApplied && Time.time < startTime + 2.0f)
            {
                yield return null;
            }

            if (!damageApplied) hitAction.Invoke();

            for (int i = 1; i < hits; i++)
            {
                yield return new WaitForSeconds(skill.delayBetweenHits);

                foreach (var targetEnemy in targets)
                {
                    if (targetEnemy == null || !targetEnemy.isAlive) continue;

                    int damageToDeal = baseDamageMap[targetEnemy];
                    if (i == hits - 1) damageToDeal += remainderMap[targetEnemy]; 

                    targetEnemy.TakeDamage(user, damageToDeal, skill.elementType);
                }
            }

            yield return new WaitForSeconds(0.5f);
            user.animator.Play("Idle");

        }
    }
}