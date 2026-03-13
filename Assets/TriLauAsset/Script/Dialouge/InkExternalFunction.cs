using Ink.Runtime;
using MyRule.UI;
using UnityEngine;

namespace MyRule
{
    public class InkExternalFunction
    { 
        public void Bind(Story story)
        {
            story.BindExternalFunction("OpenStore", () => OpenStore());
        }

        public void Unbind(Story story)
        {
            story.UnbindExternalFunction("OpenStore");
        }

        private void OpenStore()
        {
            EventBus<SwitchPanelEvent>.Raise(new SwitchPanelEvent(PanelType.Store));
        }
    }
}