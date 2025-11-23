using System.Collections;
using Turnbase;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Rule_DotDamage", menuName = "Battle Rules/Fixed DoT Damage Per Turn", order = 1)]
    public class RuleExample_DamageOverTime : BattleRule
    {
        [Header("Cấu hình Luật")]
        public int damagePerTurn = 10;

        public ElementType element = ElementType.None;

        public override IEnumerator ExecuteRule(BattleManager battleManager, Character characterToAct)
        {
            Debug.Log($"[BATTLE RULE] ON");
            if (characterToAct != null && characterToAct.isAlive)
            {
                characterToAct.TakeDamage(damagePerTurn, element);
            }
            yield return new WaitForSeconds(0.2f);
        }

        public override void ResetRule(BattleManager battleManager)
        {
            Debug.Log($"[BATTLE RULE] Kết thúc luật {ruleName}.");
        }
    }
}
