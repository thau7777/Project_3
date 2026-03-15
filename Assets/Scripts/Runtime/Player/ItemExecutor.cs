using UnityEngine;

public class ItemExecutor : MonoBehaviour
{
    [SerializeField, TabGroup("Item Setup")]
    private TopDownItemStrategy[] _itemArray = new TopDownItemStrategy[6];
    private ItemRuntimeInstance[] _itemRuntimeInstances = new ItemRuntimeInstance[6];

    [SerializeField,TabGroup("References")]
    private InputReader _inputReader;

    private void OnEnable()
    {
        _inputReader.playerTopDownActions.onItemUse += UseItem;
    }
    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onItemUse -= UseItem;
    }

    private void Start()
    {
        InitializeItems();
    }

    private void InitializeItems()
    {
        for (int i = 0; i < 6; i++)
        {
            _itemRuntimeInstances[i] = new ItemRuntimeInstance(_itemArray[i], i,3);
        }
        EventBus<TopdownInitializeItemsEvent>.Raise(new TopdownInitializeItemsEvent(_itemRuntimeInstances));
    }
    public void AddItem(int index, TopDownItemStrategy item, int quantity = 1)
    {
        if (index < 0 || index > 5) return;
        _itemArray[index] = item;
        _itemRuntimeInstances[index] = new ItemRuntimeInstance(item, index, quantity);
        EventBus<TopdownInitializeItemsEvent>.Raise(new TopdownInitializeItemsEvent(_itemRuntimeInstances));
    }
    // ItemExecutor.cs - update UseItem and add ClearItem
    public void UseItem(int index)
    {
        if (index < 0 || index > 5) return;
        var item = _itemRuntimeInstances[index];
        if (item.Definition == null || item.IsOnCooldown || item.IsEmpty) return;

        var context = new ItemStrategyContext(transform, Vector3.zero, Quaternion.identity);
        item.Cast(context);

        if (item.Definition.loseQuantityOnUse)
        {
            item.currentQuantity--;
            if (item.IsEmpty) ClearItem(index);
        }
    }

    private void ClearItem(int index)
    {
        _itemArray[index] = null;
        _itemRuntimeInstances[index] = new ItemRuntimeInstance(null, index);
        EventBus<TopdownInitializeItemsEvent>.Raise(new TopdownInitializeItemsEvent(_itemRuntimeInstances));
    }

    public void SwapItems(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex > 5 || toIndex < 0 || toIndex > 5) return;

        (_itemArray[fromIndex], _itemArray[toIndex]) = (_itemArray[toIndex], _itemArray[fromIndex]);

        (_itemRuntimeInstances[fromIndex], _itemRuntimeInstances[toIndex])
            = (_itemRuntimeInstances[toIndex], _itemRuntimeInstances[fromIndex]);

        // fix slot indices after swap
        _itemRuntimeInstances[fromIndex].SlotIndex = fromIndex;
        _itemRuntimeInstances[toIndex].SlotIndex = toIndex;
    }
}