using UnityEngine;

namespace MyRule
{
    public enum SigilType
    {
        Active,
        Passive
    }

    public enum EKeyBinding
    {
        Q,
        E,
        F,
        R,
    }

    [CreateAssetMenu(fileName = "SigilSO", menuName = "Sigil/SigilSO")]
    public class SigilSO : ScriptableObject
    {
        [Header("SigilInfo")]
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

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
            }
        }

        public SigilType sigilType;
        public Sprite sigilIcon;
        public string sigilName;
        [TextArea(3, 10)]
        public string sigilDesTD;
        [TextArea(3, 10)]
        public string sigilDesTB;
        public GameObject sigilPreb;
        public ERarity rarity;
        [ShowIfEnumValue("sigilType", SigilType.Active)]
        public EKeyBinding keyBinding;

        [ShowIfEnumValue("sigilType", SigilType.Active)]
        [Header("Sigil Effect")]
        public Skill sigilEffect;

        public int baseDmg;
        public int manaCost;

        [TabGroup("AttributePoints")]
        public int vigor;
        [TabGroup("AttributePoints")]
        public int mind;
        [TabGroup("AttributePoints")]
        public int endurance;
        [TabGroup("AttributePoints")]
        public int strength;
        [TabGroup("AttributePoints")]
        public int dexterity;
        [TabGroup("AttributePoints")]
        public int intelligence;
        [TabGroup("AttributePoints")]
        public int faith;
        [TabGroup("AttributePoints")]
        public int arcane;

        [TabGroup("Stats")]
        public int phys;
        [TabGroup("Stats")]
        public int mag;
        [TabGroup("Stats")]
        public int fire;
        [TabGroup("Stats")]
        public int frost;
        [TabGroup("Stats")] 
        public int lightning;
        [TabGroup("Stats")] 
        public int holy;
        [TabGroup("Stats")] 
        public int dark;
        [TabGroup("Stats")] 
        public int water;
        [TabGroup("Stats")]
        public int poison;
        [TabGroup("Stats")]
        public int health;
        [TabGroup("Stats")]
        public int phyDef;
        [TabGroup("Stats")]
        public int magicDef;
        [TabGroup("Stats")]
        public int fireDef;
        [TabGroup("Stats")]
        public int lightningDef;
        [TabGroup("Stats")]
        public int holyDef;
        [TabGroup("Stats")]
        public int darkDef;
        [TabGroup("Stats")]
        public int frostDef;
        [TabGroup("Stats")]
        public int waterDef;
        [TabGroup("Stats")]
        public int poisonDef;
        [TabGroup("Stats")]
        public float speed;
        [TabGroup("Stats")]
        public int critChance;
        [TabGroup("Stats")]
        [Range(1.5f, 2.3f)]
        public float critMult = 1.5f;

        [TabGroup("Requirements")]
        public int price;
    }
}