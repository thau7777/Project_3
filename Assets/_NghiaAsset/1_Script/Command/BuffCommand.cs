using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


namespace Turnbase
{
    public class BuffCommand : SkillCommand
    {
        private BattleManager battleManager;

        public BuffCommand(Character user, Character target, Skill skill, BattleManager battleManager)
            : base(user, target, skill)
        {
            this.battleManager = battleManager;
        }

        public override IEnumerator Execute()
        {
            Debug.Log($"{user.name} dùng skill Buff {skill.buffProperties.statToModify}!");

            //ApplyStatusEffectsAndStacks(user, target, skill);

            yield return AnimateBuffUser();

            List<Character> targetsToBuff = FindTargets();

            ApplyBuffEffects(targetsToBuff);

            yield return new WaitForSeconds(0.5f);

            battleManager.EndTurn(user);
        }

        private IEnumerator AnimateBuffUser()
        {
            user.animator.Play(skill.animationTriggerName);
            yield return new WaitForSeconds(1.5f);
        }

        private List<Character> FindTargets()
        {
            if (skill.targetType == SkillTargetType.Ally || skill.targetType == SkillTargetType.Self)
            {
                if (target != null && target.isAlive)
                {
                    return new List<Character> { target };
                }
            }
            else if (skill.targetType == SkillTargetType.Allies)
            {
                return battleManager.allCombatants
                    .Where(c => c.isPlayer == user.isPlayer && c.isAlive)
                    .ToList();
            }
            return new List<Character>();
        }

        private void ApplyBuffEffects(List<Character> targetsToBuff)
        {
            foreach (var charTarget in targetsToBuff)
            {
                Transform spawnPoint = charTarget.buffEffectSpawnPoint != null ? charTarget.buffEffectSpawnPoint : charTarget.transform;
                Flyweight_TB activeVFX = SpawnContinuousEffect(spawnPoint.position, charTarget, skill);

                if (activeVFX != null)
                {
                    activeVFX.transform.SetParent(spawnPoint);
                    activeVFX.transform.localPosition = Vector3.zero;
                }

                if (skill.buffProperties.statToModify == StatType.Purify)
                {
                    charTarget.debuffManager?.PurifyAllDebuffs();
                }
                else
                {
                    charTarget.buffManager.ApplyBuff(skill.buffProperties, activeVFX, skill.buffProperties.amount, skill);
                }
            }
        }
    }
}