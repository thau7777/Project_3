using UnityEngine;

namespace MyRule
{
    public enum ItemType
    {
        HealthPotion,
        ManaPotion,
        PhysicDmgPotion,
        MagicDmgPotion,
        FireDmgPotion,
        WaterDmgPotion,
        FrostDmgPotion,
        HolyDmgPotion,
        DarkDmgPotion,
        PoisonDmgPotion,
        PhysicDefPotion,
        MagicDefPotion,
        FireDefPotion,
        WaterDefPotion,
        FrostDefPotion,
        HolyDefPotion,
        DarkDefPotion,
        PosionDefPotion,
        LightingDmgPotion,
        LightningDefPotion,
    }

    [CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
    public class ItemSO : ScriptableObject
    {
        public ItemType itemType;
        public Sprite icon;
        public int price;
        public int recoveryAmount;
    }
}