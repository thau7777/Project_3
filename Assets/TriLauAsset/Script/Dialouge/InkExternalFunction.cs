using Ink.Runtime;
using MyRule.Event;
using MyRule.UI;
using UnityEngine;

namespace MyRule
{
    public class InkExternalFunction
    { 
        public void Bind(Story story)
        {
            story.BindExternalFunction("OpenStore", () => OpenStore());
            story.BindExternalFunction("TriggerMiniGame", () => TriggerMiniGame());
        }

        public void Unbind(Story story)
        {
            story.UnbindExternalFunction("OpenStore");
            story.UnbindExternalFunction("TriggerMiniGame");
        }

        private void OpenStore()
        {
            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Store));
        }

        private void TriggerMiniGame()
        {
            Debug.Log("TriggerMiniGame");
            EventBus<TriggerMiniGameEvent>.Raise(new TriggerMiniGameEvent());
        }
    }
}