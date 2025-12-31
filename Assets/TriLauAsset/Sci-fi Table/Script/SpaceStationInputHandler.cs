using UnityEngine;


namespace MyRule
{
    public class SpaceStationInputHandler : MonoBehaviour
    {
        public InputReader inputReader;

        private void OnEnable()
        {
            inputReader.SwitchActionMap(ActionMap.SpaceStation);

            inputReader.spaceStationActions.onMove += OnMove;
            inputReader.spaceStationActions.onInteract += OnInteract;
            inputReader.spaceStationActions.onEsc += OnEsc;
        }

        private void OnDisable()
        {
            inputReader.spaceStationActions.onMove -= OnMove;
            inputReader.spaceStationActions.onInteract -= OnInteract;
            inputReader.spaceStationActions.onEsc -= OnEsc;
        }

        private void OnMove(Vector2 movement)
        {
            EventBus<ScifiMouseMoveEvent>.Raise(new ScifiMouseMoveEvent(movement));
        }

        private void OnInteract()
        {
            EventBus<ScifitableInteractEvent>.Raise(new ScifitableInteractEvent());
        }

        private void OnEsc()
        {
            EventBus<ScifitableEscEvent>.Raise(new ScifitableEscEvent());
        }
    }
}