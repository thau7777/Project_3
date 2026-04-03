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
                Skill.DebuffSettings finalDebuff = skill.debuffProperties;

                if (finalDebuff.statToModify == DebuffType.Random)
                {
                    DebuffType[] pool = {
                    DebuffType.Burn,
                    DebuffType.Poison,
                    DebuffType.Stun,
                    DebuffType.DefReduction,
                    DebuffType.SpeedReduction,
                    DebuffType.Paralysis
            };

                    finalDebuff.statToModify = pool[Random.Range(0, pool.Length)];

                    Debug.Log($"[RANDOM DEBUFF] Kỹ năng tung ra hiệu ứng ngẫu nhiên: {finalDebuff.statToModify}");
                }

                target.debuffManager.ApplyDebuff(user, finalDebuff);
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
                    target.buffManager.ProcessSkillStacks(skill, target);
                }
            }

            user.UpdateOwnUI();
            target.UpdateOwnUI();

            if (user.battleUIManager != null) user.battleUIManager.UpdateCharacterUI(user);
            if (target.battleUIManager != null) target.battleUIManager.UpdateCharacterUI(target);
        }

        protected Flyweight_TB SpawnImpactEffect(Vector3 position, Skill skill)
        {
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

        protected Flyweight_TB SpawnMeleeEffect(Vector3 position, Skill skill)
        {
            FlyweightSettings_TB settingsToSpawn = skill.meleeSettings;
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
                Debug.LogWarning($"Thiếu FlyweightSettings Melee VFX cho kỹ năng: {skill.skillName}.");
            }

            return effectInstance;
        }

        protected Flyweight_TB SpawnContinuousEffect(Vector3 position, Character targetCharacter, Skill skill)
        {

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