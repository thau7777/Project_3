using UnityEngine;
using UnityEngine.UI;

public static class DamageCalculator
{
    public static int CalculateDamageByStats(CharacterStats attackerStats, CharacterStats defenderStats, bool isMagicAttack, int baseDamage, ElementalType attackElementalType, bool dealTrueDamage)
    {
        float finalDamage = baseDamage;

        finalDamage += isMagicAttack ? attackerStats.MagicAttackDamage : attackerStats.AttackDamage;

        if (!dealTrueDamage)
        {
            float defenseMultiplier = 100f / (100f + (isMagicAttack ? defenderStats.MagicDefense : defenderStats.PhysicalDefense));
            finalDamage *= defenseMultiplier;

            float elementalDamage = GetElementalDamageStat(attackElementalType, attackerStats);
            float elementalDefense = GetElementalDefenseStat(attackElementalType, defenderStats);
            float elementMultiplier = (1f + elementalDamage / 100f) * (1f - elementalDefense / 100f);
            finalDamage *= elementMultiplier;
        }
        else
        {
            float elementalDamage = GetElementalDamageStat(attackElementalType, attackerStats);
            finalDamage *= (1f + elementalDamage / 100f);
        }

        var elementalDamageMultiplier = ElementalManager.Instance.GetDamageMultiplier(attackElementalType, defenderStats.ElementalType);
        finalDamage *= elementalDamageMultiplier;

        return Mathf.RoundToInt(finalDamage);
    }

    private static float GetElementalDamageStat(ElementalType elementalType, CharacterStats characterStats)
    {
        float value;
        switch(elementalType)
        {
            case ElementalType.Fire:
                value = characterStats.FireDamage;
                break;
            case ElementalType.Water:
                value = characterStats.WaterDamage;
                break;
            case ElementalType.Frost:
                value = characterStats.FrostDamage;
                break;
            case ElementalType.Lightning:
                value = characterStats.LightningDamage;
                break;
            case ElementalType.Holy:
                value = characterStats.HolyDamage;
                break;
            case ElementalType.Dark:
                value = characterStats.DarkDamage;
                break;
            case ElementalType.Poison:
                value = characterStats.PoisonDamage;
                break;
            default:
                value = 1;
                break;
        }

        return value;
    }

    private static float GetElementalDefenseStat(ElementalType elementalType, CharacterStats characterStats)
    {
        float value;
        switch(elementalType)
        {
            case ElementalType.Fire:
                value = characterStats.FireDefense;
                break;
            case ElementalType.Water:
                value = characterStats.WaterDefense;
                break;
            case ElementalType.Frost:
                value = characterStats.FrostDefense;
                break;
            case ElementalType.Lightning:
                value = characterStats.LightningDefense;
                break;
            case ElementalType.Holy:
                value = characterStats.HolyDefense;
                break;
            case ElementalType.Dark:
                value = characterStats.DarkDefense;
                break;
            case ElementalType.Poison:
                value = characterStats.PoisonDefense;
                break;
            default:
                value = 1;
                break;
        }
        return value;
    }
}
