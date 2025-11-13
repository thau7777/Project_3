using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;


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
            if (user.isPlayer)
            {
                return battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
            }
            else
            {
                return battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
            }
        }

        private IEnumerator ApplyDamageToTargets(List<Character> targets)
        {
            ElementType element = skill.elementType;

            foreach (Character aoeTarget in targets)
            {
                if (aoeTarget == null || !aoeTarget.isAlive) continue;

                int finalDamage = DamageCalculator.GetFinalDamage(user, aoeTarget, skill, battleManager);

                aoeTarget.TakeDamage(finalDamage, element);

                SpawnImpactEffect(aoeTarget.transform.position);

                if (skill.debuffProperties.statToModify != DebuffType.None)
                {
                    aoeTarget.debuffManager.ApplyDebuff(skill.debuffProperties);

                }

                if (skill.stackApplicationTarget == StackApplicationTarget.Target)
                {
                    user.buffManager.ProcessSkillStacks(skill, aoeTarget);
                }

                yield return new WaitForSeconds(TARGET_DELAY);
            }
        }

        private void SpawnImpactEffect(Vector3 position)
        {
            FlyweightSettings2 effectToSpawn = skill.impactVFXPrefab;

            if (effectToSpawn != null)
            {
                Flyweight2 effectInstance = FlyweightFactory2.Spawn(effectToSpawn);

                if (effectInstance != null)
                {
                    effectInstance.FlyweightInitialize(position, Quaternion.identity);

                }
            }
        }
    }
}