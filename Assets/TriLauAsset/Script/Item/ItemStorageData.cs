using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class ItemStorageData
    {
        private const int MAX_LENGHT = 6;
        
        [JsonProperty] private ItemData[] items;

        [JsonIgnore] public int Lenght => items.Length;
        [JsonIgnore] public ItemData[] Items => items;

        public ItemStorageData()
        {
            this.items = new ItemData[MAX_LENGHT];
        }

        public void AddItem(int index, ItemData item) => items[index] = item;

        public void RemoveItemSlot(int index) => items[index] = null;

        public int GetEmptySlot()
        {
            for (int i = 0; i < Lenght; i++)
            {
                if (items[i] == null) return i;
            }

            return Lenght;
        }
    }

    [Serializable]
    public class ItemData
    {
        [JsonProperty] private ItemType type;
        [JsonProperty] private Sprite icon;
        [JsonProperty] private int price;
        [JsonProperty] private int recoveryAmount;

        [JsonIgnore] public ItemType Type => type;
        [JsonIgnore] public Sprite Icon => icon;
        [JsonIgnore] public int Price => price;
        [JsonIgnore] public int RecoveryAmount => recoveryAmount;

        public ItemData(ItemSO itemSO)
        {
            type = itemSO.itemType;
            icon = itemSO.icon;
            price = itemSO.price;
            recoveryAmount = itemSO.recoveryAmount;
        }
    }
}