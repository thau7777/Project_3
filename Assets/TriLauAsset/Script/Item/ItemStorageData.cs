using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    [JsonObject]
    public class ItemStorageData
    {
        private const int MAX_LENGHT = 6;
        
        [JsonProperty] private ItemData[] items;

        [JsonIgnore] public int Lenght => items.Length;
        [JsonIgnore] public ItemData[] Items => items;

        [JsonConstructor]
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

            return 100;
        }
    }

    [JsonObject]
    public class ItemData
    {
        [JsonProperty] private int slotIndex;
        [JsonProperty] private ItemType itemType;
        [JsonProperty] private int recoveryAmount;

        [JsonIgnore] public int SlotIndex => slotIndex;
        [JsonIgnore] public ItemType ItemType => itemType;
        [JsonIgnore] public int RecoveryAmount => recoveryAmount;

        [JsonConstructor]
        public ItemData(int slotIndex, ItemType itemType, int recoveryAmount)
        {
            this.slotIndex = slotIndex;
            this.itemType = itemType;
            this.recoveryAmount = recoveryAmount;
        }
    }
}