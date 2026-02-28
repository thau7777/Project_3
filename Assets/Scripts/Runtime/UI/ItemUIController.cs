// ItemUIController.cs
using UnityEngine;

public class ItemUIController : MonoBehaviour
{
    private EventBinding<TopDownInitializeItemsEvent> _itemInitializeEventBinding;

    [SerializeField] private ItemSlotUI[] _slots = new ItemSlotUI[6];

    private ItemRuntimeInstance[] _itemRuntimeInstances;

    private void OnEnable()
    {
        _itemInitializeEventBinding = new(OnItemInitialize);
        EventBus<TopDownInitializeItemsEvent>.Register(_itemInitializeEventBinding);
    }

    private void OnDisable()
    {
        EventBus<TopDownInitializeItemsEvent>.Deregister(_itemInitializeEventBinding);
    }

    private void OnItemInitialize(TopDownInitializeItemsEvent evt)
    {
        _itemRuntimeInstances = evt.items;
        for (int i = 0; i < _slots.Length; i++)
            _slots[i].Bind(_itemRuntimeInstances[i]);
    }

    private void Update()
    {
        if (_itemRuntimeInstances.IsNullOrEmpty()) return;
        foreach (var slot in _slots)
            slot.Tick();
    }
}