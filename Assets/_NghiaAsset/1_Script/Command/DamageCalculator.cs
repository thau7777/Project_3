using UnityEngine;

namespace Turnbase
{
    public static class DamageCalculator
    {
        public static bool IsLastHitCrit;
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
                case ElementType.Water:
                case ElementType.Frost:
                case ElementType.Holy:
                    offensiveStat = user.stats.magicAttack;
                    defensiveStat = target.stats.magicDefense;

                    //xuyên phòng thủ phép 
                    foreach (var passive in user.passiveSkills)
                    {
                        if (passive is Passive_MagicPenetration magicPen)
                        {
                            defensiveStat = magicPen.GetReducedDefense(defensiveStat);
                        }
                    }
                    break;

                case ElementType.Physical:
                case ElementType.None:
                    offensiveStat = user.stats.physicalAttack;
                    defensiveStat = target.stats.physicalDefense;

                    //xuyên phong thủ vật lý
                    foreach (var passive in user.passiveSkills)
                    {
                        if (passive is Passive_ArmorPenetration armorPen)
                        {
                            defensiveStat = armorPen.GetReducedDefense(defensiveStat);
                        }
                    }
                    break;

                default:
                    offensiveStat = user.stats.physicalAttack;
                    defensiveStat = target.stats.physicalDefense;
                    break;
            }

            //st cơ bản -> giảmt trừ phòng thủ -> khắc chế nguyên tố -> chí mạng -> final damage


            int rawDamage = offensiveStat * skill.damage;

            if (skill.manaCost <= 0)
            {
                if (user.buffManager != null && user.buffManager.basicAttackBuffTurnsRemaining > 0)
                {
                    rawDamage += user.buffManager.basicAttackBuffAmount;
                    Debug.Log($"[BUFF] Cộng thêm {user.buffManager.basicAttackBuffAmount} sát thương vào đòn đánh thường.");
                }

                foreach (var passive in user.passiveSkills)
                {
                    if (passive is Passive_BasicAttackBoost boost)
                    {
                        rawDamage = boost.ApplyBoost(rawDamage);
                    }
                }
            }

            float defenseMultiplier = 100f / (defensiveStat + 100f);
            float damageBase = rawDamage * defenseMultiplier;

            float elementMultiplier = GetElementMultiplier(skill, target, battleManager);
            float preCritDamageFloat = damageBase * elementMultiplier;

            IsLastHitCrit = UnityEngine.Random.Range(0, 100) < user.stats.critChance;
            // Thêm vào ngay sau dòng tính isCrit
            Debug.Log($"[CALC] Tỉ lệ Crit: {user.stats.critChance} | Kết quả Roll: {IsLastHitCrit}");

            if (IsLastHitCrit)
            {
                float oldDamage = preCritDamageFloat;
                float critMultiplier = (float)user.stats.critMult / 100f;
                preCritDamageFloat *= critMultiplier;
                Debug.Log($"[CALC] Đã nhân Crit: {oldDamage} -> {preCritDamageFloat} (Multiplier: {critMultiplier})");
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

        /// <summary>
        /// Công thức giảm damage theo chỉ số chống chịu (Armor hoặc MR).
        /// </summary>
        private static float GetReduction(float stat)
        {
            if (stat >= 0)
                return 100f / (100f + stat);
            else
                return 2f - (100f / (100f - stat)); // xử lý khi xuyên giáp khiến stat âm
        }

        /// <summary>
        /// Công thức tăng damage theo chỉ số tấn công (AD hoặc AP).
        /// </summary>
        private static float GetBonusDamage(float stat)
        {
            if (stat >= 0)
                return 1f + (stat / 100f);
            else
                return 1f - (stat / 100f);
        }
    }
}