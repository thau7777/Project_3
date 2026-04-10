using Cysharp.Threading.Tasks;
using MyRule.Audio;
using MyRule.Event;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyRule
{
    public class EndingController : MonoBehaviour
    {
        [SerializeField] private DialougeTrigger dialougeTrigger;
        [SerializeField] private GameObject cam1;
        [SerializeField] private GameObject cam2;
        [SerializeField] private GameObject cam3;
        [SerializeField] private InputReader inputReader;

        [SerializeField] private GameObject char1;
        [SerializeField] private GameObject char2;
        [SerializeField] private GameObject char3;

        [SerializeField] private DialougeTrigger dialougeTrigger2;

        [Header("Char4 Control")]
        [SerializeField] private Transform char4;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform target;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float accel = 2f;

        [SerializeField] private EndingCredit[] endingCredits;
        [SerializeField] private float DelayShowText = 3f;
        [SerializeField] private SmoothLookAtCredit smoothLookAtCredit;
        [SerializeField] private Transform gate;
        [SerializeField] private Volume whiteVol;
        [SerializeField] private float delayGMRPower = 10f;
        [SerializeField] private float powerDuration = 1.0f;

        private float currentSpeed = 0f;
        private bool canMove = false;
        private bool finishFirstDialogue;

        private CancellationTokenSource cts;

        private EventBinding<DialogueCamEvent> dialogueCamEvent;
        private EventBinding<DialogueFinishedEvent> dialogueFinishedEvent;

        private void OnEnable()
        {
            dialogueCamEvent = new EventBinding<DialogueCamEvent>(SwitchEndingCam);
            EventBus<DialogueCamEvent>.Register(dialogueCamEvent);

            dialogueFinishedEvent = new EventBinding<DialogueFinishedEvent>(OnDialogueFinish);
            EventBus<DialogueFinishedEvent>.Register(dialogueFinishedEvent);
        }

        private void OnDisable()
        {
            EventBus<DialogueCamEvent>.Deregister(dialogueCamEvent);
            EventBus<DialogueFinishedEvent>.Deregister(dialogueFinishedEvent);
        }

        private void Start()
        {
            cam1.SetActive(false);
            cam2.SetActive(false);

            EnterEnding().Forget();

            inputReader.SwitchActionMap(ActionMap.DiceRoll);

            FinishMatch();
        }

        private void Update()
        {
            if (target == null || char4 == null) return;
            if (!canMove) return;

            Vector3 direction = target.position - char4.position;
            direction.y = 0;

            float distance = direction.magnitude;

            if (distance > 0.1f)
            {
                direction.Normalize();

                Quaternion lookRotation = Quaternion.LookRotation(direction);
                char4.rotation = Quaternion.Slerp(char4.rotation, lookRotation, rotateSpeed * Time.deltaTime);

                currentSpeed = Mathf.MoveTowards(currentSpeed, 1f, accel * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, accel * Time.deltaTime);
            }

            animator.SetFloat("Y", currentSpeed);
        }

        private void OnAnimatorMove()
        {
            if (!canMove || char4 == null) return;

            char4.position += animator.deltaPosition;
            char4.rotation *= animator.deltaRotation;
        }

        private async UniTask EnterEnding()
        {
            BlackFade.Instance.FadeOut(1f).Forget();

            await UniTask.Delay(1000);

            cam1.SetActive(true);

            await UniTask.Delay(300);

            dialougeTrigger.Trigger();
        }

        private async void OnDialogueFinish()
        {
            BlackFade.Instance.FadeThisFrame(0.5f);

            if (!finishFirstDialogue)
            {
                char1.SetActive(false);
                char2.SetActive(false);

                char3.SetActive(true);

                await UniTask.Delay(2000);

                cam3.SetActive(true);

                dialougeTrigger2.Trigger();

                finishFirstDialogue = true;
            }
            else
            {
                char3.SetActive(false);

                AudioManager.Instance.PlaySound("EndingBGMusic");

                await UniTask.Delay(2000);

                canMove = true;

                ShowCredit();
            }
        }

        private async void ShowCredit()
        {
            await UniTask.Delay(8000);

            for (int i = 0; i < endingCredits.Length; i++)
            {
                endingCredits[i].ShowText();
                smoothLookAtCredit.SetTarget(endingCredits[i].transform);
                await UniTask.Delay((int)(DelayShowText * 1000));
            }

            smoothLookAtCredit.SetTarget(gate);

            await UniTask.Delay((int)(delayGMRPower * 1000));

            SetPower();

            await UniTask.Delay(12000);
            await Loader.LoadSceneDirect(Loader.EScene.MainMenuScene);
        }

        private void SetPower()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            Transition.TransitionValue(
                setter: value => whiteVol.weight = value,
                from: whiteVol.weight,
                to: 1f,
                duration: powerDuration,
                token: cts.Token).Forget();
        }

        private void SwitchEndingCam(DialogueCamEvent evt)
        {
            switch (evt.camName)
            {
                case "cam1":
                    cam1.SetActive(true);
                    cam2.SetActive(false);
                    break;
                case "cam2":
                    cam1.SetActive(false);
                    cam2.SetActive(true);
                    break;
            }
        }

        private void FinishMatch()
        {
            MatchManager.Instance.FinishMatch();
        }    
    }
}