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

        public int phys;
        public int mag;
        public int fire;
        public int water;
        public int frost;
        public int lightning;
        public int holy;
        public int dark;
        public int poison;
        public int health;
        public int phyDef;
        public int magDef;
        public int fireDef;
        public int waterDef;
        public int frostDef;
        public int lightningDef;
        public int holyDef;
        public int darkDef;
        public int poisonDef;
        public int resRate;
        public float attackSpeed;
        public float critChance;
        [Range(1.5f, 2.3f)]
        public float critMult = 1.5f;
    }
}