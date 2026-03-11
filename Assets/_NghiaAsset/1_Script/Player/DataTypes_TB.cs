using MyRule;
using UnityEngine;

namespace Turnbase
{
    public enum BattleState
    {
        Waiting,
        Ready,
        Attacking,
        TakingDamage,
        Dead,
        Parrying,
        Interrupted
    }



    [System.Serializable]
    public enum CharacterClass
    {
        Sword_Shield,
        Magical,
        Summon,
        Tank,
        Enemy,


    }

    [System.Serializable]
    public enum CharacterElement
    {
        None,
        Physical,
        Magical,
        Fire,
        Water,
        Ice,
        Poison,
        Lightning,
        Dark,
        Frost,
        Holy,
        Normal,

    }

    [System.Serializable]
    public class StackData
    {
        public string stackId;
        public int currentStacks;
        public Sprite icon;
    }

    [System.Serializable]
    public class CharacterInfo
    {
        public string name;
        public Sprite Avatar;
        public int level;
    }


    [System.Serializable]
    public class CharacterStats
    {
        public int maxHP;
        public int currentHP;
        public int maxMP;
        public int currentMP;
        public int maxShield;
        public int currentShield;
        
        public int physicalAttack;
        public int magicAttack;
        
        public int physicalDefense;
        public int magicDefense;


        public int fireDefense;
        public int lightningDefense;
        public int frostDefense;
        public int darkDefense;
        public int holyDefense;
        public int waterDefense;
        public int poisonDefense;

        public int fireDamageBonus;
        public int lightningDamageBonus;
        public int frostDamageBonus;
        public int darkDamageBonus;
        public int holyDamageBonus;
        public int waterDamageBonus;
        public int poisonDamageBonus;



        public int critChance;
        public int critMult;
        public int speed;

        public CharacterStats() { }

        public CharacterStats(CharacterStatsData statsData)
        {
            this.maxHP = statsData.BaseStatsData.MaxHealth;
            this.currentHP = statsData.BaseStatsData.CurrentHealth;
            this.maxMP = statsData.BaseStatsData.MaxMana;
            this.currentMP = statsData.BaseStatsData.CurrentMana;
            //this.maxShield = statsSO.shield;
            //this.currentShield = statsSO.shield;
            this.physicalAttack = statsData.Damage.PhysDmg;
            this.magicAttack = statsData.Damage.MagDmg;
            this.physicalDefense = statsData.Defense.PhysDef;
            this.magicDefense = statsData.Defense.MagDef;


            this.fireDefense = statsData.Defense.FireDef;
            this.lightningDefense = statsData.Defense.LightningDef;
            this.frostDefense = statsData.Defense.FrostDef;
            this.darkDefense = statsData.Defense.DarkDef;
            this.holyDefense = statsData.Defense.HolyDef;
            this.waterDefense = statsData.Defense.WaterDef;
            this.poisonDefense = statsData.Defense.PoisonDef;

            this.fireDamageBonus = statsData.Damage.FireDmg;
            this.lightningDamageBonus = statsData.Damage.LightningDmg;
            this.frostDamageBonus = statsData.Damage.FrostDmg;
            this.darkDamageBonus = statsData.Damage.DarkDmg;
            this.holyDamageBonus = statsData.Damage.HolyDmg;
            this.waterDamageBonus = statsData.Damage.WaterDmg;
            this.poisonDamageBonus = statsData.Damage.PoisonDmg;

            this.speed = (int) statsData.BaseStatsData.Speed;

            this.critChance = statsData.BaseStatsData.CritChance;
            this.critMult = (int) (statsData.BaseStatsData.CritMult * 100);
        }
    }

}