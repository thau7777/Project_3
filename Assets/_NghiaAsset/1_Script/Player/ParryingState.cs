using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;



namespace Turnbase
{
    public class ParryingState : BaseState
    {
        private Character attacker;
        public ParryingState(CharacterStateMachine stateMachine, Character attacker = null) : base(stateMachine)
        {
            this.attacker = attacker;
        }

        public override void OnEnter()
        {
            Debug.Log($"{stateMachine.character.name} chuyển sang ParryingState.");

            var cmd = new ParryCommand(stateMachine.character, attacker);
            stateMachine.character.StartCoroutine(ExecuteCommand(cmd));
        }

        private IEnumerator ExecuteCommand(ICommand command)
        {
            yield return stateMachine.character.StartCoroutine(command.Execute());
            stateMachine.battleManager.EndTurn(stateMachine.character);
        }

        public override void OnExit()
        {
            Time.timeScale = 1f;

            CameraAction.instance.NormalCamera(stateMachine.character);

            stateMachine.character.StopAllCoroutines();
        }
    }

}
