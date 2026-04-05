using Cysharp.Threading.Tasks;
using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class TAEController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private TutorialTrigger tutorialTrigger;

        private EventBinding<DialogueFinishedEvent> dialogueFinishEvt;

        private void OnEnable()
        {
            dialogueFinishEvt = new EventBinding<DialogueFinishedEvent>(FinishDialogue);
            EventBus<DialogueFinishedEvent>.Register(dialogueFinishEvt);
        }

        private void OnDisable()
        {
            EventBus<DialogueFinishedEvent>.Deregister(dialogueFinishEvt);
        }

        private void Start()
        {
            TriggerTAE();
        }

        public async void TriggerTAE()
        {
            if (GameSystemManager.Instance != null)
            {
                var data = GameSystemManager.Instance?.GameData?.DialougeData;
                if (data == null) return;

                if (data.KeyValuePairs.TryGetValue("hasMeetTAE", out var value)
                    && value is bool hasMeetTAE && hasMeetTAE)
                {
                    Destroy(this.gameObject);
                    await UniTask.Delay(7000);
                    CinematicBorder.Instance.HideBorder(0.4f).Forget();
                    return;
                }
            }

            await UniTask.Delay(7000);

            inputReader.SwitchActionMap(ActionMap.DiceRoll);

            NPCManager.Instance.TriggerNPC();
        }

        private async void FinishDialogue()
        {
            inputReader.SwitchActionMap(ActionMap.SpaceStation);
            BlackFade.Instance.FadeThisFrame(0.2f);
            await UniTask.Delay(200);
            Destroy(this.gameObject);
            await CinematicBorder.Instance.HideBorder(0f);
            tutorialTrigger.Trigger();
        }
    }
}
