using UnityEngine;

namespace MyRule
{
    public class ItemManager : Singleton<ItemManager>
    {
        [SerializeField] private ItemSO[] itemSOs;

        public ItemSO GetRandomItem()
        {
            int i = Random.Range(0, itemSOs.Length);

            return itemSOs[i];
        }

        public ItemSO GetItemByType(ItemType itemType)
        {
            for (int i = 0; i < itemSOs.Length; i++)
            {
                if (itemSOs[i].itemType == itemType)
                    return itemSOs[i];
            }

            return null;
        }    
    }
}