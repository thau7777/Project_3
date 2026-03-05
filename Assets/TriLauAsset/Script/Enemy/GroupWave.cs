using System;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class GroupWave
    {
        [Range(2, 4)]
        private int waveCount;
        private WaveData[] waveDatas;

        public GroupWave(int waveCount)
        {
            this.waveCount = waveCount;
            this.waveDatas = new WaveData[waveCount];
        }

        public void AddWaveData(int index, WaveData waveData)
        {
            if (index >= 0 && index < waveCount)
            {
                waveDatas[index] = waveData;
            }
        }
    }

    [Serializable]
    public class WaveData
    {
        private int enemyCount;
        private EnemyData[] enemies;

        public WaveData(int maxEnemyCount)
        {
            enemies = new EnemyData[maxEnemyCount];
        }

        public void AddEnemy(int index, EnemyData enemyData)
        {
            enemies[index] = enemyData;
        }
    }

    [Serializable]
    public class EnemyData
    {
        private EnemyId enemyId;
        private int phys;
        private int mag;
        private int fire;
        private int frost;
        private int lightning;
        private int holy;
        private int dark;
        private int water;
        private int poison;
        private int health;
        private int phyDef;
        private int magDef;
        private int fireDef;
        private int frostDef;
        private int lightningDef;
        private int holyDef;
        private int darkDef;
        private int waterDef;
        private int poisonDef;
        private int resRate;
        private float attackSpeed;
        private float critChance;
        private float critMult;

        public EnemyId EnemyId => enemyId;
        public int Phys => phys;
        public int Mag => mag;
        public int Fire => fire;
        public int Frost => frost;
        public int Lightning => lightning;
        public int Holy => holy;
        public int Dark => dark;
        public int Water => water;
        public int Poison => poison;
        public int Health => health;
        public int PhyDef => phyDef;
        public int MagDef => magDef;
        public int FireDef => fireDef;
        public int FrostDef => frostDef;
        public int LightningDef => lightningDef;
        public int HolyDef => holyDef;
        public int DarkDef => darkDef;
        public int WaterDef => waterDef;
        public int PoisonDef => poisonDef;
        public int ResRate => resRate;
        public float AttackSpeed => attackSpeed;
        public float CritChance => critChance;
        public float CritMult => critMult;

        public EnemyData(int level, EnemyId enemyId, int phys, int mag, int fire, int frost, int lightning, int holy, int dark, int water, int poison, int health, int phydef, int magdef, int firedef, int frostdef, int lightningdef, int holydef, int darkdef, int waterdef, int poisonDef, int resRate, float attackSpeed, float critChance, float critMult)
        {
            this.enemyId = enemyId;
            this.phys = phys;
            this.mag = mag;
            this.fire = fire;
            this.frost = frost;
            this.lightning = lightning;
            this.holy = holy;
            this.dark = dark;
            this.water = water;
            this.poison = poison;
            this.health = health;
            this.phyDef = phydef;
            this.magDef = magdef;
            this.fireDef = firedef;
            this.frostDef = frostdef;
            this.lightningDef = lightningdef;
            this.holyDef = holydef;
            this.darkDef = darkdef;
            this.waterDef = waterdef;
            this.poisonDef = poisonDef;
            this.resRate = resRate;
            this.attackSpeed = attackSpeed;
            this.critChance = critChance;
            this.critMult = critMult;
             
            if (level <= 4)
            {
                ScaleStats(level, mult: 0.2f);
            }
            else if (level <= 8)
            {
                ScaleStats(level, mult: 0.4f);
            }
            else if (level <= 10)
            {
                ScaleStats(level, mult: 0.6f);
            }
            else
            {
                ScaleStats(level, mult: 0.8f);
            }
        }

        private void ScaleStats(int level, float mult)
        {
            this.phys = (int)(phys * (1 + level * mult));
            this.mag = (int)(mag * (1 + level * mult));
            this.fire = (int)(fire * (1 + level * mult));
            this.frost = (int)(frost * (1 + level * mult));
            this.lightning = (int)(lightning * (1 + level * mult));
            this.holy = (int)(holy * (1 + level * mult));
            this.dark = (int)(dark * (1 + level * mult));
            this.water = (int)(water * (1 + level * mult));
            this.poison = (int)(poison * (1 + level * mult));
            this.health = (int)(health * (1 + level * mult));
            this.phyDef = (int)(phyDef * (1 + level * mult));
            this.magDef = (int)(magDef * (1 + level * mult));
            this.fireDef = (int)(fireDef * (1 + level * mult));
            this.frostDef = (int)(frostDef * (1 + level * mult));
            this.lightningDef = (int)(lightningDef * (1 + level * mult));
            this.holyDef = (int)(holyDef * (1 + level * mult));
            this.darkDef = (int)(darkDef * (1 + level * mult));
            this.waterDef = (int)(waterDef * (1 + level * mult));
            this.poisonDef = (int)(poisonDef * (1 + level * mult));
            this.resRate = (int)(resRate * (1 + level * mult));
            this.attackSpeed *= (1 + level * mult);
            this.critChance *= (1 + level * mult);
            this.critMult *= (1 + level * mult);
        }
    }
}