using MyRule.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class MiniGameManager : Singleton<MiniGameManager>
    {
        private const string miniGameNameScene = "UI GAME";

        private EventBinding<TriggerMiniGameEvent> triggerMiniGameEvent;
        private EventBinding<MiniGameResultEvent> resultEvent;

        private void OnEnable()
        {
            triggerMiniGameEvent = new EventBinding<TriggerMiniGameEvent>(TriggerMiniGame);
            EventBus<TriggerMiniGameEvent>.Register(triggerMiniGameEvent);

            resultEvent = new EventBinding<MiniGameResultEvent>(ExitMiniGame);
            EventBus<MiniGameResultEvent>.Register(resultEvent);
        }

        private void OnDisable()
        {
            EventBus<TriggerMiniGameEvent>.Deregister(triggerMiniGameEvent);
            EventBus<MiniGameResultEvent>.Deregister(resultEvent);
        }

        private void TriggerMiniGame()
        {
            Debug.Log("TriggerMiniGame");

            if (!SceneManager.GetSceneByName(miniGameNameScene).isLoaded)
            {
                SceneManager.LoadScene(miniGameNameScene, LoadSceneMode.Additive);
                DialogueManager.Instance.CanContinueDialogue = false;
            }
            else
            {
                Debug.LogWarning($"[MiniGameManager] Scene '{miniGameNameScene}' has been loaded.");
            }
        }

        private void ExitMiniGame()
        {
            if (SceneManager.GetSceneByName(miniGameNameScene).isLoaded)
            {
                SceneManager.UnloadSceneAsync(miniGameNameScene);
                DialogueManager.Instance.CanContinueDialogue = true;
            }
            else
            {
                Debug.LogWarning($"[MiniGameManager] Scene '{miniGameNameScene}' hasnt been loaded.");
            }
        }
    }
}