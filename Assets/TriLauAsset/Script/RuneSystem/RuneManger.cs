using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class RuneManger : PersistentSingleton<RuneManger>, IGameData
    {
        private int runeAmount = 100;

        public int RuneAmount => runeAmount;
        
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
            runeAmount += evt.runeAmount;

            EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(runeAmount));
        }

        public void SetStartRune(int amount)
        {
            runeAmount = amount;
        }

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                runeAmount = data.MatchData.RuneInMatch;
                EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(runeAmount));
            }
            
            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                data.MatchData.SetRuneInMatch(runeAmount);
            }
        }
    }
}