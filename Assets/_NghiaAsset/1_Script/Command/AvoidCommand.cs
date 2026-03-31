using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class AvoidCommand : ICommand
    {
        private Character character;
        private float moveDistance = 2f; 
        private float moveDuration = 0.2f; 

        public AvoidCommand(Character character)
        {
            this.character = character;
        }

        public IEnumerator Execute()
        {
            Debug.LogWarning($"{character.name} thực hiện né và lùi lại!");

            ParryPopup parryPopupComponent = character.GetComponent<ParryPopup>();
            parryPopupComponent.ShowAvoidPopup(character);

            Vector3 startPos = character.initialPosition;
            Vector3 backwardPos = startPos + (character.transform.forward * -moveDistance);

            yield return MoveCharacter(character.transform, startPos, backwardPos, moveDuration);

            yield return new WaitForSeconds(1.5f);

            yield return MoveCharacter(character.transform, backwardPos, startPos, moveDuration);

            character.stateMachine.SwitchState(character.stateMachine.waitingState);
        }

        private IEnumerator MoveCharacter(Transform t, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                t.position = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            t.position = to;
        }
    }
}