using System.Collections;
using UnityEngine;


namespace Turnbase
{
    public class AvoidState : BaseState
    {
        public AvoidState(CharacterStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnter()
        {
            Debug.LogWarning($"{stateMachine.character.name} chuyển sang Avoid.");

            var cmd = new AvoidCommand(stateMachine.character);
            stateMachine.character.StartCoroutine(ExecuteCommand(cmd));
        }

        private IEnumerator ExecuteCommand(ICommand command)
        {
            yield return stateMachine.character.StartCoroutine(command.Execute());
            stateMachine.battleManager.EndTurn(stateMachine.character);
        }

        public override void OnExit()
        {
            stateMachine.character.StopAllCoroutines();
        }
    }

}
