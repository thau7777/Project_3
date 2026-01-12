using UnityEngine;

namespace MyRule
{
    public enum SigilType
    {
        Normal,
        Special
    }

    public enum ActiveSigilType
    {
        L_Mouse,
        R_Mouse,
        Space,
        F,
    }

    [CreateAssetMenu(fileName = "SigilSO", menuName = "Sigil/SigilSO")]
    public class SigilSO : ScriptableObject
    {
        [Header("SigilInfo")]
        public SigilType type;
        public Sprite sigilIcon;
        public string sigilName;
        [TextArea(3, 10)]
        public string sigilDesTD;
        [TextArea(3, 10)]
        public string sigilDesTB;
        public GameObject sigilPreb;
        public int rarity;
        public bool isActiveSigil;
        [ShowIf("isActiveSigil")]
        public ActiveSigilType activeSigilType;

        [ShowIf("isActiveSigil")]
        [Header("Sigil Effect")]
        public Skill sigilEffect;

        [TabGroup("Stats")]
        [LabelText("phys", "grey")]
        public int phys;
        [TabGroup("Stats")]
        [LabelText("mag", "cyan")]
        public int mag;
        [TabGroup("Stats")]
        [LabelText("health", "green")]
        public int health;
        [TabGroup("Stats")]
        [LabelText("def", "yellow")]
        public int def;
        [TabGroup("Stats")]
        [LabelText("resRate", "orange")]
        public int resRate;
        [TabGroup("Stats")]
        [LabelText("attackSpeed", "blue")]
        public float attackSpeed;
        [TabGroup("Stats")]
        [LabelText("critChance", "red")]
        public float critChance;
        [TabGroup("Stats")]
        [Range(1.5f, 2.3f)]
        public float critMult = 1.5f;

        [TabGroup("Requirements")]
        public int str;
        [TabGroup("Requirements")]
        public int intel;
        [TabGroup("Requirements")]
        public int dex;
        [TabGroup("Requirements")]
        public int faith;
        [TabGroup("Requirements")]
        public int arcane;
        [TabGroup("Requirements")]
        public int price;

        [ShowIfEnumValue("type", SigilType.Special)]
        [TabGroup("Stats")]
        public int diceFaceIncrease;
        [ShowIfEnumValue("type", SigilType.Special)]
        [TabGroup("Stats")]
        public int rewardShapeDropRate;
        [ShowIfEnumValue("type", SigilType.Special)]
        [TabGroup("Stats")]
        public int goldIncrease;

        [ShowIfEnumValue("type", SigilType.Special)]
        [TabGroup("Requirements")]
        public int numbOfRolls;
    }
}