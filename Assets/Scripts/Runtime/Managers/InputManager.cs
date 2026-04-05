using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : PersistentSingleton<InputManager>
{
    [SerializeField, Required]
    private InputReader _inputReader;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive) return;

        switch (scene.name)
        {
            case "TopDown": 
                _inputReader.SwitchActionMap(ActionMap.PlayerTopDown);
                break;
            case "TurnBased":
                _inputReader.SwitchActionMap(ActionMap.PlayerTurnBased);
                break;
            case "FPS":
                _inputReader.SwitchActionMap(ActionMap.PlayerFPS);
                break;
            case "TowerDefense":
                _inputReader.SwitchActionMap(ActionMap.PlayerTowerDefense);
                break;
            case "PopupGame":
                _inputReader.SwitchActionMap(ActionMap.PopUpGame);
                break;
            case "MainMenuScene":
                _inputReader.SwitchActionMap(ActionMap.UI);
                break;
            case "SpaceStationScene":
                Debug.Log("spacestation");
                _inputReader.SwitchActionMap(ActionMap.SpaceStation);
                break;
            case "MazeScene":
                _inputReader.SwitchActionMap(ActionMap.DiceRoll);
                break;
            case "GreenlandScene":
                _inputReader.SwitchActionMap(ActionMap.DiceRoll);
                break;
            case "DesertScene":
                _inputReader.SwitchActionMap(ActionMap.DiceRoll);
                break;
            case "IcelandScene":
                _inputReader.SwitchActionMap(ActionMap.DiceRoll);
                break;
            default:
                _inputReader.SwitchActionMap(ActionMap.UI);
                break;
        }
    }

    public void DisableAllAction()
    {
        _inputReader.DisableActions();
    }
    public void EnableActionMap(ActionMap map)
    {
        _inputReader.SwitchActionMap(map);
    }
}
