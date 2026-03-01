using UnityEngine;
using System.Collections;


namespace Turnbase
{
    public class TakingDamageState : BaseState
    {
        public TakingDamageState(CharacterStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnter()
        {
            stateMachine.character.StartCoroutine(HandleTakingDamage());
        }

        private IEnumerator HandleTakingDamage()
        {
            stateMachine.character.animator.Play("Hurt");

            yield return new WaitForSeconds(0.5f);

            if (stateMachine.character.isAlive)
            {
                stateMachine.SwitchState(stateMachine.waitingState);
            }
            else
            {
                stateMachine.SwitchState(stateMachine.deadState);
            }
        }

        public override void OnExit()
        {
            stateMachine.character.StopAllCoroutines();
        }
    }

}
