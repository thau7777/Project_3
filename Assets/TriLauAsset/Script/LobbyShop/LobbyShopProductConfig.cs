using UnityEngine;


namespace MyRule
{
    public enum EProduct
    {
        Sigil,
        Card,
        Gold,
        Crystal,
    }

    public enum EUnit
    {
        Gold,
        Crystal,
        RealMoney,
    }

    [CreateAssetMenu(fileName = "LobbyShopProductConfig", menuName = "Scriptable Objects/LobbyShopProductConfig")]
    public class LobbyShopProductConfig : ScriptableObject
    {
        [SerializeField] public string id;
#if UNITY_EDITOR
        [ContextMenu("Generate New ID")]
        public void GenerateNewID()
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
        public string productName;
        public Sprite icon;
        public EProduct productType;
        public EUnit unit;

        [ShowIfEnumValue("productType", EProduct.Sigil)]
        public SigilSO sigilSO;

        [ShowIfEnumValue("productType", EProduct.Card)]
        public CardConfig cardConfig;

        [ShowIfEnumValue("productType", EProduct.Gold)]
        public int gold;

        [ShowIfEnumValue("productType", EProduct.Crystal)]
        public int crystal;

        public int prices;
    }
}