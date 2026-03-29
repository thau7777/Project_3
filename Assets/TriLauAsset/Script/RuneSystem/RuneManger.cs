using Cysharp.Threading.Tasks;
using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class RuneManger : PersistentSingleton<RuneManger>, IGameData
    {
        private int currentRuneAmount = 100;

        public int CurrentRuneAmount => currentRuneAmount;

        private int lockReceiveTurn = 0;

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
            if (lockReceiveTurn <= 0)
            {
                currentRuneAmount += evt.runeAmount;

                EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount));
            }
            else
            {
                lockReceiveTurn -= 1;
            }
        }

        public void SetStartRune(int amount)
        {
            currentRuneAmount = amount;
        }

        public void SetLockReceiveTurn(int turn) => lockReceiveTurn = turn;

        public async UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                currentRuneAmount = data.MatchData.RuneInMatch;
                lockReceiveTurn = data.MatchData.LockReceiveRuneTurn;
                EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount));

                await UniTask.WaitUntil(() => DialogueManager.Instance);

                EventBus<UpdateInkDialogueVariableEvent>.Raise(new UpdateInkDialogueVariableEvent("currentRune", currentRuneAmount));
            }
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                data.MatchData.SetRuneInMatch(currentRuneAmount);
                data.MatchData.SetLockReceiveRuneTurn(lockReceiveTurn);
            }
        }
    }
}