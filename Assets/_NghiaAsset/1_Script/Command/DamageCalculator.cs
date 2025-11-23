using UnityEngine;

namespace Turnbase
{
    public static class DamageCalculator
    {
        public static int GetFinalDamage(Character user, Character target, Skill skill, BattleManager battleManager)
        {
            int offensiveStat;
            int defensiveStat;

            switch (skill.elementType)
            {
                case ElementType.Magical:
                case ElementType.Fire:
                case ElementType.Ice:
                case ElementType.Poison:
                case ElementType.Lightning:
                case ElementType.Dark:
                    offensiveStat = user.stats.magicAttack;
                    defensiveStat = target.stats.magicDefense;
                    break;

                case ElementType.Physical:
                case ElementType.None:
                default:
                    offensiveStat = user.stats.physicalAttack;
                    defensiveStat = target.stats.physicalDefense;
                    break;
            }

            //st cơ bản -> giảmt trừ phòng thủ -> khắc chế nguyên tố -> chí mạng -> final damage


            int rawDamage = offensiveStat * skill.damage;

            float defenseMultiplier = 100f / (defensiveStat + 100f);
            float damageBase = rawDamage * defenseMultiplier;

            float elementMultiplier = GetElementMultiplier(skill, target, battleManager);
            float preCritDamageFloat = damageBase * elementMultiplier;

            bool isCrit = UnityEngine.Random.Range(0, 100) < user.stats.crit;
            if (isCrit)
            {
                float critMultiplier = (float)user.stats.critDamage / 100f;
                preCritDamageFloat *= critMultiplier;
            }

            int finalDamage = Mathf.RoundToInt(preCritDamageFloat);

            if (rawDamage > 0)
            {
                finalDamage = Mathf.Max(1, finalDamage);
            }


            return finalDamage;
        }

        private static float GetElementMultiplier(Skill skill, Character target, BattleManager battleManager)
        {
            if (battleManager != null && battleManager.elementChart != null)
            {
                return battleManager.elementChart.GetMultiplier(skill.elementType, target.characterElement);
            }
            return 1.0f;
        }
    }
}