using UnityEngine;


namespace MyRule
{
    public struct CharacterModelEvent : IEvent
    {
        public EClass characterClass;

        public CharacterModelEvent(EClass characterClass)
        {
            this.characterClass = characterClass;
        }
    }

    public struct CharacterStatsUpdatedEvent : IEvent
    {
        public readonly CharacterStatsData characterStats;

        public CharacterStatsUpdatedEvent(CharacterStatsData characterStatsData)
        {
            this.characterStats = characterStatsData;
        }
    }
}