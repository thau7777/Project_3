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
            inputReader.spaceStationActions.onActve += OnActive;
        }

        private void OnDisable()
        {
            inputReader.spaceStationActions.onMove -= OnMove;
            inputReader.spaceStationActions.onInteract -= OnInteract;
            inputReader.spaceStationActions.onEsc -= OnEsc;
            inputReader.spaceStationActions.onActve -= OnActive;
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

        private void OnActive()
        {
            EventBus<ScifitableActiveEvent>.Raise(new ScifitableActiveEvent());
        }
    }
}