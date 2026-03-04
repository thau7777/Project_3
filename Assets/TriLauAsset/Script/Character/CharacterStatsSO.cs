using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "CharacterStatsSO", menuName = "Scriptable Objects/CharacterStatsSO")]
    public class CharacterStatsSO : ScriptableObject
    {
        public int level;

        [Header("Stat Points")]
        public int runeNeed;
        public int virgor;
        public int mind;
        public int endurance;
        public int strength;
        public int dexterity;
        public int intelligence;
        public int faith;
        public int arcane;

        [Header("Base Stats")] 
        public int hp;
        public int fp;
        public int stamina;
        public int shield;
        public int phyDef;
        public int magicDef;
        public int fireDef;
        public int lightningDef;
        public int holyDef;
        public int darkDef;
        public int frostDef;
        public int waterDef;
        public int resRate;
        public float attackSpeed;

        [Header("PhysicalAttack Power")]
        public int physicalDmg;
        public int magicDmg;
        public int critChance;
        [Range(1.5f, 2.3f)]
        public float critMult;
    }
}