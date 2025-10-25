using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Turnbase
{
    public class DamageAllCommand : ICommand
    {
        private Character user;
        private Skill skill;
        private BattleManager battleManager;

        private const float ATTACK_MULTIPLIER = 0.5f;

        public DamageAllCommand(Character user, Skill skill, BattleManager battleManager)
        {
            this.user = user;
            this.skill = skill;
            this.battleManager = battleManager;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{user.name} dùng skill AOE {skill.skillName}");

            user.animator.Play(skill.animationTriggerName);

            yield return new WaitForSeconds(1.5f);

            int rawDamage = CalculateRawDamage();
            List<Character> allTargets = GetTargets();

            yield return ApplyDamageToTargets(rawDamage, allTargets);

            yield return new WaitForSeconds(0.5f);

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
                case ElementType.Holy:
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
            if (skill.targetType == SkillTargetType.Enemies)
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
                int defensiveStat = aoeTarget.stats.defense;

                if (skill.elementType != ElementType.Physical && skill.elementType != ElementType.None)
                {
                    defensiveStat = aoeTarget.stats.magicDefense;
                }

                float damageMultiplier = 100f / (defensiveStat + 100f);

                int finalDamage = Mathf.RoundToInt(rawDamage * damageMultiplier);

                if (rawDamage > 0)
                {
                    finalDamage = Mathf.Max(1, finalDamage);
                }

                aoeTarget.TakeDamage(finalDamage);
                SpawnImpactEffect(aoeTarget.transform.position);

                yield return null;
            }
        }

        private void SpawnImpactEffect(Vector3 position)
        {
            GameObject effectToSpawn = skill.impactVFXPrefab;

            if (effectToSpawn != null)
            {
                GameObject effectInstance = GameObject.Instantiate(effectToSpawn, position, Quaternion.identity);

                GameObject.Destroy(effectInstance, 3f);
            }
            else
            {
                Debug.LogWarning($"Thiếu Prefab Impact VFX cho kỹ năng: {skill.skillName}");
            }
        }
    }
}