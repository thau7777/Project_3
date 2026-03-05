using UnityEngine;

namespace MyRule
{
    public class ItemStorageView : MonoBehaviour
    {
        [SerializeField] private ItemSlotView[] slotViews;

        private EventBinding<AddItemEvent> addItemEventBinding;

        private void OnEnable()
        {
            addItemEventBinding = new EventBinding<AddItemEvent>(HandleAddItem);
            EventBus<AddItemEvent>.Register(addItemEventBinding);
        }

        private void OnDisable()
        {
            EventBus<AddItemEvent>.Deregister(addItemEventBinding);
        }

        private void HandleAddItem(AddItemEvent evt)
        {
            if (slotViews[evt.index] == null)
            {
                slotViews[evt.index].SetItem(evt.item);
            }
        }
    }
}