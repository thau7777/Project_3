using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTB : MonoBehaviour
{
    public InputReader inputReader;

    private void Start()
    {
        inputReader.SwitchActionMap(ActionMap.PlayerTurnBased);

        inputReader.playerTurnBasedActions.OnTestEvent += HandleTestEvent;
    }

    private void HandleTestEvent()
    {
        EventBus<TBVictoryEvent>.Raise(new TBVictoryEvent(true));
        SceneManager.LoadScene("BoardScene");
    }
}
