using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class ItemStorageManager : PersistentSingleton<ItemStorageManager>, IGameData
    {
        private ItemStorageData itemStorage;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void AddItemToStorage(ItemSO itemSO)
        {
            var index = itemStorage.GetEmptySlot();

            if (index < 6)
            {
                ItemData item = new ItemData(itemSO);
                itemStorage.AddItem(index, item);

                Debug.Log("Add " + item.Type.ToString());

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
            if (data.MatchData.ItemStorageInMatch != null)
            {
                itemStorage = data.MatchData.ItemStorageInMatch;
            }
            else
            {
                itemStorage = new ItemStorageData();
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.MatchData.SetItemStorageInMatch(itemStorage);
        }
    }
}