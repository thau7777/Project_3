using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule.UI
{
    public class UIDiceController : MonoBehaviour
    {
        public GameObject dice;
        public Animator diceAnimator;

        private EventBinding<DiceValueEvent> _diceRollEventBinding;

        private void OnEnable()
        {
            _diceRollEventBinding = new EventBinding<DiceValueEvent>(OnDiceValueEvent);
            EventBus<DiceValueEvent>.Register(_diceRollEventBinding);
        }

        private void OnDisable()
        {
            EventBus<DiceValueEvent>.Deregister(_diceRollEventBinding);
        }

        private void Start()
        {
            HideDice();
        }

        private async void OnDiceValueEvent(DiceValueEvent evt)
        {
            ShowDice();
            diceAnimator.SetInteger("DiceValue", evt.DiceValue);
            diceAnimator.SetTrigger("Roll");
            
            await UniTask.Delay(2000);

            await WaitForDiceAnimationEnd(evt.DiceValue);

        }

        private UniTask WaitForDiceAnimationEnd(int diceValue)
        {
            HideDice();

            EventBus<MazeMoveEvent>.Raise(new MazeMoveEvent(diceValue));

            return UniTask.CompletedTask;
        }

        private void ShowDice()
        {
            dice.gameObject.SetActive(true);
        }

        private void HideDice()
        {
            dice.gameObject.SetActive(false);
        }
    }
}