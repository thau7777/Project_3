using Cysharp.Threading.Tasks;
using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class RuneManger : PersistentSingleton<RuneManger>, IGameData
    {
        private int currentRuneAmount = 0;

        public int CurrentRuneAmount => currentRuneAmount;

        private int lockReceiveTurn = 0;

        private EventBinding<ReceiveRuneEvent> receiveRuneEventBinding;
        private EventBinding<SpendRuneEvent> spendRuneEventBinding;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);

            receiveRuneEventBinding = new EventBinding<ReceiveRuneEvent>(OnReceiveRune);
            EventBus<ReceiveRuneEvent>.Register(receiveRuneEventBinding);

            spendRuneEventBinding = new EventBinding<SpendRuneEvent>(OnSpendRune);
            EventBus<SpendRuneEvent>.Register(spendRuneEventBinding);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);

            EventBus<ReceiveRuneEvent>.Deregister(receiveRuneEventBinding);
            EventBus<SpendRuneEvent>.Deregister(spendRuneEventBinding);
        }

        private void OnReceiveRune(ReceiveRuneEvent evt)
        {
            if (lockReceiveTurn <= 0)
            {
                currentRuneAmount += evt.runeAmount;

                EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount, lockReceiveTurn));
                EventBus<UpdateInkDialogueVariableEvent>.Raise(new UpdateInkDialogueVariableEvent("currentRune", currentRuneAmount));
            }
            else
            {
                lockReceiveTurn -= 1;
            }
        }

        private void OnSpendRune(SpendRuneEvent evt)
        {
            currentRuneAmount -= evt.runeAmount;
            EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount, lockReceiveTurn));
        }

        public void SetStartRune(int amount)
        {
            currentRuneAmount = amount;
        }

        public void SetLockReceiveTurn(int turn)
        {
            lockReceiveTurn = turn;
            EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount, lockReceiveTurn));
        }

        public async UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                currentRuneAmount = data.MatchData.RuneInMatch;
                lockReceiveTurn = data.MatchData.LockReceiveRuneTurn;
                EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(currentRuneAmount, lockReceiveTurn));

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

        public UniTask NewGame()
        {
            currentRuneAmount = 0;
            lockReceiveTurn = 0;
            return UniTask.CompletedTask;
        }
    }
}