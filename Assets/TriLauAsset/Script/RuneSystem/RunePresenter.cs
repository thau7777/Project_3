using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule.UI
{
    public class RunePresenter
    {
        private IRuneView view;
        private int currentRune;

        private EventBinding<SendUIRuneEvent> sendUIRuneEventBinding;

        public RunePresenter(IRuneView view)
        {
            this.view = view;

            sendUIRuneEventBinding = new EventBinding<SendUIRuneEvent>(HandleRuneEvent);
            EventBus<SendUIRuneEvent>.Register(sendUIRuneEventBinding);
        }

        public void Clearup()
        {
            this.view = null;
            EventBus<SendUIRuneEvent>.Deregister(sendUIRuneEventBinding);
        }

        private void HandleRuneEvent(SendUIRuneEvent e)
        {
            if (e.runeLockAmount > 0)
            {
                view.SetRuneLock(true, e.runeLockAmount.ToString());
            }
            else
            {
                view.SetRuneLock(false);
            }

            view.AdjustRune(e.runAmount);
        }
    }
}