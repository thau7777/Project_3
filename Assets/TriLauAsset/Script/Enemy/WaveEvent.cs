using UnityEngine;

namespace MyRule
{
    public struct WaveEvent : IEvent
    {
        public GroupWave GroupWave { get; }

        public WaveEvent(GroupWave groupWave)
        {
            GroupWave = groupWave;
        }
    }
}