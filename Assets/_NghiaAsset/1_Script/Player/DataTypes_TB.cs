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

        public int critChance;
        public int critMult;
        public int agility;

        public CharacterStats() { }

        public CharacterStats(CharacterStatsSO statsSO)
        {
            this.maxHP = statsSO.hp;
            this.currentHP = statsSO.hp;
            this.maxMP = statsSO.fp;
            this.currentMP = statsSO.fp;
            this.maxShield = statsSO.shield;
            this.currentShield = statsSO.shield;
            this.physicalAttack = statsSO.physicalDmg;
            this.magicAttack = statsSO.magicDmg;
            this.physicalDefense = statsSO.phyDef;
            this.magicDefense = statsSO.magicDef;
            this.fireDefense = statsSO.fireDef;
            this.lightningDefense = statsSO.lightningDef;
            this.frostDefense = statsSO.frostDef;
            this.darkDefense = statsSO.darkDef;
            this.holyDefense = statsSO.holyDef;
            this.waterDefense = statsSO.waterDef;
            this.critChance = statsSO.critChance;
            this.critMult = (int) (statsSO.critMult * 100);
            this.agility = (int) statsSO.attackSpeed;

        }
    }

}