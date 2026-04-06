using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class GroupWave
    {
        [JsonProperty] private int waveCount;
        [JsonProperty] private WaveData[] waveDatas;
        [JsonIgnore] public WaveData[] WaveDatas => waveDatas;

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
                Debug.Log("Add wave" + index);
            }
        }
    }

    [Serializable]
    public class WaveData
    {
        [JsonProperty] private int enemyCount;
        [JsonProperty] private EnemyData[] enemies;
        [JsonIgnore] public EnemyData[] Enemies => enemies;
        
        public WaveData(int maxEnemyCount)
        {
            enemyCount = maxEnemyCount;
            enemies = new EnemyData[maxEnemyCount];
        }

        public void AddEnemy(int index, EnemyData enemyData)
        {
            enemies[index] = enemyData;
            Debug.Log("Add" + enemyData.EnemyId.ToString());
        }
    }

    [Serializable]
    public class EnemyData
    {
        [JsonProperty] private EnemyId enemyId;
        [JsonProperty] private int health;
        [JsonProperty] private int stamina;
        [JsonProperty] private int phys;
        [JsonProperty] private int mag;
        [JsonProperty] private int fire;
        [JsonProperty] private int frost;
        [JsonProperty] private int lightning;
        [JsonProperty] private int holy;
        [JsonProperty] private int dark;
        [JsonProperty] private int water;
        [JsonProperty] private int poison;
        [JsonProperty] private int phyDef;
        [JsonProperty] private int magDef;
        [JsonProperty] private int fireDef;
        [JsonProperty] private int frostDef;
        [JsonProperty] private int lightningDef;
        [JsonProperty] private int holyDef;
        [JsonProperty] private int darkDef;
        [JsonProperty] private int waterDef;
        [JsonProperty] private int poisonDef;
        [JsonProperty] private int resRate;
        [JsonProperty] private float attackSpeed;
        [JsonProperty] private float critChance;
        [JsonProperty] private float critMult;

        [JsonIgnore] public EnemyId EnemyId => enemyId;
        [JsonIgnore] public int Health => health;
        [JsonIgnore] public int Stamina => stamina;
        [JsonIgnore] public int Phys => phys;
        [JsonIgnore] public int Mag => mag;
        [JsonIgnore] public int Fire => fire;
        [JsonIgnore] public int Frost => frost;
        [JsonIgnore] public int Lightning => lightning;
        [JsonIgnore] public int Holy => holy;
        [JsonIgnore] public int Dark => dark;
        [JsonIgnore] public int Water => water;
        [JsonIgnore] public int Poison => poison;
        [JsonIgnore] public int PhyDef => phyDef;
        [JsonIgnore] public int MagDef => magDef;
        [JsonIgnore] public int FireDef => fireDef;
        [JsonIgnore] public int FrostDef => frostDef;
        [JsonIgnore] public int LightningDef => lightningDef;
        [JsonIgnore] public int HolyDef => holyDef;
        [JsonIgnore] public int DarkDef => darkDef;
        [JsonIgnore] public int WaterDef => waterDef;
        [JsonIgnore] public int PoisonDef => poisonDef;
        [JsonIgnore] public int ResRate => resRate;
        [JsonIgnore] public float AttackSpeed => attackSpeed;
        [JsonIgnore] public float CritChance => critChance;
        [JsonIgnore] public float CritMult => critMult;

        public EnemyData(int level, EnemyId enemyId, int phys, int mag, int fire, int frost, int lightning, int holy, int dark, int water, int poison, int health, int stamina, int phydef, int magdef, int firedef, int frostdef, int lightningdef, int holydef, int darkdef, int waterdef, int poisonDef, int resRate, float attackSpeed, float critChance, float critMult)
        {
            this.enemyId = enemyId;
            this.health = health;
            this.stamina = stamina;
            this.phys = phys;
            this.mag = mag;
            this.fire = fire;
            this.frost = frost;
            this.lightning = lightning;
            this.holy = holy;
            this.dark = dark;
            this.water = water;
            this.poison = poison;
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
            this.health *= (int)(1 + level * mult);
            this.stamina += (int)(1 + level * mult);
            this.phys += (int)(1 + level * mult);
            this.mag += (int)((1 + level * mult));
            this.fire *= (int)((1 + level * mult));
            this.frost *= (int)((1 + level * mult));
            this.lightning *= (int)((1 + level * mult));
            this.holy *= (int)((1 + level * mult));
            this.dark *= (int)((1 + level * mult));
            this.water *= (int)((1 + level * mult));
            this.poison *= (int)((1 + level * mult));
            this.phyDef += (int)((1 + level * mult));
            this.magDef += (int)((1 + level * mult));
            this.fireDef *= (int)((1 + level * mult));
            this.frostDef *= (int)((1 + level * mult));
            this.lightningDef *= (int)((1 + level * mult));
            this.holyDef *= (int)((1 + level * mult));
            this.darkDef *= (int)((1 + level * mult));
            this.waterDef *= (int)((1 + level * mult));
            this.poisonDef *= (int)((1 + level * mult));
            this.resRate += (int)((1 + level * mult));
            this.critChance *= level * mult;
        }
    }
}