using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class UIDiceController : MonoBehaviour
    {
        public GameObject dice;
        public Animator diceAnimator;
        public TextMeshProUGUI diceValueText;

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

            diceValueText.gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }

        private async void OnDiceValueEvent(DiceValueEvent evt)
        {
            ShowDice();

            diceAnimator.SetTrigger("Roll");
            
            await UniTask.Delay(1000);

            diceAnimator.SetInteger("DiceValue", evt.DiceValue);
            diceValueText.text = evt.DiceValue.ToString();
            
            await UniTask.Delay(500);
            EventBus<MazeJumpEvent>.Raise(new MazeJumpEvent());
            await UniTask.Delay(1000);
            diceValueText.gameObject.SetActive(true);
            
            await UniTask.Delay(2000);

            await WaitForDiceAnimationEnd(evt.DiceValue);

        }

        private UniTask WaitForDiceAnimationEnd(int diceValue)
        {
            HideDice();

            diceValueText.gameObject.SetActive(false);

            EventBus<MazeStepEvent>.Raise(new MazeStepEvent(diceValue));

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