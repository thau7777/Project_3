using UnityEngine;


namespace Turnbase
{
    public class BattleBuffManager : MonoBehaviour
    {

        public void ProcessPassiveSkills(Character character)
        {
            if (!character.isAlive || character.passiveSkills == null)
            {
                return;
            }

            foreach (var passiveSkills in character.passiveSkills)
            {
                if (passiveSkills == null || passiveSkills.applicationTiming != PassiveTiming.OnTurnStart)
                {
                    continue;
                }

                int initialMaxHP = character.stats.maxHP;

                switch (passiveSkills.effectType)
                {
                    case PassiveEffectType.HealPerTurn:
                        if (character.stats == null) continue;

                        int percentHeal = Mathf.RoundToInt(character.stats.maxHP * passiveSkills.effectValuePercentage);

                        int fixedHeal = Mathf.RoundToInt(passiveSkills.effectValue);

                        int totalHealAmount = percentHeal + fixedHeal;

                        if (totalHealAmount > 0)
                        {
                            character.Heal(totalHealAmount);
                            Debug.Log($"{character.name} hồi phục {totalHealAmount} HP (Bao gồm {passiveSkills.effectValuePercentage * 100}% HP và {fixedHeal} HP cố định) nhờ kỹ năng thụ động {passiveSkills.skillName}.");
                        }
                        break;

                    case PassiveEffectType.ManaPerTurn:
                        if(character.stats == null) continue;

                        int percentMana = Mathf.RoundToInt(character.stats.maxMP * passiveSkills.effectValuePercentage);

                        int fixedMana = Mathf.RoundToInt(passiveSkills.effectValue);

                        int totalManaAmout = percentMana + fixedMana;

                        if(totalManaAmout > 0)
                        {
                            character.RestoreMana(totalManaAmout);
                            Debug.Log($"{character.name} hồi phục {totalManaAmout} MP (Bao gồm {passiveSkills.effectValuePercentage * 100}% MP và {fixedMana} MP cố định) nhờ kỹ năng thụ động {passiveSkills.skillName}.");

                        }
                        break;

                    case PassiveEffectType.IncreasePermanentMaxHP:

                        int percentIncrease = Mathf.RoundToInt(initialMaxHP * passiveSkills.effectValuePercentage);

                        int fixedIncrease = Mathf.RoundToInt(passiveSkills.effectValue);

                        int totalIncreaseAmount = percentIncrease + fixedIncrease;

                        if (totalIncreaseAmount > 0)
                        {
                            character.stats.maxHP += totalIncreaseAmount;

                            character.stats.currentHP += totalIncreaseAmount;

                            Debug.Log($"{character.name} tăng tối đa HP vĩnh viễn thêm {totalIncreaseAmount} HP (Bao gồm {passiveSkills.effectValuePercentage * 100}% và {fixedIncrease} HP cố định) nhờ kỹ năng thụ động {passiveSkills.skillName}.");
                        }
                        break;
                    case PassiveEffectType.BonusPhysicalAttack:
                        if (character.stats == null) continue;
                        int bonusPA = Mathf.RoundToInt(passiveSkills.effectValue);
                        int bonusPAPercent = Mathf.RoundToInt(character.stats.physicalAttack * passiveSkills.effectValuePercentage);
                        character.stats.physicalAttack += bonusPA + bonusPAPercent;
                        Debug.Log($"{character.name} nhận thêm {bonusPA + bonusPAPercent} Công vật lý nhờ kỹ năng thụ động {passiveSkills.skillName}.");
                        break;


                    default:
                        break;
                }
            }
        }

        public void ProcessOnBattleStartPassives(Character character)
        {
            if (character == null || character.passiveSkills == null || character.stats == null)
            {
                return;
            }
            int initialMaxHP = character.stats.maxHP;

            foreach (var passiveSkills in character.passiveSkills)
            {
                if (passiveSkills == null || passiveSkills.applicationTiming != PassiveTiming.OnBattleStart)
                {
                    continue;
                }

                switch (passiveSkills.effectType)
                {
                    case PassiveEffectType.IncreasePermanentMaxHP:

                        int percentIncrease = Mathf.RoundToInt(initialMaxHP * passiveSkills.effectValuePercentage);

                        int fixedIncrease = Mathf.RoundToInt(passiveSkills.effectValue);

                        int totalIncreaseAmount = percentIncrease + fixedIncrease;

                        if (totalIncreaseAmount > 0)
                        {
                            character.stats.maxHP += totalIncreaseAmount;

                            character.stats.currentHP += totalIncreaseAmount;

                            Debug.Log($"{character.name} tăng tối đa HP vĩnh viễn thêm {totalIncreaseAmount} HP (Bao gồm {passiveSkills.effectValuePercentage * 100}% và {fixedIncrease} HP cố định) nhờ kỹ năng thụ động {passiveSkills.skillName}.");
                        }
                        break;

                    case PassiveEffectType.BonusPhysicalAttack:
                        if (character.stats == null) continue;
                        int bonusPA = Mathf.RoundToInt(passiveSkills.effectValue);
                        int bonusPAPercent = Mathf.RoundToInt(character.stats.physicalAttack * passiveSkills.effectValuePercentage);
                        character.stats.physicalAttack += bonusPA + bonusPAPercent;
                        Debug.Log($"{character.name} nhận thêm {bonusPA + bonusPAPercent} Công vật lý nhờ kỹ năng thụ động {passiveSkills.skillName}.");
                        break;

                    default:
                        break;
                }
            }
        }


    }

}
