using MyRule.UI;
using UnityEngine;

namespace MyRule
{
    public class StoreRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask nodeLayer;

        private Card currentHover;
        private float mouseDownTime;
        private const float MaxClickDuration = 0.5f;

        public bool canInteract = false;

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
                    currentHover.OnClick();
                }
            }
        }
    }
}