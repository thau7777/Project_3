using MyRule.CommandPattern;
using UnityEngine;

namespace MyRule.UI
{
    public class TabFadePresenter : BaseUIPresenter
    {
        private float fadeDuration;
        private CanvasGroup canvasGroup;

        public TabFadePresenter(IBaseUIView view, CanvasGroup canvasGroup, float fadeDuration) : base(view)
        {
            this.canvasGroup = canvasGroup;
            this.fadeDuration = fadeDuration;
        }

        protected override void OnSwitchPanel(SwitchPanelEvent e)
        {
            if (e.Type == view.Type && !view.IsActive)
            {
                ICommand showCommand = new PanelComand(view, canvasGroup, fadeDuration);
                CommandInvoker.ExecuteCommand(showCommand);
            }
        }
    }
}