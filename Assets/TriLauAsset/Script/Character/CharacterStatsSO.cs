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
        public int def;
        public int resRate;
        public float attackSpeed;

        [Header("Attack Power")]
        public int physicalDmg;
        public int magicDmg;
        public float critChance;
        [Range(1.5f, 2.3f)]
        public float critMult;
    }
}