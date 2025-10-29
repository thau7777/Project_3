using UnityEngine;

namespace Turnbase
{
    public class StunnedState : BaseState 
    {
        public StunnedState(CharacterStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnter()
        {
            stateMachine.character.animator.SetBool("IsIdle", false);
            Debug.Log(stateMachine.gameObject.name + " đã bị CHOÁNG.");
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            Debug.Log(stateMachine.gameObject.name + " đã hết CHOÁNG.");

        }
    }
}