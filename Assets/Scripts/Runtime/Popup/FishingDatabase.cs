using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishingDatabase", menuName = "Scriptable Objects/FishingDatabase")]
public class FishingDatabase : ScriptableObject
{
    public List<FishingItemData> items;

    public FishingItemData GetRandomItem()
    {
        return items[Random.Range(0,items.Count)];

        //List<FishingItemData> fishItems = items.FindAll(item => item.isFish);
        //if (fishItems.Count == 0)
        //{
        //    Debug.LogWarning("No fish items found in the database!");
        //    return null;
        //}
        //int randomIndex = Random.Range(0, fishItems.Count);
        //return fishItems[randomIndex];
    }
}
