using UnityEngine;
using System.Collections;
using Turnbase;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Rule_DotDamage", menuName = "Battle Rules/Fixed DoT Damage Per Turn", order = 1)]
    public class RuleExample_DamageOverTime : BattleRule
    {
        [Header("Cấu hình Luật")]
        public int damagePerTurn = 10;

        public override IEnumerator ExecuteRule(BattleManager battleManager)
        {
            Debug.Log($"[BATTLE RULE] Kích hoạt luật: {ruleName} - Mất {damagePerTurn} HP mỗi lượt.");

            foreach (Character combatant in battleManager.allCombatants)
            {
                if (combatant.isAlive)
                {
                    combatant.TakeDamage(damagePerTurn);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        public override void ResetRule(BattleManager battleManager)
        {
            Debug.Log($"[BATTLE RULE] Kết thúc luật {ruleName}.");
        }
    }
}
