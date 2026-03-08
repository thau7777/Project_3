using MyRule.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace MyRule
{
    public class MapNodeRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask nodeLayer;
        [SerializeField] private InputReader inputReader;

        private MapNode currentHover;
        private float mouseDownTime;
        private const float MaxClickDuration = 0.5f;

        private bool canInteract = true;

        private EventBinding<SwitchPanelEvent> evtBinding;

        private void OnEnable()
        {
            evtBinding = new EventBinding<SwitchPanelEvent>(HandlePanel);
            EventBus<SwitchPanelEvent>.Register(evtBinding);
            inputReader.diceRollActions.onEsc += OnEsc;
        }

        private void OnDisable()
        {
            EventBus<SwitchPanelEvent>.Deregister(evtBinding);
            inputReader.diceRollActions.onEsc -= OnEsc;
        }

        void Update()
        {
            HandleHover();
            HandleClick();
        }

        private void HandleHover()
        {
            if (!canInteract) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 400f, nodeLayer))
            {
                MyRule.MapNode node = hit.collider.GetComponent<MyRule.MapNode>();

                if (node != null && node != currentHover)
                {
                    currentHover?.OnHoverExit();
                    currentHover = node;
                    currentHover.OnHoverEnter();
                }
            }
            else
            {
                if (currentHover != null)
                {
                    currentHover.OnHoverExit();
                    currentHover = null;
                }
            }
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

        private void HandlePanel(SwitchPanelEvent evt)
        {
            if (!canInteract) return;

            switch (evt.Type)
            {
                case PanelType.Stats:
                case PanelType.Store:
                    canInteract = false;
                    break;
            }
        }

        private void OnEsc()
        {
            if (canInteract) return;
            canInteract = true;
        }
    }
}