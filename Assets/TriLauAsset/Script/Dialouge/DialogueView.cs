using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ink.Runtime;
using MyRule.Event;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private InputReader inputReader;

        private EventBinding<DialougeStartedEvent> dialougeStartedEventBinding;
        private EventBinding<DialogueFinishedEvent> dialougeFinishedEventBinding;
        private EventBinding<DisplayDialogueEvent> displayDialogueEventBinding;
        private EventBinding<UpdateSpeakerNameEvent> updateSpeakerNameEventBinding;

        private CancellationTokenSource cts;
        private CancellationTokenSource typingCts;

        private DisplayDialogueEvent currentEvent;

        private bool isTyping;

        public bool IsTyping => isTyping;

        private int currentChoiceIndex = 0;
        private int currentChoiceCount = 0;
        private bool isShowingChoices = false;
        public bool IsShowingChoices => isShowingChoices;

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

            inputReader.diceRollActions.onMove += NavigateChoiceView;
        }

        private void OnDisable()
        {
            EventBus<DialougeStartedEvent>.Deregister(dialougeStartedEventBinding);
            EventBus<DialogueFinishedEvent>.Deregister(dialougeFinishedEventBinding);
            EventBus<DisplayDialogueEvent>.Deregister(displayDialogueEventBinding);
            EventBus<UpdateSpeakerNameEvent>.Deregister(updateSpeakerNameEventBinding);

            inputReader.diceRollActions.onMove -= NavigateChoiceView;
        }

        private void Start()
        {
            cts = new();
        }

        private void Show()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            canvasGroup.DOFade(1f, fadeDuration);

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

            canvasGroup.DOFade(0, fadeDuration);

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

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                choiceButtons[i].SetCanSubmit(false);
            }

            TypeDialogue(evt).Forget();
        }

        private async UniTask TypeDialogue(DisplayDialogueEvent evt)
        {
            typingCts?.Cancel();
            typingCts?.Dispose();
            typingCts = new CancellationTokenSource();

            isTyping = true;

            isShowingChoices = false;

            currentEvent = evt;

            continueIcon.SetActive(false);

            dialogueText.text = evt.dialogueLine;
            dialogueText.maxVisibleCharacters = 0;

            dialogueText.ForceMeshUpdate();
            int totalCharacters = dialogueText.textInfo.characterCount;

            try
            {
                for (int i = 0; i <= totalCharacters; i++)
                {
                    dialogueText.maxVisibleCharacters = i;

                    await UniTask.Delay(
                        (int)(typingSpeed * 1000),
                        cancellationToken: typingCts.Token
                    );
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

            isTyping = false;

            ShowChoices(evt).Forget();
            continueIcon.SetActive(true);
        }

        public void SkipTyping()
        {
            if (!isTyping) return;

            typingCts?.Cancel();

            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

            isTyping = false;

            ShowChoices(currentEvent).Forget();
            continueIcon.SetActive(true);
        }

        private async UniTask ShowChoices(DisplayDialogueEvent evt)
        {
            if (isShowingChoices) return;

            isShowingChoices = true;

            currentChoiceIndex = 0;
            currentChoiceCount = Mathf.Min(evt.dialogueChoices.Count, choiceButtons.Length);

            for (int i = 0; i < currentChoiceCount; i++)
            {
                Choice dialogueChoice = evt.dialogueChoices[i];
                DialougeChoiceButtonView choiceButton = choiceButtons[i];

                choiceButton.SetText(dialogueChoice.text);
                choiceButton.SetIndex(i);
                choiceButton.SetCanSubmit(true);
            }

            await UniTask.WaitForEndOfFrame();

            EventSystem.current.SetSelectedGameObject(choiceButtons[currentChoiceIndex].gameObject);
        }

        private void NavigateChoiceView(Vector2 input)
        {
            if (!isShowingChoices || currentChoiceCount == 0) return;

            if (input.y < 0)
            {
                currentChoiceIndex++;
                if (currentChoiceIndex >= currentChoiceCount) currentChoiceIndex = 0;
            }
            else if (input.y > 0)
            {
                currentChoiceIndex--;
                if (currentChoiceIndex < 0) currentChoiceIndex = currentChoiceCount - 1;
            }

            EventSystem.current.SetSelectedGameObject(choiceButtons[currentChoiceIndex].gameObject);
        }

        private void UpdateSpeakerName(UpdateSpeakerNameEvent evt)
        {
            Debug.Log(evt.name);
            nameText.text = evt.name;
        }
    }
}