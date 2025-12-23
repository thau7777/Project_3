using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Turnbase
{
    public class SummonCommand : SkillCommand
    {
        public SummonCommand(Character user, Character target, Skill skill) : base(user, target, skill) { }

        public override IEnumerator Execute()
        {
            Debug.Log($"{user.name} sử dụng kỹ năng triệu hồi {skill.skillName}");

            if (TB_AudioSkillManager.Instance != null && skill.castSound.clip != null)
            {
                TB_AudioSkillManager.Instance.PlaySkillSound(skill.castSound);
            }

            if (!string.IsNullOrEmpty(skill.animationTriggerName))
            {
                user.animator.Play(skill.animationTriggerName);
            }

            yield return new WaitForSeconds(0.8f);

            GameObject petPrefab = skill.summonPrefab.FirstOrDefault();
            Character newPet = null;

            if (petPrefab != null && user.battleManager != null)
            {
                newPet = user.battleManager.SummonPet(user, petPrefab);
            }

            if (newPet != null)
            {
                FlyweightSettings_TB effectToSpawn = skill.impactVFXPrefab;
                if (effectToSpawn != null)
                {
                    Flyweight_TB effectInstance = FlyweightFactory_TB.Spawn(effectToSpawn);
                    effectInstance.Initialize(newPet.transform.position, Quaternion.identity);

                    user.StartCoroutine(ReleaseVFXAfterDelay(effectInstance, 2.0f));
                }

                if (TB_AudioSkillManager.Instance != null && skill.impactSound.clip != null)
                {
                    TB_AudioSkillManager.Instance.PlaySkillSound(skill.impactSound);
                }

                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (user.battleManager != null)
            {
                user.battleManager.EndTurn(user);
            }
        }

        private IEnumerator ReleaseVFXAfterDelay(Flyweight_TB effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (effect != null)
            {
                FlyweightFactory_TB.ReturnToPool(effect);
            }
        }
    }
}