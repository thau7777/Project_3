using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRule.UI
{
    public class SigilSlotView : MonoBehaviour, IDropHandler
    {
        [SerializeField] private int slotIndex;
        [SerializeField] private bool canDrop = true;
        public SigilView sigilView;
     
        private bool isEmpty = true;

        public int SlotIndex => slotIndex;

        public bool IsEmpty => sigilView.IsEmpty;

        public void OnDrop(PointerEventData eventData)
        {
            if (!canDrop) return;

            SigilView draggedSigilView = eventData.pointerDrag.GetComponent<SigilView>();
            if (draggedSigilView == null) return;

            Transform draggedOriginSlot = draggedSigilView.parentAfterDrag;
            sigilView.transform.SetParent(draggedOriginSlot);
            sigilView.parentAfterDrag = draggedOriginSlot;
            SigilSlotView sigilSlotView = draggedOriginSlot.GetComponent<SigilSlotView>();
            sigilSlotView.sigilView = sigilView;

            draggedSigilView.transform.SetParent(transform);
            draggedSigilView.parentAfterDrag = transform;
            sigilView = draggedSigilView;

            int draggedSlotIndex = sigilSlotView.SlotIndex;

            SigilStorageManager.Instance.SigilStorageData.SwapActiveSigil(draggedSlotIndex, slotIndex);
        }

        public void SetSigilView(SigilSO sigilSO)
        {
            if (sigilView != null) sigilView.SetSigil(sigilSO);
        }
    }
}