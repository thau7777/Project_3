using MyRule.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyRule
{
    public class MiniGameManager : Singleton<MiniGameManager>
    {

        private EventBinding<TriggerMiniGameEvent> triggerMiniGameEvent;
        private EventBinding<MiniGameResultEvent> resultEvent;

        private string mGName;

        public string MGName => mGName;

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

        private async void TriggerMiniGame(TriggerMiniGameEvent evt)
        {
            Debug.Log("TriggerMiniGame");

            mGName = evt.name;
            await Loader.LoadSceneAdditive(Loader.EScene.UIGAME);
            DialogueManager.Instance.CanContinueDialouge(false);

        }

        private async void ExitMiniGame()
        {
            await Loader.UnloadSceneAdditive(Loader.EScene.UIGAME);

            DialogueManager.Instance.CanContinueDialouge(true);

        }
    }
}