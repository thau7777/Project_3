using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule
{
    public class SigilView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private SigilSO sigilSO;
        [SerializeField] private bool canDrag = false;
        [SerializeField] private bool isPassive = false;

        private Canvas rootCanvas;

        public Transform parentAfterDrag;
        public bool IsEmpty => sigilSO == null;

        private void Awake()
        {
            rootCanvas = GetComponentInParent<Canvas>();
            icon.raycastTarget = false;
        }

        public void SetSigil(SigilSO sigilSO)
        {
            this.sigilSO = sigilSO;
            canDrag = true;
            icon.raycastTarget = true;
            Color c = icon.color;
            c.a = 1f;
            icon.color = c;

            icon.sprite = sigilSO.sigilIcon;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canDrag || IsEmpty) return;

            if (isPassive) return;

            icon.raycastTarget = false;
            parentAfterDrag = transform.parent;
            transform.SetParent(rootCanvas.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag || IsEmpty) return;

            if (isPassive) return;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rootCanvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPos
            );

            transform.position = worldPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag || IsEmpty) return;

            if (isPassive) return;

            icon.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
        }
    }
}