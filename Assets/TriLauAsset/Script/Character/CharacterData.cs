using Newtonsoft.Json;
using System;

namespace MyRule
{
    [Serializable]
    public class CharacterData
    {
        [JsonProperty] public string _name;
        [JsonProperty] public string _backStory;
        [JsonProperty] private EClass _characterClass;
        [JsonProperty] private CharacterStatsData _characterStatsData;

        [JsonIgnore] public string Name => _name;
        [JsonIgnore] public string BackStory => _backStory;
        [JsonIgnore] public EClass CharacterClass => _characterClass;
        [JsonIgnore] public CharacterStatsData CharacterStatsData => _characterStatsData;

        public CharacterData() 
        {
            _name = "Mage";
            _backStory = "...";
            _characterClass = EClass.Mage;
            _characterStatsData = new CharacterStatsData();
        }

        public CharacterData(string name, string backStory, EClass characterClass, CharacterStatsData characterStatsData)
        {
            _name = name;
            _backStory = backStory;
            _characterClass = characterClass;
            _characterStatsData = characterStatsData;
        }
    }

    [Serializable]
    public class CharacterStatsData
    {
        [JsonProperty] private AttributesData _attributesData;
        [JsonProperty] private BaseStatsData _BaseData;
        [JsonProperty] private DamageData _damage;
        [JsonProperty] private DefenseData _defense;

        [JsonIgnore] public AttributesData AttributesData => _attributesData;
        [JsonIgnore] public BaseStatsData BaseStatsData => _BaseData;
        [JsonIgnore] public DamageData Damage => _damage;
        [JsonIgnore] public DefenseData Defense => _defense;

        public CharacterStatsData()
        {
            _attributesData = new AttributesData();
            _BaseData = new BaseStatsData();
            _damage = new DamageData();
            _defense = new DefenseData();
        }

        public CharacterStatsData(AttributesData attributesData, BaseStatsData baseData, DamageData damage, DefenseData defense)
        {
            _attributesData = attributesData;
            _BaseData = baseData;
            _damage = damage;
            _defense = defense;
        }
    }

    [Serializable]
    public class AttributesData
    {
        [JsonProperty] private int _vigor;
        [JsonProperty] private int _mind;
        [JsonProperty] private int _endurance;
        [JsonProperty] private int _strength;
        [JsonProperty] private int _dexterity;
        [JsonProperty] private int _intelligence;
        [JsonProperty] private int _faith;
        [JsonProperty] private int _arcane;

        [JsonIgnore] public int Vigor => _vigor;
        [JsonIgnore] public int Mind => _mind;
        [JsonIgnore] public int Endurance => _endurance;
        [JsonIgnore] public int Strength => _strength;
        [JsonIgnore] public int Dexterity => _dexterity;
        [JsonIgnore] public int Intelligence => _intelligence;
        [JsonIgnore] public int Faith => _faith;
        [JsonIgnore] public int Arcane => _arcane;

        public AttributesData() 
        {
            _vigor = 0;
            _mind = 0;
            _endurance = 0;
            _strength = 0;
            _dexterity = 0;
            _intelligence = 0;
            _faith = 0;
            _arcane = 0;
        }

        public AttributesData(int vigor, int mind, int endurance, int strength, int dexterity, int intelligence, int faith, int arcane)
        {
            _vigor = vigor;
            _mind = mind;
            _endurance = endurance;
            _strength = strength;
            _dexterity = dexterity;
            _intelligence = intelligence;
            _faith = faith;
            _arcane = arcane;
        }
    }

    [Serializable] 
    public class BaseStatsData
    {
        [JsonProperty] private int _currentHealth;
        [JsonProperty] private int _maxHealth;
        [JsonProperty] private int _currentMana;
        [JsonProperty] private int _maxMana;
        [JsonProperty] private int _currentStamina;
        [JsonProperty] private int _maxStamina;
        [JsonProperty] private float _speed;
        [JsonProperty] private int _critChance;
        [JsonProperty] private float _critMult;

        [JsonIgnore] public int CurrentHealth => _currentHealth;
        [JsonIgnore] public int MaxHealth => _maxHealth;
        [JsonIgnore] public int CurrentMana => _currentMana;
        [JsonIgnore] public int MaxMana => _maxMana;
        [JsonIgnore] public int CurrentStamina => _currentStamina;
        [JsonIgnore] public int MaxStamina => _maxStamina;
        [JsonIgnore] public float Speed => _speed;
        [JsonIgnore] public int CritChance => _critChance;
        [JsonIgnore] public float CritMult => _critMult;

        public BaseStatsData() 
        {
            _currentHealth = 0;
            _maxHealth = 0;
            _currentMana = 0;
            _maxMana = 0;
            _currentStamina = 0;
            _maxStamina = 0;
            _speed = 0;
            _critChance = 0;
            _critMult = 0;
        }

        public BaseStatsData(int health, int mana, int stamina, float speed, int critChance, float critMult)
        {
            _currentHealth = health;
            _maxHealth = health;
            _currentMana = mana;
            _maxMana = mana;
            _currentStamina = stamina;
            _maxStamina = stamina;
            _speed = speed;
            _critChance = critChance;
            _critMult = critMult;
        }
        
        public float GetHealthRate() => _currentHealth / _maxHealth;
    }

    [Serializable]
    public class DamageData
    {
        [JsonProperty] private int _physDmg;
        [JsonProperty] private int _magDmg;
        [JsonProperty] private int _fireDmg;
        [JsonProperty] private int _lightningDmg;
        [JsonProperty] private int _holyDmg;
        [JsonProperty] private int _darkDmg;
        [JsonProperty] private int _frostDmg;
        [JsonProperty] private int _waterDmg;
        [JsonProperty] private int _poisonDmg;

        [JsonIgnore] public int PhysDmg => _physDmg;
        [JsonIgnore] public int MagDmg => _magDmg;
        [JsonIgnore] public int FireDmg => _fireDmg;
        [JsonIgnore] public int LightningDmg => _lightningDmg;
        [JsonIgnore] public int HolyDmg => _holyDmg;
        [JsonIgnore] public int DarkDmg => _darkDmg;
        [JsonIgnore] public int FrostDmg => _frostDmg;
        [JsonIgnore] public int WaterDmg => _waterDmg;
        [JsonIgnore] public int PoisonDmg => _poisonDmg;

        public DamageData() 
        {
            _physDmg = 0;
            _magDmg = 0;
            _fireDmg = 0;
            _lightningDmg = 0;
            _holyDmg = 0;
            _darkDmg = 0;
            _frostDmg = 0;
            _waterDmg = 0;
            _poisonDmg = 0;
        }

        public DamageData(int physDmg, int magDmg, int fireDmg, int lightningDmg, int holyDmg, int darkenDmg, int frostDmg, int waterDmg, int poisonDmg)
        {
            this._physDmg = physDmg;
            this._magDmg = magDmg;
            this._fireDmg = fireDmg;
            this._lightningDmg = lightningDmg;
            this._holyDmg = holyDmg;
            this._darkDmg = darkenDmg;
            this._frostDmg = frostDmg;
            this._waterDmg = waterDmg;
            this._poisonDmg = poisonDmg;
        }
    }

    [Serializable]
    public class DefenseData
    {
        [JsonProperty] private int _physDef;
        [JsonProperty] private int _magDef;
        [JsonProperty] private int _fireDef;
        [JsonProperty] private int _lightningDef;
        [JsonProperty] private int _holyDef;
        [JsonProperty] private int _darkDef;
        [JsonProperty] private int _frostDef;
        [JsonProperty] private int _waterDef;
        [JsonProperty] private int _poisonDef;

        [JsonIgnore] public int PhysDef => _physDef;
        [JsonIgnore] public int MagDef => _magDef;
        [JsonIgnore] public int FireDef => _fireDef;
        [JsonIgnore] public int LightningDef => _lightningDef;
        [JsonIgnore] public int HolyDef => _holyDef;
        [JsonIgnore] public int DarkDef => _darkDef;
        [JsonIgnore] public int FrostDef => _frostDef;
        [JsonIgnore] public int WaterDef => _waterDef;
        [JsonIgnore] public int PoisonDef => _poisonDef;

        public DefenseData() 
        {
            _physDef = 0;
            _magDef = 0;
            _fireDef = 0;
            _lightningDef = 0;
            _holyDef = 0;
            _darkDef = 0;
            _frostDef = 0;
            _waterDef = 0;
            _poisonDef = 0;
        }

        public DefenseData(int physDef, int magDef, int fireDef, int lightningDef, int holyDef, int darkDef, int frostDef, int waterDef, int poisonDef)
        {
            _physDef = physDef;
            _magDef = magDef;
            _fireDef = fireDef;
            _lightningDef = lightningDef;
            _holyDef = holyDef;
            _darkDef = darkDef;
            _frostDef = frostDef;
            _waterDef = waterDef;
            _poisonDef = poisonDef;
        }
    }
}