using UnityEngine;

namespace MyRule
{
    public class WaveManager : PersistentSingleton<WaveManager>
    {
        [SerializeField] private GroupWave groupWave;

        private EventBinding<WaveEvent> eventBinding;

        private void OnEnable()
        {
            eventBinding = new EventBinding<WaveEvent>(OnWaveEvent);
            EventBus<WaveEvent>.Register(eventBinding);
        }

        private void OnDisable()
        {
            EventBus<WaveEvent>.Deregister(eventBinding);
        }

        private void OnWaveEvent(WaveEvent waveEvent)
        {
            groupWave = waveEvent.GroupWave;
        }

        public GroupWave GetCurrentWave()
        {
            return groupWave;
        }
    }
}