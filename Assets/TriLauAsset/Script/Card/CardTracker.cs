using MyRule.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class CardTracker : Singleton<CardTracker>
    {
        private Card currentHover;
        private float mouseDownTime;
        private const float MaxClickDuration = 0.5f;

        private bool canInteract = false;

        private EventBinding<HoverCardEvent> hoverEventBinding;

        private void OnEnable()
        {
            hoverEventBinding = new EventBinding<HoverCardEvent>(HandleHover);
            EventBus<HoverCardEvent>.Register(hoverEventBinding);
        }

        private void OnDisable()
        {
            EventBus<HoverCardEvent>.Deregister(hoverEventBinding);
        }

        void Update()
        {
            HandleClick();
        }

        public void UnlockInteract(bool locked) => canInteract = locked;

        private void HandleHover(HoverCardEvent evt)
        {
            if (!canInteract) return;

            currentHover = evt.card;
        }

        private void HandleClick()
        {
            if (!canInteract) return;

            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                mouseDownTime = Time.time;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (Time.time - mouseDownTime < MaxClickDuration && currentHover != null)
                {
                    currentHover.OnClick();
                }
            }
            else if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                EventBus<ShowCardDetailEvent>.Raise(new ShowCardDetailEvent(currentHover.SigilSO, currentHover.SigilData));
            }
        }
    }
}