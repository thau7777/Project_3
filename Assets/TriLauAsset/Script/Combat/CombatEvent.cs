using UnityEngine;

namespace MyRule.Event
{
    public struct UpdateTDCombatWavesEvent : IEvent
    {
        public readonly GroupWave groupWave;

        public UpdateTDCombatWavesEvent(GroupWave groupWave)
        {
            this.groupWave = groupWave;
        }
    }

    public struct UpdateTBCombatWavesEvent : IEvent
    {
        public readonly GroupWave groupWave;

        public UpdateTBCombatWavesEvent(GroupWave groupWave)
        {
            this.groupWave = groupWave;
        }
    }

    public struct ShowCombatChoiceEvent : IEvent
    {
        public readonly bool showCombatChoice;

        public ShowCombatChoiceEvent(bool show)
        {
            showCombatChoice = show;
        }
    }
}