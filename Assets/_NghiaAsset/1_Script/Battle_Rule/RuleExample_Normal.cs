using System.Collections;
using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Normal Rule", menuName = "Battle Rules/Normal Per Turn", order = 1)]
    public class RuleExample_Normal : BattleRule
    {
        public override IEnumerator ExecuteRule(BattleManager battleManager, Character characterToAct)
        {
            if (!(characterToAct is RoundTracker))
            {
                yield break;
            }

            Debug.Log("Normal Rule: Không có gì xảy ra trong vòng này.");
        }

        public override void ResetRule(BattleManager battleManager)
        {
            Debug.Log($"[BATTLE RULE] Kết thúc luật {ruleName}.");
        }

    }

}