using MyRule;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TopdownItemDataBase : Singleton<TopdownItemDataBase>
{
    [Serializable]  
    public struct TopdownItemData
    {
        public ItemType Type;
        public TopDownItemStrategy itemStrategy;
    }

    [SerializeField]
    private List<TopdownItemData> _itemList;

    public TopDownItemStrategy GetItemStrategyByType(ItemType type)
    {
        foreach (var item in _itemList)
        {
            if(type == item.Type)
                return item.itemStrategy;
        }
        return null;
    }

}
