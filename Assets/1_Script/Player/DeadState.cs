using UnityEngine;
using System.Collections;



namespace Turnbase
{
    public class DeadState : BaseState
    {
        public DeadState(CharacterStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnter()
        {
            Debug.Log($"{stateMachine.character.name} chuyển sang DeadState. Đã chết!");

            stateMachine.character.StopAllCoroutines();

            Collider col = stateMachine.character.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            stateMachine.character.animator.SetTrigger("Die");

            stateMachine.character.StartCoroutine(HandleDeath());
        }

        private IEnumerator HandleDeath()
        {
            yield return new WaitForSeconds(3f);

            if (stateMachine.battleManager != null)
            {
                if (stateMachine.battleManager.activeCharacter == stateMachine.character)
                {
                    stateMachine.battleManager.activeCharacter = null;
                    stateMachine.battleManager.isProcessingTurn = false;
                }

                stateMachine.battleManager.RemoveCombatant(stateMachine.character);
            }

            GameObject.Destroy(stateMachine.character.gameObject);
        }

        public override void OnExit()
        {

        }
    }
}
