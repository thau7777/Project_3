using MyRule.Event;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.EventSystems;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;

namespace MyRule.UI
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField] private Volume dialogueVolume;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.2f;

        [Header("Dialogue")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private float typingSpeed = 0.04f;
        [SerializeField] private GameObject continueIcon;

        [Header("Speaker")]
        [SerializeField] private TextMeshProUGUI nameText;

        [Header("Choices")]
        [SerializeField] private DialougeChoiceButtonView[] choiceButtons;

        private EventBinding<DialougeStartedEvent> dialougeStartedEventBinding;
        private EventBinding<DialogueFinishedEvent> dialougeFinishedEventBinding;
        private EventBinding<DisplayDialogueEvent> displayDialogueEventBinding;
        private EventBinding<UpdateSpeakerNameEvent> updateSpeakerNameEventBinding;

        private CancellationTokenSource cts;
        private CancellationTokenSource typingCts;

        private bool isTyping;
        private string currentLine;

        private void OnEnable()
        {
            dialougeStartedEventBinding = new EventBinding<DialougeStartedEvent>(DialogueStarted);
            EventBus<DialougeStartedEvent>.Register(dialougeStartedEventBinding);

            dialougeFinishedEventBinding = new EventBinding<DialogueFinishedEvent>(DialogueFinished);
            EventBus<DialogueFinishedEvent>.Register(dialougeFinishedEventBinding);

            displayDialogueEventBinding = new EventBinding<DisplayDialogueEvent>(DisplayDialogue);
            EventBus<DisplayDialogueEvent>.Register(displayDialogueEventBinding);

            updateSpeakerNameEventBinding = new EventBinding<UpdateSpeakerNameEvent>(UpdateSpeakerName);
            EventBus<UpdateSpeakerNameEvent>.Register(updateSpeakerNameEventBinding);
        }

        private void OnDisable()
        {
            EventBus<DialougeStartedEvent>.Deregister(dialougeStartedEventBinding);
            EventBus<DialogueFinishedEvent>.Deregister(dialougeFinishedEventBinding);
            EventBus<DisplayDialogueEvent>.Deregister(displayDialogueEventBinding);
            EventBus<UpdateSpeakerNameEvent>.Deregister(updateSpeakerNameEventBinding);
        }

        private void Start()
        {
            cts = new();
        }

        private void Show()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                setter: value => dialogueVolume.weight = value,
                from: dialogueVolume.weight,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();
        }

        private void Hide()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                setter: value => dialogueVolume.weight = value,
                from: dialogueVolume.weight,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();
        }

        private void ResetPanel()
        {
            dialogueText.text = "";
            dialogueText.maxVisibleCharacters = 0;
        }

        private void DialogueStarted()
        {
            Show();
            Debug.Log("StartDialogue");
        }

        private void DialogueFinished()
        {
            typingCts?.Cancel();
            Hide();
            ResetPanel();
        }

        private void DisplayDialogue(DisplayDialogueEvent evt)
        {
            Debug.Log("DisplayDialogue");

            typingCts?.Cancel();

            currentLine = evt.dialogueLine;

            foreach (DialougeChoiceButtonView choiceButton in choiceButtons)
            {
                choiceButton.gameObject.SetActive(false);
            }

            TypeDialogue(evt).Forget();
        }

        private async UniTask TypeDialogue(DisplayDialogueEvent evt)
        {
            typingCts = new CancellationTokenSource();
            isTyping = true;

            continueIcon.SetActive(false);
            DialogueManager.Instance.CanContinueDialogue = false;

            dialogueText.text = evt.dialogueLine;
            dialogueText.maxVisibleCharacters = 0;

            dialogueText.ForceMeshUpdate();
            int totalCharacters = dialogueText.textInfo.characterCount;

            for (int i = 0; i <= totalCharacters; i++)
            {
                dialogueText.maxVisibleCharacters = i;

                await UniTask.Delay(
                    (int)(typingSpeed * 1000),
                    cancellationToken: typingCts.Token
                );
            }

            isTyping = false;

            ShowChoices(evt);

            continueIcon.SetActive(true);

            DialogueManager.Instance.CanContinueDialogue = true;
        }

        private void ShowChoices(DisplayDialogueEvent evt)
        {
            for (int i = 0; i < evt.dialogueChoices.Count; i++)
            {
                Choice dialogueChoice = evt.dialogueChoices[i];
                DialougeChoiceButtonView choiceButton = choiceButtons[i];

                choiceButton.gameObject.SetActive(true);
                choiceButton.SetText(dialogueChoice.text);
                choiceButton.SetIndex(i);

                if (i == 0)
                {
                    EventSystem.current.SetSelectedGameObject(choiceButton.gameObject);
                }
            }
        }

        public void SkipTyping()
        {
            if (!isTyping) return;

            typingCts?.Cancel();

            dialogueText.maxVisibleCharacters = int.MaxValue;

            isTyping = false;
            continueIcon.SetActive(true);
        }

        private void UpdateSpeakerName(UpdateSpeakerNameEvent evt)
        {
            Debug.Log(evt.name);
            nameText.text = evt.name;
        }
    }
}