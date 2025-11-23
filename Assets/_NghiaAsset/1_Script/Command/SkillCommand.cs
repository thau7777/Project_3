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

        protected void ApplyStatusEffectsAndStacks(Character user, Character target, Skill skill)
        {
            if (skill.debuffProperties.statToModify != DebuffType.None && target.debuffManager != null)
            {
                target.debuffManager.ApplyDebuff(skill.debuffProperties);
            }

            if ((skill.skillType == SkillType.Buff || skill.skillType == SkillType.Shield) && target.buffManager != null)
            {
                target.buffManager.ApplyBuff(
                    skill.buffProperties,
                    null,
                    skill.buffProperties.amount 
                );
            }

            if (user.buffManager != null)
            {
                user.buffManager.ProcessSkillStacks(skill, target);
            }


            user.UpdateOwnUI();
            if (user.battleUIManager != null)
            {
                user.battleUIManager.UpdateCharacterUI(user);
            }
        }

        protected Flyweight2 SpawnImpactEffect(Vector3 position, Skill skill)
        {
            FlyweightSettings2 settingsToSpawn = skill.impactVFXPrefab;
            Flyweight2 effectInstance = null; 

            if (settingsToSpawn != null)
            {
                effectInstance = FlyweightFactory2.Spawn(settingsToSpawn); 

                if (effectInstance != null)
                {
                    effectInstance.Initialize(position, Quaternion.identity);

                }
            }
            else
            {
                Debug.LogWarning($"Thiếu FlyweightSettings Impact VFX cho kỹ năng: {skill.skillName}.");
            }

            return effectInstance; 
        }

        protected Flyweight2 SpawnContinuousEffect(Vector3 position, Character targetCharacter, Skill skill)
        {
            FlyweightSettings2 settingsToSpawn = skill.impactVFXPrefab; 
            Flyweight2 effectInstance = null; 

            if (settingsToSpawn != null)
            {
                effectInstance = FlyweightFactory2.Spawn(settingsToSpawn); 

                if (effectInstance != null)
                {
                    effectInstance.Initialize(position, Quaternion.identity); 
                    
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