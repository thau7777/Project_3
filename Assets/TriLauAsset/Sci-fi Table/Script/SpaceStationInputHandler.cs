using MyRule.CommandPattern;
using MyRule.UI;
using UnityEngine;


namespace MyRule
{
    public class SpaceStationInputHandler : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private HoloTable holoTable;
        [SerializeField] private PlanetManager planetManager;

        private void OnEnable()
        {
            inputReader.SwitchActionMap(ActionMap.SpaceStation);

            inputReader.spaceStationActions.onMove += OnMove;
            inputReader.spaceStationActions.onInteract += OnInteract;
            inputReader.spaceStationActions.onEsc += OnEsc;
            inputReader.spaceStationActions.onActve += OnActive;
            inputReader.spaceStationActions.onTab += OpenTabView;
        }

        private void OnDisable()
        {
            inputReader.spaceStationActions.onMove -= OnMove;
            inputReader.spaceStationActions.onInteract -= OnInteract;
            inputReader.spaceStationActions.onEsc -= OnEsc;
            inputReader.spaceStationActions.onActve -= OnActive;
            inputReader.spaceStationActions.onTab -= OpenTabView;
        }

        private void Start()
        {

        }

        private void OnMove(Vector2 movement)
        {
            EventBus<ScifiMouseMoveEvent>.Raise(new ScifiMouseMoveEvent(movement));
        }

        private void OnInteract()
        {
            if (holoTable == null || planetManager == null) return;

            if (!holoTable.HasActive)
            {
                ICommand command = new HoloTableInteractCommand(holoTable);
                CommandInvoker.ExecuteCommand(command);
            }
            else
            {
                ICommand command = new PlanetCommand(planetManager);
                CommandInvoker.ExecuteCommand(command);
            }

            if (PortalManager.Instance.CanInteract)
            {
                EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Portal));
            }
        }

        private void OnEsc()
        {
            CommandInvoker.UndoCommand();
        }

        private void OnActive()
        {
            EventBus<ScifitableActiveEvent>.Raise(new ScifitableActiveEvent());
        }

        private void OpenTabView()
        {
            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.TabView));
        }
    }
}