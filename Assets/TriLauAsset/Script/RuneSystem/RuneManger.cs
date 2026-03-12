using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class RuneManger : PersistentSingleton<RuneManger>, IGameData
    {
        private int currentRuneAmount = 100;

        public int CurrentRuneAmount => currentRuneAmount;
        
        private EventBinding<ReceiveRuneEvent> receiveRuneEventBinding;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);

            receiveRuneEventBinding = new EventBinding<ReceiveRuneEvent>(OnReceiveRune);
            EventBus<ReceiveRuneEvent>.Register(receiveRuneEventBinding);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);

            EventBus<ReceiveRuneEvent>.Deregister(receiveRuneEventBinding);
        }

        private void OnReceiveRune(ReceiveRuneEvent evt)
        {
            currentRuneAmount += evt.runeAmount;

            EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount));
        }

        public void SetStartRune(int amount)
        {
            currentRuneAmount = amount;
        }

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                currentRuneAmount = data.MatchData.RuneInMatch;
                EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount));
            }
            
            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                data.MatchData.SetRuneInMatch(currentRuneAmount);
            }
        }
    }
}