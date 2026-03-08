using UnityEngine;

namespace MyRule
{
    public class RuneManger : PersistentSingleton<RuneManger>
    {
        [SerializeField] private RuneSO runeSO;
        
        private EventBinding<ReceiveRuneEvent> receiveRuneEventBinding;

        private void OnEnable()
        {
            receiveRuneEventBinding = new EventBinding<ReceiveRuneEvent>(OnReceiveRune);
            EventBus<ReceiveRuneEvent>.Register(receiveRuneEventBinding);
        }

        private void OnDisable()
        {
            EventBus<ReceiveRuneEvent>.Deregister(receiveRuneEventBinding);
        }

        private void OnReceiveRune(ReceiveRuneEvent evt)
        {
            runeSO.runeAmount += evt.runeAmount;

            EventBus<SendUIRuneEvent>.Raise(new SendUIRuneEvent(runeSO.runeAmount));
        }

        public int GetRuneAmount() => runeSO.runeAmount;

        public void SetStartRune(int amount)
        {
            runeSO.runeAmount = amount;
        }
    }
}