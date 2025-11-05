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

        protected Flyweight SpawnImpactEffect(Vector3 position, Skill skill)
        {
            FlyweightSettings settingsToSpawn = skill.impactVFXPrefab;
            Flyweight effectInstance = null; 

            if (settingsToSpawn != null)
            {
                effectInstance = FlyweightFactory.Spawn(settingsToSpawn); 

                if (effectInstance != null)
                {
                    effectInstance.FlyweightInitialize(position, Quaternion.identity);

                }
            }
            else
            {
                Debug.LogWarning($"Thiếu FlyweightSettings Impact VFX cho kỹ năng: {skill.skillName}.");
            }

            return effectInstance; 
        }

        protected Flyweight SpawnContinuousEffect(Vector3 position, Character targetCharacter, Skill skill)
        {
            FlyweightSettings settingsToSpawn = skill.impactVFXPrefab; 
            Flyweight effectInstance = null; 

            if (settingsToSpawn != null)
            {
                effectInstance = FlyweightFactory.Spawn(settingsToSpawn); 

                if (effectInstance != null)
                {
                    effectInstance.FlyweightInitialize(position, Quaternion.identity); 
                    
                    effectInstance.transform.SetParent(targetCharacter.transform);
                    effectInstance.transform.localPosition = Vector3.zero;

                    Debug.Log($"Đã Spawn hiệu ứng liên tục '{settingsToSpawn.name}' lên {targetCharacter.name} dùng Flyweight.");
                }
            }
            else
            {
                Debug.LogWarning($"Thiếu FlyweightSettings Continuous VFX cho kỹ năng: {skill.skillName}.");
            }
            return effectInstance;
        }

    }
}