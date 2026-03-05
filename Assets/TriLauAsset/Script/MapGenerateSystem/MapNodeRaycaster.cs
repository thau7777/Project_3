using UnityEngine;

namespace MyRule
{
    public class MapNodeRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask nodeLayer;

        private MapNode currentHover;
        private float mouseDownTime;
        private const float MaxClickDuration = 0.5f;

        void Update()
        {
            HandleHover();
            HandleClick();
        }

        void HandleHover()
        {
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

        void HandleClick()
        {
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