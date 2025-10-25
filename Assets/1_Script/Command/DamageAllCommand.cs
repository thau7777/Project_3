using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


namespace Turnbase
{
    public class DamageAllCommand : ICommand
    {
        private Character user;
        private Skill skill;
        private BattleManager battleManager;

        private const float ATTACK_MULTIPLIER = 0.5f;
        private const float TARGET_DELAY = 0.05f;

        public DamageAllCommand(Character user, Skill skill, BattleManager battleManager)
        {
            this.user = user;
            this.skill = skill;
            this.battleManager = battleManager;
        }

        public IEnumerator Execute()
        {
            user.animator.Play(skill.animationTriggerName);

            yield return new WaitForSeconds(1.5f);

            int rawDamage = CalculateRawDamage();
            List<Character> allTargets = GetTargets();

            yield return ApplyDamageToTargets(rawDamage, allTargets);

            float totalAnimationDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(totalAnimationDuration);

            if (battleManager != null)
            {
                battleManager.EndTurn(user);
            }
        }

        private int CalculateRawDamage()
        {
            int offensiveStat;

            switch (skill.elementType)
            {
                case ElementType.Magical:
                case ElementType.Fire:
                case ElementType.Ice:
                case ElementType.Poison:
                case ElementType.Lightning:
                case ElementType.Dark:
                    offensiveStat = user.stats.magicAttack;
                    break;

                case ElementType.Physical:
                case ElementType.None:
                default:
                    offensiveStat = user.stats.attack;
                    break;
            }

            return skill.damage + Mathf.RoundToInt(offensiveStat * ATTACK_MULTIPLIER);
        }

        private List<Character> GetTargets()
        {
            if (user.isPlayer)
            {
                return battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
            }
            else
            {
                return battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
            }
        }

        private IEnumerator ApplyDamageToTargets(int rawDamage, List<Character> targets)
        {
            foreach (Character aoeTarget in targets)
            {
                int defensiveStat;

                if (skill.elementType != ElementType.Physical && skill.elementType != ElementType.None)
                {
                    defensiveStat = aoeTarget.stats.magicDefense;
                }
                else
                {
                    defensiveStat = aoeTarget.stats.defense;
                }

                float defenseMultiplier = 100f / (defensiveStat + 100f);
                float damageBase = rawDamage * defenseMultiplier;

                float elementMultiplier = GetElementMultiplier(aoeTarget);
                int finalDamage = Mathf.RoundToInt(damageBase * elementMultiplier);

                if (rawDamage > 0)
                {
                    finalDamage = Mathf.Max(1, finalDamage);
                }

                aoeTarget.TakeDamage(finalDamage);
                SpawnImpactEffect(aoeTarget.transform.position);

                yield return new WaitForSeconds(TARGET_DELAY);
            }
        }

        private float GetElementMultiplier(Character target)
        {
            if (battleManager != null && battleManager.elementChart != null)
            {
                return battleManager.elementChart.GetMultiplier(skill.elementType, target.characterElement);
            }

            return 1.0f;
        }

        private void SpawnImpactEffect(Vector3 position)
        {
            GameObject effectToSpawn = skill.impactVFXPrefab;

            float duration = 3f;

            if (effectToSpawn != null)
            {
                GameObject effectInstance = GameObject.Instantiate(effectToSpawn, position, Quaternion.identity);

                GameObject.Destroy(effectInstance, duration);
            }
        }
    }
}