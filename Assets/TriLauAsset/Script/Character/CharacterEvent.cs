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

    public struct CharacterUpdatedEvent : IEvent
    {
        public readonly CharacterData character;

        public CharacterUpdatedEvent(CharacterData characterData)
        {
            this.character = characterData;
        }
    }
}