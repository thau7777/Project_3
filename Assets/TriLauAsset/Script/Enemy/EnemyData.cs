using UnityEngine;
namespace MyRule
{
    public enum EnemyId
    {
        Vampy,
        BlackKnight,
        DemonKing,
        Doragon,
        NagaWizard,
        Salamander,
        BishopKnight,
        Golem,
        Beholder,
        Necropolis,
        Spector,
        BattleBee,
        Gargoyle,
        Werewolf,
        Mimic,
        Cyclops,
        Orc,
        Cactus,
        Nepenthe,
        RatAssassin,
        Craber,
        Myrmidon,
        StingRay,
    }

    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
    public class EnemyDataSO : ScriptableObject
    {
        public EnemyId enemyId;
        public int level;

        [TabGroup("Base Stats")]
        public int hp;
        [TabGroup("Base Stats")]
        public int fp;
        [TabGroup("Base Stats")]
        public int stamina;

        [TabGroup("Defense Stats")]
        public int phyDef;
        [TabGroup("Defense Stats")]
        public int magDef;
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
        public float attackSpeed;
        [TabGroup("Attack Stats")]
        public float critChance;
        [TabGroup("Attack Stats")]
        [Range(1.5f, 2.3f)]
        public float critMult = 1.5f;
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