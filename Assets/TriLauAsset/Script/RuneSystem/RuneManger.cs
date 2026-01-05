using UnityEngine;

namespace MyRule
{
    public class RuneManger : MonoBehaviour
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
            runeSO.runeCount += evt.runeCount;
        }
    }
}