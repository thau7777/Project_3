using MyRule;
using UnityEngine;

namespace MyRule
{
    public class DataPersistence : MonoBehaviour
    {
        [SerializeField] private DataSO dataSO;

        private EventBinding<TBVictoryEvent> victoryEventBinding;

        private void OnEnable()
        {
            victoryEventBinding = new EventBinding<TBVictoryEvent>(OnTBVictoryEvent);
            EventBus<TBVictoryEvent>.Register(victoryEventBinding);
        }

        private void OnDisable()
        {
            EventBus<TBVictoryEvent>.Deregister(victoryEventBinding);
        }

        private void OnTBVictoryEvent(TBVictoryEvent evt)
        {
            dataSO.matchResults.Add(evt.isVictory);
        }
    }
}