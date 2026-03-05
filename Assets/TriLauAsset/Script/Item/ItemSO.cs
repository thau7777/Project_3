using UnityEngine;

namespace MyRule
{
    public enum ItemType
    {
        HealthPotion,
        ManaPotion,
    }

    [CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
    public class ItemSO : ScriptableObject
    {
        public ItemType itemType;
        public Sprite icon;
        public int recoveryAmount;
    }
}