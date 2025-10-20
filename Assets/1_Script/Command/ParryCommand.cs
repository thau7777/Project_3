using System.Collections;
using UnityEngine;


namespace Turnbase
{
    public class ParryCommand : ICommand
    {
        private Character character;

        public ParryCommand(Character character)
        {
            this.character = character;
        }

        public IEnumerator Execute()
        {
            Debug.Log($"{character.name} bắt đầu Parry!");

            character.animator.SetTrigger("Parry");

            yield return new WaitForSeconds(0.8f);

            character.stateMachine.SwitchState(character.stateMachine.waitingState);

            Debug.Log($"{character.name} kết thúc Parry.");

            Time.timeScale = 1f;
        }
    }

}
