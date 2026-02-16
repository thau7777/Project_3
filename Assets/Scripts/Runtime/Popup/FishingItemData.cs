using UnityEngine;

[CreateAssetMenu(fileName = "FishingItemData", menuName = "Scriptable Objects/FishingItemData")]
public class FishingItemData : ScriptableObject
{
    public string itemName;
    public bool isFish;
    public GameObject prefab;
}
