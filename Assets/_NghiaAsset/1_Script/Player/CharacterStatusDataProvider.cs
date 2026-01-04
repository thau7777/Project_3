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


            if (buffManager.shieldTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Shield",
                    TurnsRemaining = buffManager.shieldTurnsRemaining,
                    Detail = $"{stats.currentShield} Shield",
                    IsBuff = true,
                    Icon = buffManager.shieldIcon

                });
            }

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
                int buffAmount = stats.agility - buffManager.originalBaseAgility;
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
                    Detail = $"+{buffAmount}",
                    IsBuff = true,
                    Icon = buffManager.magicalDefenseBuffIcon
                });
            }



            if (debuffManager.burnTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Burn",
                    TurnsRemaining = debuffManager.burnTurnsRemaining,
                    Detail = $"{debuffManager.burnDamagePerTurn} Damage/turn",
                    IsBuff = false,
                    Icon = debuffManager.burnIcon
                });
            }

            if (debuffManager.poisonTurnsRemaining > 0)
            {
                effects.Add(new StatusEffectData
                {
                    Name = "Poison",
                    TurnsRemaining = debuffManager.poisonTurnsRemaining,
                    Detail = $"{debuffManager.poisonDamagePerTurn} Damage/turn",
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

            return effects;
        }
        #endregion    

    }

}