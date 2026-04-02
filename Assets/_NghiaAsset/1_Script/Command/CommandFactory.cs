using UnityEngine;

namespace Turnbase
{
    public static class SkillCommandFactory
    {
        public static ICommand CreateCommand(Character user, Character target, Skill skill, BattleManager battleManager)
        {

            if (skill.skillType == SkillType.MeleeAttack && user.buffManager.splashAttackTurnsRemaining > 0)
            {
                return new DamageAllCommand(user, skill, battleManager);
            }

            switch (skill.skillType)
            {
                case SkillType.MeleeAttack:
                    return new MeleeAttackCommand(user, target, skill, battleManager);

                case SkillType.RangedAttack:
                    return new RangedAttackCommand(user, target, skill, battleManager);

                case SkillType.Heal:
                    return new HealCommand(user, target, skill, battleManager);

                case SkillType.Buff:
                    return new BuffCommand(user, target, skill, battleManager);

                case SkillType.Shield:
                    return new ShieldCommand(user, target, skill, battleManager);

                case SkillType.Special:
                    return new HealCommand(user, target, skill, battleManager);

                case SkillType.DamageAll:
                    return new DamageAllCommand(user, skill, battleManager);

                case SkillType.RangedProjectile:
                    return new ProjectileAttackCommand(user, target, skill, battleManager);

                case SkillType.DamageGlobal:
                    return new DamageAllGlobalCommand(user, skill, battleManager);

                case SkillType.LaserAttack: 
                    return new LaserAttackCommand(user, target, skill, battleManager);

                case SkillType.Chain:
                    return new ChainAttackCommand(user,target, skill, battleManager);

                case SkillType.XPassive:
                    return new XPassiveCommand(user, skill, battleManager);

                case SkillType.DebuffPunisher:
                    return new DebuffPunisherCommand(user, target, skill, battleManager);

                case SkillType.DebuffExtender:
                    return new DebuffExtenderCommand(user, target, skill, battleManager);

                case SkillType.StingRayAttack:
                    return new StingRayAttackCommand(user, target, skill, battleManager);

                default:
                    Debug.LogWarning("Skill chưa được hỗ trợ: " + skill.skillType);
                    return null;
            }
        }
    }

}

