using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEditor.Rendering.FilterWindow;


namespace Turnbase
{
    public class DamageAllGlobalCommand : ICommand
    {
        private Character user;
        private Skill skill;
        private BattleManager battleManager;

        private const float TARGET_DELAY = 0.05f;
        private const float DAMAGE_START_DELAY = 1.5f;

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
                user.animator.Play(skill.animationTriggerName);
            }

            yield return new WaitForSeconds(DAMAGE_START_DELAY);

            SpawnImpactEffect(new Vector3(0f, 0f, 0f));

            List<Character> allTargets = GetTargets();

            yield return ApplyDamageToTargets(allTargets);

            float totalAnimationDuration = user.animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(0.5f);

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

        private IEnumerator ApplyDamageToTargets(List<Character> targets)
        {
            ElementType element = skill.elementType;

            foreach (Character aoeTarget in targets)
            {
                if (aoeTarget == null || !aoeTarget.isAlive) continue;

                int finalDamage = DamageCalculator.GetFinalDamage(user, aoeTarget, skill, battleManager);

                aoeTarget.TakeDamage(finalDamage, element);

                if (skill.debuffProperties.statToModify != DebuffType.None)
                {
                    aoeTarget.debuffManager.ApplyDebuff(skill.debuffProperties);

                }

                if (skill.stackApplicationTarget == StackApplicationTarget.Target)
                {
                    user.buffManager.ProcessSkillStacks(skill, aoeTarget);
                }

                if (skill.cameraTimeline != null && battleManager.mainDirector != null)
                {
                    PlayableDirector director = battleManager.mainDirector;
                    director.playableAsset = skill.cameraTimeline;

                    director.Play();
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
                    ((ImpactVFX2)effectInstance).Initialize(position, Quaternion.identity, skill.impactVFXDuration);
                }
            }
        }
    }
}