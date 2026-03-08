using System;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class ItemStorage
    {
        private Item[] items;

        public int Lenght => items.Length;

        public ItemStorage(int lenght)
        {
            this.items = new Item[lenght];
        }

        public void AddItem(int index, Item item)
        {
            items[index] = item;
        }

        public void RemoveItemSlot(int index)
        {
            items[index] = null;
        }

        public int GetEmptySlot()
        {
            for (int i = 0; i < Lenght; i++)
            {
                if (items[i] == null) return i;
            }

            return Lenght;
        }

        public void Clear()
        {
            items = null;
        }
    }

    [Serializable]
    public class Item
    {
        private ItemType type;
        private Sprite icon;
        private int price;
        private int recoveryAmount;

        public ItemType Type => type;
        public Sprite Icon => icon;
        public int Price => price;
        public int RecoveryAmount => recoveryAmount;

        public Item(ItemSO itemSO)
        {
            type = itemSO.itemType;
            icon = itemSO.icon;
            price = itemSO.price;
            recoveryAmount = itemSO.recoveryAmount;
        }
    }
}