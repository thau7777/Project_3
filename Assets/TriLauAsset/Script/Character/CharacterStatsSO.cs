using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "CharacterStatsSO", menuName = "Scriptable Objects/CharacterStatsSO")]
    public class CharacterStatsSO : ScriptableObject
    {
        public int level;
    
        [TabGroup("Stat Points")]
        public int rune;
        [TabGroup("Stat Points")]
        public int virgor;
        [TabGroup("Stat Points")]
        public int mind;
        [TabGroup("Stat Points")]
        public int endurance;
        [TabGroup("Stat Points")]
        public int strength;
        [TabGroup("Stat Points")]
        public int dexterity;
        [TabGroup("Stat Points")]
        public int intelligence;
        [TabGroup("Stat Points")]
        public int faith;
        [TabGroup("Stat Points")]
        public int arcane;

        [TabGroup("Base Stats")] 
        public int hp;
        [TabGroup("Base Stats")] 
        public int fp;
        [TabGroup("Base Stats")] 
        public int stamina;

        [TabGroup("Defense Stats")]
        public int phyDef;
        [TabGroup("Defense Stats")]
        public int magicDef;
        [TabGroup("Defense Stats")]
        public int fireDef;
        [TabGroup("Defense Stats")]
        public int lightningDef;
        [TabGroup("Defense Stats")]
        public int holyDef;
        [TabGroup("Defense Stats")]
        public int darkDef;
        [TabGroup("Defense Stats")]
        public int frostDef;
        [TabGroup("Defense Stats")]
        public int waterDef;
        [TabGroup("Defense Stats")]
        public int poisonDef;

        [TabGroup("Attack Stats")]
        public float speed;
        [TabGroup("Attack Stats")]
        public int attackDmg;
        [TabGroup("Attack Stats")]
        public int magicDmg;
        [TabGroup("Attack Stats")]
        public int critChance;
        [TabGroup("Attack Stats")]
        [Range(1.5f, 2.3f)]
        public float critMult;
        [TabGroup("Attack Stats")]
        public int fireDmg;
        [TabGroup("Attack Stats")]
        public int lightningDmg;
        [TabGroup("Attack Stats")]
        public int holyDmg;
        [TabGroup("Attack Stats")]
        public int darkDmg;
        [TabGroup("Attack Stats")]
        public int frostDmg;
        [TabGroup("Attack Stats")]
        public int waterDmg;
        [TabGroup("Attack Stats")]
        public int poisonDmg;

    }
}