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

        public CharacterStats(CharacterData statsData)
        {
            this.maxHP = statsData.CharacterStatsData.BaseStatsData.MaxHealth;
            this.currentHP = statsData.CharacterStatsData.BaseStatsData.CurrentHealth;
            this.maxMP = statsData.CharacterStatsData.BaseStatsData.MaxMana;
            this.currentMP = statsData.CharacterStatsData.BaseStatsData.CurrentMana;
            //this.maxShield = statsSO.shield;
            //this.currentShield = statsSO.shield;
            this.physicalAttack = statsData.CharacterStatsData.Damage.PhysDmg;
            this.magicAttack = statsData.CharacterStatsData.Damage.MagDmg;
            this.physicalDefense = statsData.CharacterStatsData.Defense.PhysDef;
            this.magicDefense = statsData.CharacterStatsData.Defense.MagDef;


            this.fireDefense = statsData.CharacterStatsData.Defense.FireDef;
            this.lightningDefense = statsData.CharacterStatsData.Defense.LightningDef;
            this.frostDefense = statsData.CharacterStatsData.Defense.FrostDef;
            this.darkDefense = statsData.CharacterStatsData.Defense.DarkDef;
            this.holyDefense = statsData.CharacterStatsData.Defense.HolyDef;
            this.waterDefense = statsData.CharacterStatsData.Defense.WaterDef;
            this.poisonDefense = statsData.CharacterStatsData.Defense.PoisonDef;

            this.fireDamageBonus = statsData.CharacterStatsData.Damage.FireDmg;
            this.lightningDamageBonus = statsData.CharacterStatsData.Damage.LightningDmg;
            this.frostDamageBonus = statsData.CharacterStatsData.Damage.FrostDmg;
            this.darkDamageBonus = statsData.CharacterStatsData.Damage.DarkDmg;
            this.holyDamageBonus = statsData.CharacterStatsData.Damage.HolyDmg;
            this.waterDamageBonus = statsData.CharacterStatsData.Damage.WaterDmg;
            this.poisonDamageBonus = statsData.CharacterStatsData.Damage.PoisonDmg;

            this.speed = (int) statsData.CharacterStatsData.BaseStatsData.Speed;

            this.critChance = statsData.CharacterStatsData.BaseStatsData.CritChance;
            this.critMult = (int) (statsData.CharacterStatsData.BaseStatsData.CritMult * 100);
        }
    }

}