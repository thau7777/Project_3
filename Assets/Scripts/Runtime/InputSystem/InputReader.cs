using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum ActionMap
{
    PlayerTopDown,
    PlayerTurnBased,
    PlayerFPS,
    PlayerTowerDefense,
    UI
}

[CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/InputReader")]
public class InputReader : ScriptableObject
{
    private InputActions input;
    public PlayerTopDownActions playerTopDownActions;
    public PlayerTurnBasedActions playerTurnBasedActions;
    public PlayerFPSActions playerFPSActions;
    public PlayerTowerDefenseActions playerTowerDefenseActions;
    public UIActions uiActions;

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
        if (input == null)
        {
            input = new InputActions();
            input.PlayerTopDown.SetCallbacks(playerTopDownActions);
            input.PlayerTurnBased.SetCallbacks(playerTurnBasedActions);
            input.PlayerFPS.SetCallbacks(playerFPSActions);
            input.PlayerTowerDefense.SetCallbacks(playerTowerDefenseActions);
            input.UI.SetCallbacks(uiActions);
        }
        input.UI.Enable();
    }
    private void DisableActions()
    {
        if(input == null) return;
        input.PlayerTopDown.Disable();
        input.PlayerTurnBased.Disable();
        input.PlayerFPS.Disable();
        input.PlayerTowerDefense.Disable();
        input.UI.Disable();
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
        }
        Debug.Log("switched action map to: " + map.ToString());
    }
}
