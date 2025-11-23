using UnityEngine;
using System.Collections;
using Turnbase;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Rule_AttackGrowth", menuName = "Battle Rules/Attack Growth Per Turn", order = 2)]
    public class RuleExample_AttackGrowth : BattleRule
    {
        [Header("Cấu hình Luật")]
        public int baseAttackIncrease = 100;
        public float percentageAttackIncrease = 0.10f;

        public override IEnumerator ExecuteRule(BattleManager battleManager, Character characterToAct)
        {
            Debug.Log($"[BATTLE RULE] Luật {ruleName} được áp dụng lên {characterToAct.name}");

            if (characterToAct != null && characterToAct.isAlive && characterToAct.stats != null)
            {
                int percentageBonus = Mathf.RoundToInt(characterToAct.stats.physicalAttack * percentageAttackIncrease);

                characterToAct.stats.physicalAttack += baseAttackIncrease;

                characterToAct.stats.physicalAttack += percentageBonus;

                Debug.Log($"- Tăng {baseAttackIncrease} ATK cơ bản.");

                Debug.Log($"- Tăng {percentageAttackIncrease * 100}% ATK ");
            }

            yield return new WaitForSeconds(0.1f);
        }

        public override void ResetRule(BattleManager battleManager)
        {
            Debug.Log($"[BATTLE RULE] Kết thúc luật {ruleName}.");
        }
    }
}