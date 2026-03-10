using System.Collections;
using MyRule.Audio;
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
                target.debuffManager.ApplyDebuff(user, skill.debuffProperties);
            }

            if ((skill.skillType == SkillType.Buff || skill.skillType == SkillType.Shield) && target.buffManager != null)
            {
                target.buffManager.ApplyBuff(skill.buffProperties, null, skill.buffProperties.amount, skill);
            }

            if (skill.stackApplicationTarget == StackApplicationTarget.Self)
            {
                if (user.buffManager != null)
                {
                    user.buffManager.ProcessSkillStacks(skill, target);
                }
            }
            else if (skill.stackApplicationTarget == StackApplicationTarget.Target)
            {
                if (target.buffManager != null)
                {
                    target.buffManager.ProcessSkillStacks(skill, user);
                }
            }

            user.UpdateOwnUI();
            target.UpdateOwnUI();

            if (user.battleUIManager != null) user.battleUIManager.UpdateCharacterUI(user);
            if (target.battleUIManager != null) target.battleUIManager.UpdateCharacterUI(target);
        }

        protected Flyweight_TB SpawnImpactEffect(Vector3 position, Skill skill)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(skill.impactSFXType);
            }

            FlyweightSettings_TB settingsToSpawn = skill.impactVFXPrefab;
            Flyweight_TB effectInstance = null; 

            if (settingsToSpawn != null)
            {
                effectInstance = FlyweightFactory_TB.Spawn(settingsToSpawn);

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

        protected Flyweight_TB SpawnContinuousEffect(Vector3 position, Character targetCharacter, Skill skill)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(skill.impactSFXType);
            }

            FlyweightSettings_TB settingsToSpawn = skill.impactVFXPrefab; 
            Flyweight_TB effectInstance = null; 

            if (settingsToSpawn != null)
            {
                effectInstance = FlyweightFactory_TB.Spawn(settingsToSpawn); 

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