using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public interface ICommand
    {
        IEnumerator Execute();
    }

    public abstract class SkillCommand : ICommand
    {
        protected Character user;
        protected Character target;
        protected Skill skill;

        public SkillCommand(Character user, Character target, Skill skill)
        {
            this.user = user;
            this.target = target;
            this.skill = skill;
        }

        public abstract IEnumerator Execute();

        protected void SpawnImpactEffect(Vector3 position, Skill skill)
        {
            GameObject effectToSpawn = skill.impactVFXPrefab;
            if (effectToSpawn != null)
            {
                GameObject effectInstance = GameObject.Instantiate(effectToSpawn, position, Quaternion.identity);

                GameObject.Destroy(effectInstance, 3f);
            }
            else
            {
                Debug.LogWarning($"Thiếu Prefab Impact VFX cho kỹ năng: {skill.skillName}.");
            }

        }

        protected GameObject SpawnContinuousEffect(Vector3 position, Character targetCharacter, Skill skill)
        {
            GameObject effectToSpawn = skill.impactVFXPrefab; 
            GameObject effectInstance = null; 

            if (effectToSpawn != null)
            {
                effectInstance = GameObject.Instantiate(effectToSpawn, position, Quaternion.identity);

                effectInstance.transform.SetParent(targetCharacter.transform);

                effectInstance.transform.localPosition = Vector3.zero;



                Debug.Log($"Đã Spawn hiệu ứng liên tục '{effectToSpawn.name}' lên {targetCharacter.name}.");
            }
            else
            {
                Debug.LogWarning($"Thiếu Prefab Continuous VFX cho kỹ năng: {skill.skillName}.");
            }
            return effectInstance;
        }
    }
}




