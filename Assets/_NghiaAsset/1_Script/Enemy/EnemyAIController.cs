using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using System;

namespace Turnbase
{
    public static class SkillResource
    {
        public const int MAX_POOL = 100;
        public static readonly Dictionary<SkillType, int> CostPerUse = new Dictionary<SkillType, int>
        {
            { SkillType.Heal, 50 },
            { SkillType.Buff, 50 },
            { SkillType.Shield, 100 },
            { SkillType.MeleeAttack, 20 },
            { SkillType.RangedAttack, 20 },
            { SkillType.RangedProjectile, 20 },
        };
    }

    public class EnemyAIController
    {
        private const float RANDOMNESS_FACTOR_MIN = -0.05f;
        private const float RANDOMNESS_FACTOR_MAX = 0.05f;

        private const float BONUS_FINISHER = 100f;
        private const float BONUS_CRITICAL_HEAL = 80f; 
        private const float BONUS_DEBUFF = 50f; 
        private const float BONUS_AOE_PER_TARGET = 75f;

        public (Skill chosenSkill, Character chosenTarget) DetermineBestAction(Character user, BattleManager battleManager)
        {
            float maxScore = -9999f;
            Skill bestSkill = null;
            Character bestTarget = null;

            List<Character> playerTargets = battleManager.allCombatants
                .FindAll(c => c.isPlayer && c.isAlive && !c.isVirtualTracker);
            List<Character> allyTargets = battleManager.allCombatants
                .FindAll(c => !c.isPlayer && c.isAlive && !c.isVirtualTracker);

            if (playerTargets.Count == 0 && allyTargets.Count == 0)
            {
                return (null, null);
            }

            Enemy enemyUser = user as Enemy;

            foreach (Skill skill in user.skills)
            {
                int skillTypeCost = 0;

                if (SkillResource.CostPerUse.TryGetValue(skill.skillType, out int cost))
                {
                    skillTypeCost = cost;
                }

                if (enemyUser != null && enemyUser.currentSkillTypePool.GetValueOrDefault(skill.skillType, 0) < skillTypeCost)
                {
                    continue;
                }

                if (user.stats.currentMP < skill.manaCost) continue;

                List<Character> currentPossibleTargets = GetTargetsForSkill(user, skill, playerTargets, allyTargets);

                if (currentPossibleTargets.Count == 0 && skill.skillType != SkillType.DamageAll && skill.targetType != SkillTargetType.Allies)
                {
                    continue;
                }

                if (skill.targetType == SkillTargetType.Enemies) 
                {
                    if (playerTargets.Count == 0) continue;

                    float aoeScore = EvaluateDamageAllSkill(user, skill, playerTargets, battleManager);
                    if (aoeScore > maxScore)
                    {
                        maxScore = aoeScore;
                        bestSkill = skill;
                        bestTarget = playerTargets.FirstOrDefault();
                    }
                    continue;
                }

                if (skill.targetType == SkillTargetType.Allies)
                {
                    float aoeScore = EvaluateAllyAllSkill(user, skill, allyTargets, battleManager);
                    if (aoeScore > maxScore)
                    {
                        maxScore = aoeScore;
                        bestSkill = skill;
                        bestTarget = allyTargets.FirstOrDefault();
                    }
                    continue;
                }

                foreach (Character target in currentPossibleTargets)
                {
                    float currentScore = EvaluateSingleTargetSkill(user, target, skill, battleManager);

                    if (currentScore > maxScore)
                    {
                        maxScore = currentScore;
                        bestSkill = skill;
                        bestTarget = target;
                    }
                }
            }

            if (bestSkill == null && playerTargets.Count > 0)
            {
                Skill defaultAttack = user.skills.FirstOrDefault();
                if (defaultAttack != null)
                {
                    Debug.LogWarning("AI không chọn được skill, đánh thường");
                    return (defaultAttack, playerTargets.FirstOrDefault());
                }
            }

            return (bestSkill, bestTarget);
        }

        private float EvaluateSingleTargetSkill(Character user, Character target, Skill skill, BattleManager battleManager)
        {
            float skillPowerScore = 0f;
            float tacticalScore = 0f;

            switch (skill.skillType)
            {
                case SkillType.MeleeAttack:
                case SkillType.RangedAttack:
                case SkillType.RangedProjectile:
                    skillPowerScore = DamageCalculator.GetFinalDamage(user, target, skill, battleManager) * 1.0f;

                    if (target.stats.currentHP <= skillPowerScore && target.stats.currentHP > 0)
                    {
                        tacticalScore += BONUS_FINISHER;
                    }
                    if (skill.debuffProperties.statToModify != DebuffType.None)
                    {
                        tacticalScore += BONUS_DEBUFF;
                    }
                    break;

                case SkillType.Heal:
                    float hpPercent = (float)target.stats.currentHP / target.stats.maxHP;

                    if (hpPercent >= 0.9f)
                    {
                        return -5000f;
                    }

                    skillPowerScore = skill.damage * 1.0f;
                    tacticalScore = 0f;


                    if (hpPercent >= 0.5f)
                    {
                        tacticalScore -= 500f;
                    }

                    if (hpPercent < 0.5f)
                    {
                        tacticalScore += BONUS_CRITICAL_HEAL;
                    }
                    if (hpPercent < 0.25f)
                    {
                        tacticalScore += BONUS_CRITICAL_HEAL; 
                    }
                    break;

                case SkillType.Buff:
                case SkillType.Shield:
                    skillPowerScore = skill.buffProperties.amount * 10f;

                    tacticalScore = 0f;

                    int turnsRemaining = 0;
                    if (skill.skillType == SkillType.Buff)
                    {
                        turnsRemaining = target.buffManager.GetBuffTurnsRemaining(skill.buffProperties.statToModify);
                    }
                    else if (skill.skillType == SkillType.Shield)
                    {
                        turnsRemaining = target.buffManager.shieldTurnsRemaining;
                    }

                    if (turnsRemaining <= 0)
                    {
                        tacticalScore += 100f;
                    }
                    else if (turnsRemaining == 1)
                    {
                        tacticalScore += 50f;
                    }
                    else
                    {
                        tacticalScore -= 1000f;
                    }
                    break;
            }

            float randomness = 1.0f + Random.Range(RANDOMNESS_FACTOR_MIN, RANDOMNESS_FACTOR_MAX);

            return (skillPowerScore + tacticalScore) * randomness;
        }

        private float EvaluateDamageAllSkill(Character user, Skill skill, List<Character> targets, BattleManager battleManager)
        {
            float totalDamageScore = 0f;
            float totalFinisherBonus = 0f;

            foreach (var target in targets)
            {
                float estimatedDamage = DamageCalculator.GetFinalDamage(user, target, skill, battleManager) * 1.0f;
                totalDamageScore += estimatedDamage;

                if (target.stats.currentHP <= estimatedDamage && target.stats.currentHP > 0)
                {
                    totalFinisherBonus += BONUS_FINISHER / 2;
                }
            }

            float skillPowerScore = totalDamageScore;

            float tacticalScore = (targets.Count * BONUS_AOE_PER_TARGET) + totalFinisherBonus;

            float randomness = 1.0f + Random.Range(RANDOMNESS_FACTOR_MIN, RANDOMNESS_FACTOR_MAX);

            return (skillPowerScore + tacticalScore) * randomness;
        }

        private float EvaluateAllyAllSkill(Character user, Skill skill, List<Character> targets, BattleManager battleManager)
        {
            float totalHealScore = 0f;
            float totalUrgencyBonus = 0f;

            foreach (var target in targets)
            {
                if (skill.skillType == SkillType.Heal && target.stats.currentHP < target.stats.maxHP)
                {
                    totalHealScore += skill.damage * 2.5f;
                    if ((float)target.stats.currentHP / target.stats.maxHP < 0.5f)
                    {
                        totalUrgencyBonus += BONUS_CRITICAL_HEAL;
                    }
                }
                else if (skill.skillType == SkillType.Buff && !target.buffManager.IsBuffActive(skill.buffProperties.statToModify))
                {
                    totalHealScore += skill.buffProperties.amount * 10f;
                }
            }

            float skillPowerScore = totalHealScore;
            float tacticalScore = (targets.Count * 50f) + totalUrgencyBonus;

            float randomness = 1.0f + Random.Range(RANDOMNESS_FACTOR_MIN, RANDOMNESS_FACTOR_MAX);

            return (skillPowerScore + tacticalScore) * randomness;
        }


        private List<Character> GetTargetsForSkill(Character user, Skill skill, List<Character> playerTargets, List<Character> allyTargets)
        {
            switch (skill.targetType)
            {
                case SkillTargetType.Self:
                    return new List<Character> { user };
                case SkillTargetType.Ally:
                    return allyTargets.FindAll(c => c.isAlive);
                case SkillTargetType.Enemy:
                    return playerTargets;
                case SkillTargetType.Allies:
                case SkillTargetType.Enemies:
                    return new List<Character>();
                default:
                    return new List<Character>();
            }
        }
    }
}