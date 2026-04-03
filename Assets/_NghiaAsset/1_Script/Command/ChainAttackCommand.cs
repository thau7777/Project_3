using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Turnbase
{
    public class ChainAttackCommand : SkillCommand
    {
        private BattleManager battleManager;
        private List<Character> chainTargets = new List<Character>();
        private LineRenderer lineRenderer;

        public ChainAttackCommand(Character user, Character target, Skill skill, BattleManager battleManager)
          : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            user.parryMissCount = 0;
            chainTargets.Clear();
            if (target != null) chainTargets.Add(target);

            var otherEnemies = battleManager.allCombatants
              .Where(c => c.isPlayer != user.isPlayer && c.isAlive && c != target && !c.isVirtualTracker)
              .OrderBy(c => Vector3.Distance(target.transform.position, c.transform.position))
              .ToList();

            chainTargets.AddRange(otherEnemies);

            if (chainTargets.Count == 0) { battleManager.EndTurn(user); yield break; }

            user.animator.Play(skill.animationTriggerName);
            yield return new WaitForSeconds(0.4f);

            Flyweight_TB effectInstance = null;
            Vector3 startPos = user.SkillSpawnPoint != null ? user.SkillSpawnPoint.position : user.transform.position;

            if (skill.lazerSettings != null)
            {
                effectInstance = FlyweightFactory_TB.Spawn(skill.lazerSettings);
                if (effectInstance != null)
                {
                    effectInstance.Initialize(startPos, Quaternion.identity);
                    lineRenderer = effectInstance.GetComponent<LineRenderer>();

                    if (lineRenderer != null)
                    {
                        var autoScripts = effectInstance.GetComponents<MonoBehaviour>();
                        foreach (var s in autoScripts)
                        {
                            if (s != effectInstance && s.GetType().Name != "LineRenderer") s.enabled = false;
                        }

                        lineRenderer.enabled = true;
                        lineRenderer.useWorldSpace = true;
                        lineRenderer.positionCount = chainTargets.Count + 1;

                        for (int n = 0; n < lineRenderer.positionCount; n++)
                        {
                            lineRenderer.SetPosition(n, startPos);
                        }
                    }
                }
            }

            for (int i = 0; i < chainTargets.Count; i++)
            {
                Character currentTarget = chainTargets[i];
                Vector3 targetPoint = currentTarget.transform.position + Vector3.up * 1.1f;

                int totalDamage = DamageCalculator.GetFinalDamage(user, currentTarget, skill, battleManager);
                int damagePerHit = totalDamage / Mathf.Max(1, skill.numberOfHits);

                if (lineRenderer != null)
                {
                    lineRenderer.SetPosition(i + 1, targetPoint);

                    for (int j = i + 1; j < lineRenderer.positionCount; j++)
                    {
                        lineRenderer.SetPosition(j, targetPoint);
                    }
                }

                currentTarget.TakeDamage(user, damagePerHit, skill.elementType);
                SpawnImpactEffect(targetPoint, skill);

                if (i == 0 || skill.targetType == SkillTargetType.Enemies) 
                {
                    ApplyStatusEffectsAndStacks(user, currentTarget, skill);
                }

                float waitTime = Mathf.Max(0.2f, skill.delayBetweenHits);
                yield return new WaitForSeconds(waitTime);
            }

            if (lineRenderer != null)
            {
                Vector3[] finalPositions = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(finalPositions);

                float retractSpeed = Mathf.Max(0.1f, skill.delayBetweenHits * 0.7f);

                for (int i = 0; i < chainTargets.Count; i++)
                {
                    for (int k = 0; k <= i; k++)
                    {
                        lineRenderer.SetPosition(k, finalPositions[i + 1]);
                    }


                    yield return new WaitForSeconds(retractSpeed);
                }
            }

            if (effectInstance != null) effectInstance.gameObject.SetActive(false);

            user.animator.Play("Idle");
            yield return new WaitForSeconds(0.2f);
            battleManager.EndTurn(user);
        }
    }
}
