using Ink.Runtime;
using MyRule.Audio;
using MyRule.Event;
using MyRule.UI;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MyRule
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private const string SPEAKER_TAG = "speaker";
        private const string VOICE_TAG = "voice";
        
        [Header("Load Global JSON")]
        [SerializeField] private TextAsset inkJSON;

        [SerializeField] private DialogueView view;

        [SerializeField] private InputReader inputReader;
            
        private Story story;

        private int currentChoiceIndex = -1;

        private bool dialogueIsPlaying = false;

        private bool canContinueDialogue = true;

        private InkVariable dialogueVarialble;

        private InkExternalFunction inkExternalFunction;

        private EventBinding<EnterDialogueEvent> enterDialogueEventBinding;
        private EventBinding<UpdateChoiceIndexEvent> updateChoiceIndexEventBinding;
        private EventBinding<UpdateInkDialogueVariableEvent> updateInkDialogueVariableEventBinding;

        protected override void Awake()
        {
            base.Awake();

            story = new Story(inkJSON.text);
            dialogueVarialble = new InkVariable(story);
            inkExternalFunction = new InkExternalFunction();
            inkExternalFunction.Bind(story);

            GameSystemManager.Instance.Register(dialogueVarialble);
        }

        protected void OnDestroy()
        {
            inkExternalFunction.Unbind(story);

            GameSystemManager.Instance.Unregister(dialogueVarialble);
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

            if (view.IsTyping)
            {
                view.SkipTyping();
                return;
            }

            if (canContinueDialogue)
            {
                AudioManager.Instance.StopSound(Ami.BroAudio.BroAudioType.VoiceOver);

                ContinueStoryOrExitStory();
            }
        }

        public void CanContinueDialouge(bool locking) => canContinueDialogue = locking;

        public void EnterDialogue(EnterDialogueEvent evt)
        {
            if (dialogueIsPlaying) return;

            Debug.Log("EnterDialogue");

            dialogueIsPlaying = true;

            EventBus<OpenHUDEvent>.Raise(new OpenHUDEvent(false));

            if (RTSCameraController.Instance != null) RTSCameraController.Instance.LockInteract();

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

            if (RTSCameraController.Instance != null) RTSCameraController.Instance.UnlockInteract();

            EventBus<DialogueFinishedEvent>.Raise(new DialogueFinishedEvent());

            NPCManager.Instance.ExitDialogue();

            dialogueVarialble.StopListening(story);

            story.ResetState();
        }

        public void ContinueStoryOrExitStory()
        {
            if (!canContinueDialogue) return;

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

            ContinueStoryOrExitStory();
        }

        private void UpdateInkDialogueVariable(UpdateInkDialogueVariableEvent evt)
        {
            Ink.Runtime.Object inkValue = ConvertToInkValue(evt.value);
            this.dialogueVarialble.UpdateVariableState(evt.name, inkValue);
        }

        private Ink.Runtime.Object ConvertToInkValue(object value)
        {
            switch (value)
            {
                case int i:
                    return new IntValue(i);

                case bool b:
                    return new BoolValue(b);

                case string s:
                    return new StringValue(s);

                case float f:
                    return new FloatValue(f);

                case Enum e:
                    return new IntValue(Convert.ToInt32(e));

                default:
                    Debug.LogError($"Unsupported Ink variable type: {value.GetType()}");
                    return null;
            }
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
                    case VOICE_TAG:
                        AudioManager.Instance.PlayDialogueSound(tagValue);
                        break;
                    default:
                        Debug.Log("Loi tag");
                        break;
                }
            }
        }
    }
}