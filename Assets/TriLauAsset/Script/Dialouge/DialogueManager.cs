using Ink.Runtime;
using MyRule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private const string SPEAKER_TAG = "speaker";
        
        [Header("Load Global JSON")]
        [SerializeField] private TextAsset inkJSON;

        [SerializeField] private InputReader inputReader;
            
        private Story story;

        private int currentChoiceIndex = -1;

        private bool dialogueIsPlaying = false;

        public bool CanContinueDialogue = true;

        private DialogueVariable dialogueVarialble;

        private InkExternalFunction inkExternalFunction;

        private EventBinding<EnterDialogueEvent> enterDialogueEventBinding;
        private EventBinding<UpdateChoiceIndexEvent> updateChoiceIndexEventBinding;
        private EventBinding<UpdateInkDialogueVariableEvent> updateInkDialogueVariableEventBinding;

        protected override void Awake()
        {
            base.Awake();

            story = new Story(inkJSON.text);
            dialogueVarialble = new DialogueVariable(story);
            inkExternalFunction = new InkExternalFunction();
            inkExternalFunction.Bind(story);
        }

        protected void OnDestroy()
        {
            inkExternalFunction.Unbind(story);
        }

        private void OnEnable()
        {
            inputReader.diceRollActions.onSubmit += OnSubmit;

            enterDialogueEventBinding = new EventBinding<EnterDialogueEvent>(EnterDialogue);
            EventBus<EnterDialogueEvent>.Register(enterDialogueEventBinding);

            updateChoiceIndexEventBinding = new EventBinding<UpdateChoiceIndexEvent>(UpdateChoiceIndex);
            EventBus<UpdateChoiceIndexEvent>.Register(updateChoiceIndexEventBinding);

            updateInkDialogueVariableEventBinding = new EventBinding<UpdateInkDialogueVariableEvent>(UpdateInkDialogueVariable);
            EventBus<UpdateInkDialogueVariableEvent>.Register(updateInkDialogueVariableEventBinding);
        }

        private void OnDisable()
        {
            inputReader.diceRollActions.onSubmit -= OnSubmit;

            EventBus<EnterDialogueEvent>.Deregister(enterDialogueEventBinding);
            EventBus<UpdateChoiceIndexEvent>.Deregister(updateChoiceIndexEventBinding);
            EventBus<UpdateInkDialogueVariableEvent>.Deregister(updateInkDialogueVariableEventBinding);
        }

        private void OnSubmit()
        {
            if (!dialogueIsPlaying) return;

            if (CanContinueDialogue) ContinueStoryOrExitStory();
            else return;
        }

        public void EnterDialogue(EnterDialogueEvent evt)
        {
            if (dialogueIsPlaying) return;

            Debug.Log("EnterDialogue");

            dialogueIsPlaying = true;

            EventBus<OpenHUDEvent>.Raise(new OpenHUDEvent(false));

            EventBus<DialougeStartedEvent>.Raise(new DialougeStartedEvent());

            if (!evt.knotName.Equals(""))
            {
                story.ChoosePathString(evt.knotName);
                Debug.Log("ChoosePath " +  evt.knotName);
            }

            dialogueVarialble.SyncVariablesAndStartListening(story);

            ContinueStoryOrExitStory();
        }

        private void ExitDialogue()
        {
            dialogueIsPlaying = false;

            EventBus<OpenHUDEvent>.Raise(new OpenHUDEvent(true));

            EventBus<DialogueFinishedEvent>.Raise(new DialogueFinishedEvent());

            NPCManager.Instance.ExitDialogue();

            dialogueVarialble.StopListening(story);

            story.ResetState();
        }

        public void ContinueStoryOrExitStory()
        {
            if (!CanContinueDialogue) return;

            if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
            {
                story.ChooseChoiceIndex(currentChoiceIndex);
                currentChoiceIndex = -1;
            }

            if (story.canContinue)
            {
                string dialogueLine = story.Continue();
                HandleTags(story.currentTags);

                while (IsLineBlank(dialogueLine) && story.canContinue)
                {
                    dialogueLine = story.Continue();
                }

                if (IsLineBlank(dialogueLine) && !story.canContinue)
                {
                    ExitDialogue();
                }
                else
                {
                    EventBus<DisplayDialogueEvent>.Raise(new DisplayDialogueEvent(dialogueLine, story.currentChoices));
                }
            }            
            else if (story.currentChoices.Count == 0)
            {
                ExitDialogue();
            }
        }

        private void UpdateChoiceIndex(UpdateChoiceIndexEvent evt)
        {
            this.currentChoiceIndex = evt.index;
        }

        private void UpdateInkDialogueVariable(UpdateInkDialogueVariableEvent evt)
        {
            this.dialogueVarialble.UpdateVariableState(evt.name, evt.value);
        }

        private bool IsLineBlank(string dialogueLine)
        {
            return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
        }

        private void HandleTags(List<string> tags)
        {
            foreach (var tag in tags)
            {
                string[] splitTag = tag.Split(':');
                if (splitTag.Length < 2)
                {
                    Debug.LogError("loi");
                }
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                switch (tagKey)
                {
                    case SPEAKER_TAG:
                        EventBus<UpdateSpeakerNameEvent>.Raise(new UpdateSpeakerNameEvent(tagValue));
                        break;
                    default:
                        Debug.Log("Loi tag");
                        break;
                }
            }
        }
    }
}