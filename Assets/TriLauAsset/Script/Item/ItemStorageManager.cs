using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class ItemStorageManager : PersistentSingleton<ItemStorageManager>, IGameData
    {
        private ItemStorageData itemStorage;

        public ItemStorageData ItemStorage => itemStorage;


        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public bool HasEmptyItemStorageSlot() => itemStorage.GetEmptySlot() != 100;

        public void AddItemToStorage(ItemSO itemSO)
        {
            var index = itemStorage.GetEmptySlot();

            if (index < 6)
            {
                ItemData item = new ItemData(index, itemSO.itemType, itemSO.recoveryAmount);
                itemStorage.AddItem(index, item);

                Debug.Log("Add " + item.ItemType.ToString());

                EventBus<AddItemEvent>.Raise(new AddItemEvent(index, itemSO));
            }
            else
            {
                Debug.Log("Cant add item");
            }
        }

        public ItemStorageData GetItemStorage() => itemStorage;

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                if (data.MatchData.ItemStorageInMatch != null)
                {
                    itemStorage = data.MatchData.ItemStorageInMatch;

                    foreach (var item in itemStorage.Items)
                    {
                        if (item != null)
                        {
                            if (ItemManager.Instance != null)
                            {
                                ItemSO itemSO = ItemManager.Instance.GetItemByType(item.ItemType);
                                EventBus<AddItemEvent>.Raise(new AddItemEvent(item.SlotIndex, itemSO));
                            }
                        }
                    }
                }
                else
                {
                    itemStorage = new ItemStorageData();
                }
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData == null) return;

            data.MatchData.SetItemStorageInMatch(itemStorage);
        }

        public UniTask NewGame()
        {
            itemStorage = null;

            return UniTask.CompletedTask;
        }
    }
}