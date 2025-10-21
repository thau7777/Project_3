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
    }
}




