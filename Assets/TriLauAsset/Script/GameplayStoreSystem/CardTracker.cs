using MyRule.UI;
using UnityEngine;

namespace MyRule
{
    public class CardTracker : Singleton<CardTracker>
    {
        private Card currentHover;
        private float mouseDownTime;
        private const float MaxClickDuration = 0.5f;

        public bool canInteract = false;

        public bool isReward = false;

        private EventBinding<HoverSigilCardEvent> hoverEventBinding;

        private void OnEnable()
        {
            hoverEventBinding = new EventBinding<HoverSigilCardEvent>(HandleHover);
            EventBus<HoverSigilCardEvent>.Register(hoverEventBinding);
        }

        private void OnDisable()
        {
            EventBus<HoverSigilCardEvent>.Deregister(hoverEventBinding);
        }

        void Update()
        {
            HandleClick();
        }

        private void HandleHover(HoverSigilCardEvent evt)
        {
            if (!canInteract) return;

            currentHover = evt.card;
            Debug.Log("Hover" +  currentHover);
        }

        private void HandleClick()
        {
            if (!canInteract) return;

            if (Input.GetMouseButtonDown(0))
            {
                mouseDownTime = Time.time;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (Time.time - mouseDownTime < MaxClickDuration && currentHover != null)
                {
                    if (isReward)
                    {
                        currentHover.OnClick(isReward);
                        isReward = false;
                        canInteract = false;
                    }
                    else
                    {
                        currentHover.OnClick(false);
                    }
                }
            }
        }
    }
}