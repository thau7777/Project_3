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

            // 1. Lấy chỉ số cơ bản (Physical vs Magical)
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

                    foreach (var passive in user.passiveSkills)
                    {
                        if (passive is Passive_MagicPenetration magicPen)
                            defensiveStat = magicPen.GetReducedDefense(defensiveStat);
                    }
                    break;

                default: // Physical / Normal / None
                    offensiveStat = user.stats.physicalAttack;
                    defensiveStat = target.stats.physicalDefense;

                    foreach (var passive in user.passiveSkills)
                    {
                        if (passive is Passive_ArmorPenetration armorPen)
                            defensiveStat = armorPen.GetReducedDefense(defensiveStat);
                    }
                    break;
            }

            // 2. Tính Sát thương thô (Raw Damage)
            int rawDamage = offensiveStat + skill.damage;

            // Xử lý Buff đánh thường
            if (skill.manaCost <= 0)
            {
                if (user.buffManager != null && user.buffManager.basicAttackBuffTurnsRemaining > 0)
                    rawDamage += user.buffManager.basicAttackBuffAmount;

                foreach (var passive in user.passiveSkills)
                    if (passive is Passive_BasicAttackBoost boost)
                        rawDamage = boost.ApplyBoost(rawDamage);
            }

            // 3. Tính Giảm trừ theo Giáp/Kháng phép (Defense Multiplier)
            float defenseMultiplier = 100f / (defensiveStat + 100f);
            float damageBase = rawDamage * defenseMultiplier;

            // 4. ÁP DỤNG ELEMENTAL BONUS & DEFENSE (MỚI THÊM)
            float elementBonusMod = GetElementDamageBonus(user, skill.elementType);
            float elementDefenseMod = GetElementDefenseReduction(target, skill.elementType);

            // Công thức: Damage * (1 + %Bonus/100) * (1 - %Defense/100)
            damageBase = damageBase * (1f + elementBonusMod / 100f) * (1f - elementDefenseMod / 100f);

            // 5. Khắc chế hệ (Element Chart - multiplier từ BattleManager)
            float elementChartMultiplier = GetElementMultiplier(skill, target, battleManager);
            float currentDamage = damageBase * elementChartMultiplier;

            // 6. Bonus từ Buff đặc biệt (ví dụ: Hiến tế)
            if (user.buffManager != null && user.buffManager.lifeForPowerTurnsRemaining > 0)
            {
                currentDamage += user.buffManager.lifeForPowerBonusDamage;
            }

            // 7. Tính Chí mạng (Critical)
            IsLastHitCrit = UnityEngine.Random.Range(0, 100) < user.stats.critChance;
            if (IsLastHitCrit)
            {
                float critMultiplier = (float)user.stats.critMult / 100f;
                currentDamage *= critMultiplier;
            }
            // 8. Bounus Dmg Break
            if(user.debuffManager.breakTurnsRemaining > 0)
            {
                currentDamage *= 1.5f;
            }

            // Kết quả cuối cùng
            int finalDamage = Mathf.RoundToInt(currentDamage);
            return rawDamage > 0 ? Mathf.Max(1, finalDamage) : 0;
        }

        // Hàm bổ trợ lấy % Bonus sát thương theo hệ
        private static int GetElementDamageBonus(Character user, ElementType type)
        {
            return type switch
            {
                ElementType.Fire => user.stats.fireDamageBonus,
                ElementType.Lightning => user.stats.lightningDamageBonus,
                ElementType.Frost => user.stats.frostDamageBonus,
                ElementType.Dark => user.stats.darkDamageBonus,
                ElementType.Holy => user.stats.holyDamageBonus,
                ElementType.Water => user.stats.waterDamageBonus,
                ElementType.Poison => user.stats.poisonDamageBonus,
                _ => 0
            };
        }

        // Hàm bổ trợ lấy % Giảm sát thương theo hệ
        private static int GetElementDefenseReduction(Character target, ElementType type)
        {
            return type switch
            {
                ElementType.Fire => target.stats.fireDefense,
                ElementType.Lightning => target.stats.lightningDefense,
                ElementType.Frost => target.stats.frostDefense,
                ElementType.Dark => target.stats.darkDefense,
                ElementType.Holy => target.stats.holyDefense,
                ElementType.Water => target.stats.waterDefense,
                ElementType.Poison => target.stats.poisonDefense,
                _ => 0
            };
        }

        private static float GetElementMultiplier(Skill skill, Character target, BattleManager battleManager)
        {
            if (battleManager != null && battleManager.elementChart != null)
                return battleManager.elementChart.GetMultiplier(skill.elementType, target.characterElement);
            return 1.0f;
        }
    }
}