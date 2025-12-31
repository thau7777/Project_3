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
        public int physicalDefense;
        public int magicAttack;
        public int magicDefense;
        public int crit;
        public int critDamage;
        public int agility;
    }

}