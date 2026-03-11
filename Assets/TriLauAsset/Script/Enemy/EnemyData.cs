using Turnbase;
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

        [Header("Stats")]

        public Character enemyPrefab;

        public int health= 100;
        public int stamina = 100;
        public int phys = 10;
        public int mag = 10;
        public int fire = 0;
        public int water = 0;
        public int frost = 0;
        public int lightning = 0;
        public int holy = 0;
        public int dark = 0;
        public int poison = 0;
        public int phyDef = 10;
        public int magDef = 10;
        public int fireDef = 0;
        public int waterDef = 0;
        public int frostDef = 0;
        public int lightningDef = 0;
        public int holyDef = 0;
        public int darkDef = 0;
        public int poisonDef = 0;
        public int resRate;
        public float attackSpeed = 5;
        public float critChance = 10;
        [Range(1.5f, 2.3f)]
        public float critMult = 1.5f;
    }
}