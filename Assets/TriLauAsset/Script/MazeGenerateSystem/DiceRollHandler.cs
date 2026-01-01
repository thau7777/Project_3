using UnityEngine;

namespace MyRule
{
    public class DiceRollHandler : MonoBehaviour
    {
        private EventBinding<DiceRollEvent> _diceRollEventBinding;

        private void OnEnable()
        {
            _diceRollEventBinding = new EventBinding<DiceRollEvent>(OnDiceRollEvent);
            EventBus<DiceRollEvent>.Register(_diceRollEventBinding);
        }

        private void OnDisable()
        {
            EventBus<DiceRollEvent>.Deregister(_diceRollEventBinding);
        }

        private void OnDiceRollEvent(DiceRollEvent evt)
        {
            int[] faces = { 1, 2, 3, 4, 5, 6 };
            int[] weights = { 10, 15, 20, 25, 20, 10 };

            int totalWeight = 0;
            foreach (int w in weights)
                totalWeight += w;

            int random = Random.Range(0, totalWeight);
            int current = 0;

            for (int i = 0; i < faces.Length; i++)
            {
                current += weights[i];
                if (random < current)
                {
                    EventBus<DiceValueEvent>.Raise(new DiceValueEvent(faces[i]));
                    return;
                }
            }

            EventBus<DiceValueEvent>.Raise(new DiceValueEvent(1));
        }
    }
}