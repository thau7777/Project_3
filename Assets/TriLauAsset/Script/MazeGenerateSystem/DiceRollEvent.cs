using UnityEngine;

namespace MyRule
{
    public struct DiceRollEvent : IEvent
    {
        
    }

    public struct DiceValueEvent : IEvent
    {
        public readonly int DiceValue;

        public DiceValueEvent(int diceValue)
        {
            DiceValue = diceValue;
        }
    }
}