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
                if (bm.activeCharacter == null && !isProcessingTurn)
                {
                    bool someoneReady = false;
                    List<Character> currentCombatants = bm.allCombatants.ToList();

                    foreach (var combatant in currentCombatants)
                    {
                        if (combatant == null || !combatant.isAlive) continue;

                        combatant.actionGauge += combatant.stats.speed * Time.deltaTime;

                        if (combatant.actionGauge >= 100)
                        {
                            if (combatant.stateMachine != null && combatant.stateMachine.currentState is WaitingState)
                            {
                                someoneReady = true;
                            }
                        }
                    }

                    if (bm.turnOrderUI != null) bm.turnOrderUI.UpdateActionGaugeUI(bm.allCombatants);

                    if (someoneReady)
                    {
                        var readyChar = currentCombatants
                            .Where(c => c != null && c.isAlive && c.actionGauge >= 100 && c.stateMachine.currentState is WaitingState)
                            .OrderByDescending(c => c.actionGauge)
                            .FirstOrDefault();

                        if (readyChar != null)
                        {
                            isProcessingTurn = true;
                            StartCoroutine(bm.AdvanceTurn(readyChar));
                        }
                    }
                }
                yield return null;
            }
        }
    }
}