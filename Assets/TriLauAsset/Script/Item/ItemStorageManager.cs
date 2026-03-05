using UnityEngine;

namespace MyRule
{
    public class ItemStorageManager : PersistentSingleton<ItemStorageManager>
    {
        [SerializeField] private ItemStorage itemStorage;
        [SerializeField] private int itemCount = 6;

        private void Start()
        {
            itemStorage = new ItemStorage(itemCount);
        }

        public void ResetItemStorage()
        {
            itemStorage = new ItemStorage(itemCount);
        }

        public void AddItemToStorage(ItemSO itemSO)
        {
            var index = itemStorage.GetEmptySlot();

            if (index < 6)
            {
                Item item = new Item(itemSO);
                itemStorage.AddItem(index, item);
            }
        }

        public ItemStorage GetItemStorage() => itemStorage;
    }
}