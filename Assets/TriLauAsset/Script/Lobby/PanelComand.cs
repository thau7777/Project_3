using Cysharp.Threading.Tasks;
using MyRule.CommandPattern;
using MyRule.UI;
using System.Threading;
using UnityEngine;

namespace MyRule
{
    public class PanelComand : ICommand
    {
        private IBaseUIView panel;
        private CanvasGroup canvasGroup;
        private float fadeDuration;
        private CancellationTokenSource cts;


        public PanelComand(IBaseUIView panel, CanvasGroup canvasGroup, float duration)
        {
            this.panel = panel;
            this.canvasGroup = canvasGroup;
            this.fadeDuration = duration;
            cts = new CancellationTokenSource();
        }

        public void Execute()
        {
            if (panel.IsActive) return;

            panel.IsActive = true;
            panel.Show();
            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 1f,
                duration: fadeDuration,
                cts.Token).Forget();
        }

        public void Undo()
        {
            if (!panel.IsActive) return;

            panel.IsActive = false;
            Transition.TransitionValue(
                setter: value => canvasGroup.alpha = value,
                from: canvasGroup.alpha,
                to: 0f,
                duration: fadeDuration,
                cts.Token).Forget();
            panel.Hide();
        }
    }
}