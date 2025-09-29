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
            default:
                _inputReader.SwitchActionMap(ActionMap.UI);
                break;
        }
    }
}
