using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Turnbase;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Rule_DotDamage", menuName = "Battle Rules/Fixed DoT Damage Per Turn", order = 1)]
    public class RuleExample_DamageOverTime : BattleRule
    {
        [Header("Cấu hình Luật")]
        public int damagePerTurn = 10;
        public ElementType element = ElementType.None;

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

            Debug.Log($"[BATTLE RULE] Kích hoạt Luật {ruleName} (Sát thương: {damagePerTurn}, Mục tiêu: {targetScope}).");

            List<Character> targetCombatants = GetTargetCombatants(battleManager).ToList();

            if (targetCombatants.Count == 0)
            {
                yield break;
            }

            foreach (var character in targetCombatants)
            {
                Debug.Log($"- Áp dụng sát thương DoT lên {character.name}: {damagePerTurn} ({element}).");
                character.TakeDamage(damagePerTurn, element);
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