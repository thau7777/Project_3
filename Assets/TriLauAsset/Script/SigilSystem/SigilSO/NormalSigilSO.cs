using UnityEngine;

namespace MyRule
{
    public enum ActiveSigilType
    {
        L_Mouse,
        R_Mouse,
        Space,
        F,
    }

    [CreateAssetMenu(fileName = "SigilSO", menuName = "Sigil/NormalSigilSO")]
    public class NormalSigilSO : ScriptableObject
    {
        [Header("Sigil Info")]
        public Texture2D sigilIcon;
        public string sigilName;
        [TextArea(3, 10)]
        public string sigilDesTD;
        [TextArea(3, 10)]
        public string sigilDesTB;
        public int rarity;
        public bool isActiveSigil;
        [ShowIf("isActiveSigil")]
        public ActiveSigilType activeSigilType;

        [Header("Sigil Effect")]
        public Component sigilEffect;

        [Header("Sigil Stats")]
        public int phys;
        public int mag;
        public int health;
        public int def;
        public int resRate;
        public float attackSpeed;
        public float critChance;
        [Range(1.5f, 2.3f)]
        public float critMult = 1.5f;

        [Header("Sigil Requirements")]
        public int str;
        public int intel;
        public int dex;
        public int faith;
        public int arcane;
        public int price;
    }
}