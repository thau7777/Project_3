using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Turnbase
{
    public class BattleTurnHandler : MonoBehaviour
    {
        private BattleManager bm;
        public bool isProcessingTurn = false;

        public void Initialize(BattleManager manager) => bm = manager;

        public IEnumerator UpdateActionGaugeRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            while (true)
            {
                if (bm != null && !bm.isProcessingTurn && bm.activeCharacter == null && !isProcessingTurn)
                {
                    List<Character> currentCombatants = bm.allCombatants
                        .Where(c => c != null && c.isAlive && c.stateMachine.currentState is WaitingState)
                        .ToList();

                    if (currentCombatants.Count > 0)
                    {
                        var readyChar = currentCombatants
                            .Where(c => c.actionGauge >= 99.9f)
                            .OrderByDescending(c => c.actionGauge)
                            .FirstOrDefault();

                        if (readyChar != null)
                        {
                            isProcessingTurn = true;
                            yield return StartCoroutine(bm.AdvanceTurn(readyChar));
                        }
                        else
                        {
                            float minTimeToReachLimit = currentCombatants
                                .Select(c => (100f - c.actionGauge) / Mathf.Max(1f, c.stats.speed))
                                .Min();

                            float timeStep = Mathf.Max(0, minTimeToReachLimit);

                            foreach (var combatant in currentCombatants)
                            {
                                combatant.actionGauge += combatant.stats.speed * timeStep;
                            }

                            if (bm.turnOrderUI != null) bm.turnOrderUI.UpdateActionGaugeUI(bm.allCombatants);
                        }
                    }
                }
                yield return null;
            }
        }
    }
}