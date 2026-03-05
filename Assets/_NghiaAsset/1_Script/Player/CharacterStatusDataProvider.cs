using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Turnbase
{
    public class CharacterStatusDataProvider : MonoBehaviour
    {
        [SerializeField] private Character character;

        private CharacterStats stats => character.stats;
        private CharacterBuffManager buffManager => character.buffManager;
        private CharacterDebuffManager debuffManager => character.debuffManager;

        private void Awake()
        {
            if (character == null)
            {
                character = GetComponent<Character>();
            }
        }

        #region Lấy Dữ liệu Hiệu ứng (Cho CharacterStatUI)
        public List<StatusEffectData> GetActiveStatusEffects()
        {
            List<StatusEffectData> effects = new List<StatusEffectData>();

            if (buffManager == null || debuffManager == null) return effects;


            ////if (buffManager.shieldTurnsRemaining > 0)
            ////{
            ////    effects.Add(new StatusEffectData
            ////    {
            ////        Name = "Shield",
            ////        TurnsRemaining = buffManager.shieldTurnsRemaining,
            ////        Detail = $"{stats.currentShield} Shield",
            ////        IsBuff = true,
            ////        Icon = buffManager.shieldIcon

            ////    });
            ////}

            if (buffManager.attackBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.physicalAttack - buffManager.originalBaseAttack;
                effects.Add(new StatusEffectData
                {
                    Name = "Increase P.Attack",
                    TurnsRemaining = buffManager.attackBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.attackBuffIcon
                });
            }

            if (buffManager.defenseBuffTurnsRemaining > 0)
            {
                int buffAmount = buffManager.defenseBuffAmount;
                effects.Add(new StatusEffectData
                {
                    Name = "Increase P.Defense",
                    TurnsRemaining = buffManager.defenseBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.defenseBuffIcon
                });
            }

            if (buffManager.agilityBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.speed - buffManager.originalBaseAgility;
                effects.Add(new StatusEffectData
                {
                    Name = "Increase Agility",
                    TurnsRemaining = buffManager.agilityBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.agilityBuffIcon
                });
            }

            if (buffManager.maxHPBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.maxHP - buffManager.originalBaseMaxHP;
                effects.Add(new StatusEffectData
                {
                    Name = "Increase MaxHP",
                    TurnsRemaining = buffManager.maxHPBuffTurnsRemaining,
                    Detail = $"+{buffAmount} MaxHP",
                    IsBuff = true,
                    Icon = buffManager.maxHPBuffIcon
                });
            }

            if (buffManager.magicalAttackBuffTurnsRemaining > 0)
            {
                int buffAmount = stats.magicAttack - buffManager.magicalOriginalBaseAttack;
                effects.Add(new StatusEffectData
                {
                    Name = "Increase M.Attack",
                    TurnsRemaining = buffManager.magicalAttackBuffTurnsRemaining,
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.magicalAttackBuffIcon
                });
            }

            if (buffManager.magicalDefenseBuffTurnsRemaining > 0)
            {
                int buffAmount = buffManager.magicalDefenseBuffAmount;
                effects.Add(new StatusEffectData
                {
                    Name = "Increase M.Defense",
                    TurnsRemaining = buffManager.magicalDefenseBuffTurnsRemaining,
                    Detail = $"+{buffAmount} M.DEF",
                    IsBuff = true,
                    Icon = buffManager.magicalDefenseBuffIcon
                });
            }



            if (debuffManager.burnTurnsRemaining > 0)
            {
                int estimatedDmg = debuffManager.GetEstimatedBurnDamage();
                effects.Add(new StatusEffectData
                {
                    Name = "Burn",
                    TurnsRemaining = debuffManager.burnTurnsRemaining,
                    Detail = $"{estimatedDmg} Damage/turn",
                    IsBuff = false,
                    Icon = debuffManager.burnIcon
                });
            }

            if (debuffManager.poisonTurnsRemaining > 0)
            {
                float debuffValue = debuffManager.poisonReductionPercentage * 100f;
                effects.Add(new StatusEffectData
                {
                    Name = "Poison",
                    TurnsRemaining = debuffManager.poisonTurnsRemaining,
                    Detail = $"{debuffValue}% DEF Reduction + Weakness Break Efficiency",
                    IsBuff = false,
                    Icon = debuffManager.poisonIcon
                });
            }

            if (debuffManager.stunTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Stun",
                    TurnsRemaining = debuffManager.stunTurnsRemaining,
                    Detail = "Unable to act",
                    IsBuff = false,
                    Icon = debuffManager.stunIcon
                });
            }

            if (debuffManager.defReductionTurnsRemaining > 0)
            {
                int percentageValue = Mathf.RoundToInt(debuffManager.defReductionPercentage * 100);
                string detailText = $"{percentageValue}% Defense";

                effects.Add(new StatusEffectData
                {
                    Name = "Decrease All Defense",
                    TurnsRemaining = debuffManager.defReductionTurnsRemaining,
                    Detail = detailText,
                    IsBuff = false,
                    Icon = debuffManager.defReductionIcon
                });
            }

            if (debuffManager.speedReductionTurnsRemaining > 0)
            {
                int percentageValue = Mathf.RoundToInt(debuffManager.speedReductionPercentage * 100);

                effects.Add(new StatusEffectData
                {
                    Name = "Decrease Speed",
                    TurnsRemaining = debuffManager.speedReductionTurnsRemaining,
                    Detail = $"-{percentageValue}% Speed",
                    IsBuff = false,
                    Icon = debuffManager.speedReductionIcon
                });
            }

            if (debuffManager.breakTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Armor Break",
                    TurnsRemaining = debuffManager.breakTurnsRemaining,
                    Detail = "Increased damage taken",
                    IsBuff = false,
                    Icon = debuffManager.breakIcon
                });
            }

            if(debuffManager.paralysisTurnsRemaining > 0)
            {
                int debuffReduction = Mathf.RoundToInt(debuffManager.paralysisDamageReduction);
                effects.Add(new StatusEffectData
                {
                    Name = "Paralysis",
                    TurnsRemaining = debuffManager.paralysisTurnsRemaining,
                    Detail = $"{debuffReduction}% DMG Reduction",
                    IsBuff = false,
                    Icon = debuffManager.paralysisIcon
                });

            }

            if (buffManager.basicAttackBuffTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Atk Boost",
                    TurnsRemaining = buffManager.basicAttackBuffTurnsRemaining,
                    Detail = $"+{buffManager.basicAttackBuffAmount} Normal Dmg",
                    IsBuff = true,
                    Icon = buffManager.basicAttackBuffIcon
                });
            }

            if (buffManager.splashAttackTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Splash Attack",
                    TurnsRemaining = buffManager.splashAttackTurnsRemaining,
                    Detail = "Splash Attack All",
                    IsBuff = true,
                    Icon = buffManager.splashAttackIcon 
                });
            }

            if (buffManager.hasDivineShield)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Divine Shield",
                    TurnsRemaining = 999,
                    Detail = "Divine Shield",
                    IsBuff = true,
                    Icon = buffManager.divineShieldIcon
                });
            }

            if (character != null && character.passiveSkills != null)
            {
                foreach (var passive in character.passiveSkills)
                {
                    if (passive == null) continue;

                    effects.Add(new StatusEffectData
                    {
                        Name = passive.skillName,
                        TurnsRemaining = +999,
                        Detail = passive.description,
                        IsBuff = true,
                        Icon = passive.icon 
                    });
                }
            }

            if(buffManager.lifeForPowerTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Life For Power",
                    TurnsRemaining = buffManager.lifeForPowerTurnsRemaining,
                    Detail = $"+{buffManager.lifeForPowerBonusDamage}% Bonus Dmg",
                    IsBuff = true,
                    Icon = buffManager.lifeForPowerIcon
                });
            }

            return effects;
        }
        #endregion    

    }

}