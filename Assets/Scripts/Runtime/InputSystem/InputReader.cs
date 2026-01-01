using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum ActionMap
{
    PlayerTopDown,
    PlayerTurnBased,
    PlayerFPS,
    PlayerTowerDefense,
    UI,
    PopUpGame,
    SpaceStation,
    DiceRoll,
}

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject
{
    private InputActions input;
    public PlayerTopDownActions playerTopDownActions;
    public PlayerTurnBasedActions playerTurnBasedActions;
    public PlayerFPSActions playerFPSActions;
    public PlayerTowerDefenseActions playerTowerDefenseActions;
    public UIActions uiActions;
    public PopUpGame popUpGame;
    public SpaceStationActions spaceStationActions;
    public DiceRollActions diceRollActions;

    private void OnEnable()
    {
        InitializeActions();
    }

    private void OnDisable()
    {
        DisableActions();
    }

    private void InitializeActions()
    {
        playerTopDownActions = new PlayerTopDownActions();
        playerTurnBasedActions = new PlayerTurnBasedActions();
        playerFPSActions = new PlayerFPSActions();
        playerTowerDefenseActions = new PlayerTowerDefenseActions();
        uiActions = new UIActions();
        popUpGame = new PopUpGame();
        diceRollActions = new DiceRollActions();
        spaceStationActions = new SpaceStationActions();
        if (input == null)
        {
            input = new InputActions();
            input.PlayerTopDown.SetCallbacks(playerTopDownActions);
            input.PlayerTurnBased.SetCallbacks(playerTurnBasedActions);
            input.PlayerFPS.SetCallbacks(playerFPSActions);
            input.PlayerTowerDefense.SetCallbacks(playerTowerDefenseActions);
            input.UI.SetCallbacks(uiActions);
            input.PopUpGame.SetCallbacks(popUpGame);
            input.SpaceStation.SetCallbacks(spaceStationActions);
            input.DiceRoll.SetCallbacks(diceRollActions);
        }
        //input.UI.Enable();
    }
    private void DisableActions()
    {
        if(input == null) return;
        input.PlayerTopDown.Disable();
        input.PlayerTurnBased.Disable();
        input.PlayerFPS.Disable();
        input.PlayerTowerDefense.Disable();
        input.UI.Disable();
        input.PopUpGame.Disable();
        input.SpaceStation.Disable();
        input.DiceRoll.Disable();
    }
    public void SwitchActionMap(ActionMap map)
    {
        DisableActions();
        switch (map)
        {
            case ActionMap.PlayerTopDown:
                input.PlayerTopDown.Enable();
                break;
            case ActionMap.PlayerTurnBased:
                input.PlayerTurnBased.Enable();
                break;
            case ActionMap.PlayerFPS:
                input.PlayerFPS.Enable();
                break;
            case ActionMap.PlayerTowerDefense:
                input.PlayerTowerDefense.Enable();
                break;
            case ActionMap.UI:
                input.UI.Enable();
                break;
            case ActionMap.PopUpGame:
                input.PopUpGame.Enable();
                break;
            case ActionMap.SpaceStation:
                input.SpaceStation.Enable();
                break;
            case ActionMap.DiceRoll:
                input.DiceRoll.Enable();
                break;
        }
        Debug.Log("switched actionButton map to: " + map.ToString());
    }
}
