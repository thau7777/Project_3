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
}