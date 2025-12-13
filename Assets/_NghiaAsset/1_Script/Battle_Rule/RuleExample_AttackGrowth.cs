using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Turnbase;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Rule_AttackGrowth", menuName = "Battle Rules/Attack Growth Per Turn", order = 2)]
    public class RuleExample_AttackGrowth : BattleRule
    {
        [Header("Cấu hình Luật")]
        public int baseAttackIncrease = 100;
        public float percentageAttackIncrease = 0.10f;

        private IEnumerable<Character> GetTargetCombatants(BattleManager battleManager)
        {
            var baseTargets = battleManager.allCombatants
                .Where(c => c != null && c.isAlive && !c.isVirtualTracker && c.stats != null);

            switch (targetScope)
            {
                case TargetScope.Players:
                    return baseTargets.Where(c => c.isPlayer);
                case TargetScope.Enemies:
                    return baseTargets.Where(c => !c.isPlayer);
                case TargetScope.AllCombatants:
                default:
                    return baseTargets;
            }
        }

        public override IEnumerator ExecuteRule(BattleManager battleManager, Character characterToAct)
        {
            if (!(characterToAct is RoundTracker))
            {
                yield break;
            }

            Debug.Log($"[BATTLE RULE] Kích hoạt Luật {ruleName} (Mục tiêu: {targetScope}).");

            List<Character> targetCombatants = GetTargetCombatants(battleManager).ToList();

            if (targetCombatants.Count == 0)
            {
                yield break;
            }

            foreach (var character in targetCombatants)
            {
                int percentageBonus = Mathf.RoundToInt(character.stats.physicalAttack * percentageAttackIncrease);
                int totalIncrease = baseAttackIncrease + percentageBonus;

                character.stats.physicalAttack += totalIncrease;

                Debug.Log($"- Đã buff {character.name}: ATK +{totalIncrease} (Mới: {character.stats.physicalAttack}).");
            }

            battleManager.uiManager?.UpdateAllCharacterUIs(battleManager.allCombatants);

            yield return new WaitForSeconds(0.2f);
        }

        public override void ResetRule(BattleManager battleManager)
        {
            Debug.Log($"[BATTLE RULE] Kết thúc luật {ruleName}.");
        }
    }
}