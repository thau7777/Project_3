using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

namespace MyRule.Event
{
    public struct EnterDialogueEvent : IEvent
    {
        public readonly string knotName;

        public EnterDialogueEvent(string knotName)
        {
            this.knotName = knotName;
        }
    }

    public struct DialougeStartedEvent : IEvent
    {

    }

    public struct DialogueFinishedEvent : IEvent
    {

    }

    public struct UpdateSpeakerNameEvent : IEvent
    {
        public readonly string name;

        public UpdateSpeakerNameEvent(string name)
        {
            this.name = name;
        }
    }

    public struct DisplayDialogueEvent : IEvent
    {
        public readonly string dialogueLine;
        public readonly List<Choice> dialogueChoices;

        public DisplayDialogueEvent(string dialougeLine, List<Choice> dialogueChoices)
        {
            this .dialogueLine = dialougeLine;
            this .dialogueChoices = dialogueChoices;
        }
    }

    public struct UpdateChoiceIndexEvent : IEvent
    {
        public readonly int index;

        public UpdateChoiceIndexEvent(int index)
        {
            this .index = index;
        }
    }

    public struct UpdateInkDialogueVariableEvent : IEvent
    {
        public readonly string name;
        public Ink.Runtime.Object value;

        public UpdateInkDialogueVariableEvent(string name, Ink.Runtime.Object value)
        {
            this .name = name;
            this .value = value;
        }
    }
}