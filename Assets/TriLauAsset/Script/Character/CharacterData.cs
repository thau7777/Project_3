using Newtonsoft.Json;
using System;

namespace MyRule
{
    [Serializable]
    public class CharacterData
    {
        [JsonProperty] public string _name;
        [JsonProperty] private EClass _characterClass;
        [JsonProperty] private CharacterStatsData _characterStatsData;

        [JsonIgnore] public string Name => _name;
        [JsonIgnore] public EClass CharacterClass => _characterClass;
        [JsonIgnore] public CharacterStatsData CharacterStatsData => _characterStatsData;

        public CharacterData() 
        {
            _name = "Mage";
            _characterClass = EClass.Mage;
            _characterStatsData = new CharacterStatsData();
        }

        public CharacterData(string name, string backStory, EClass characterClass, CharacterStatsData characterStatsData)
        {
            _name = name;
            _characterClass = characterClass;
            _characterStatsData = characterStatsData;
        }
    }

    [Serializable]
    public class CharacterStatsData
    {
        [JsonProperty] private AttributesData _attributesData;
        [JsonProperty] private BaseStatsData _baseData;
        [JsonProperty] private DamageData _damage;
        [JsonProperty] private DefenseData _defense;

        [JsonIgnore] public AttributesData AttributesData => _attributesData;
        [JsonIgnore] public BaseStatsData BaseStatsData => _baseData;
        [JsonIgnore] public DamageData Damage => _damage;
        [JsonIgnore] public DefenseData Defense => _defense;

        public CharacterStatsData()
        {
            _attributesData = new AttributesData();
            _baseData = new BaseStatsData();
            _damage = new DamageData();
            _defense = new DefenseData();
        }

        public CharacterStatsData(AttributesData attributesData, BaseStatsData baseData, DamageData damage, DefenseData defense)
        {
            _attributesData = attributesData;
            _baseData = baseData;
            _damage = damage;
            _defense = defense;
        }

        public void AdjustStats(SigilSO sigilSO)
        {
            _attributesData.IncreaseVigor(sigilSO.vigor);
            int vigor = GetBonus(_attributesData.Vigor);
            _baseData.IncreaseMaxHealth(sigilSO.health);

            _attributesData.IncreaseMind(sigilSO.mind);
            int mind = GetBonus(_attributesData.Mind);
            _baseData.IncreaseMaxMana(sigilSO.mana);

            _attributesData.IncreaseEndurance(sigilSO.endurance);
            int endurance = GetBonus(_attributesData.Endurance);

            _attributesData.IncreaseStrength(sigilSO.strength);
            int strength = GetBonus(_attributesData.Strength);
            _damage.IncreasePhysDmg(sigilSO.phys, strength);

            _attributesData.IncreaseDexterity(sigilSO.dexterity);
            int dexterity = GetBonus(_attributesData.Dexterity);
            _baseData.SetCritMult(sigilSO.critMult);
            _baseData.IncreaseCritChance(sigilSO.critChance);

            _attributesData.IncreaseIntelligence(sigilSO.intelligence);
            int intelligence = GetBonus(_attributesData.Intelligence);
            _damage.IncreaseMagDmg(sigilSO.mag, intelligence);

            _attributesData.IncreaseFaith(sigilSO.faith);
            int faith = GetBonus(_attributesData.Faith);
            _damage.IncreaseFireDmg(sigilSO.fire, faith);
            _damage.IncreaseLightningDmg(sigilSO.lightning, faith);
            _damage.IncreaseFrostDmg(sigilSO.frost, faith);
            _damage.IncreaseWaterDmg(sigilSO.water, faith);
            _damage.IncreasePoisonDmg(sigilSO.poison, faith);
            _damage.IncreaseDarkDmg(sigilSO.dark, faith);
            _damage.IncreaseHolyDmg(sigilSO.holy, faith);

            _attributesData.IncreaseArcane(sigilSO.arcane);
            int arcane = GetBonus(_attributesData.Arcane);
            _defense.IncreasePhysDef(sigilSO.phyDef, arcane);
            _defense.IncreaseMagDef(sigilSO.magicDef, arcane);
            _defense.IncreaseFireDef(sigilSO.fireDef, arcane);
            _defense.IncreaseLightningDef(sigilSO.lightningDef, arcane);
            _defense.IncreaseFrostDef(sigilSO.frostDef, arcane);
            _defense.IncreaseMagDef(sigilSO.waterDef, arcane);
            _defense.IncreasePoisonDef(sigilSO.poisonDef, arcane);
            _defense.IncreaseDarkDef(sigilSO.darkDef, arcane);
            _defense.IncreaseHolyDef(sigilSO.holyDef, arcane);
        }

        private int GetBonus(int stat)
        {
            if (stat >= 0)
                return (int)(1f + (stat / 100f));
            else
                return (int)(1f - (stat / 100f));
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

        public void IncreaseVigor(int amount) => _vigor += amount;
        public void IncreaseMind(int amount) => _mind += amount;
        public void IncreaseEndurance(int amount) => _endurance += amount;
        public void IncreaseStrength(int amount) => _strength += amount;
        public void IncreaseDexterity(int amount) => _dexterity += amount;
        public void IncreaseIntelligence(int amount) => _intelligence += amount; 
        public void IncreaseFaith(int amount) => _faith += amount;
        public void IncreaseArcane(int amount) => _arcane += amount; 
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
        
        public float GetHealthRate() => (float)_currentHealth / (float)_maxHealth;
        public void SetCurrentHealth(int amount)
            { _currentHealth = amount; }

        public void IncreaseHealth(int amount)
        {
            _currentHealth += amount;

            if (_currentHealth > _maxHealth)
                _currentHealth = MaxHealth;
        }

        public void IncreaseMaxHealth(int amount) => _maxHealth += amount;
        public void IncreaseMaxMana(int amount) => _maxMana += amount;
        public void IncreaseMaxStamina(int amount) => _maxStamina += amount;
        public void IncreaseSpeed(int amount) => _speed += amount;
        public void IncreaseCritChance(int amount) => _critChance += amount;
        public void SetCritMult(float amount)
        {
            if (amount >= _critMult)
                _critMult = amount;
            else
                return;
        }
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

        public void IncreasePhysDmg(int amount, int bonus)
        {
            _physDmg += amount;
            _physDmg *= bonus;
        }

        public void IncreaseMagDmg(int amount, int bonus)
        {
            _magDmg += amount;
            _magDmg *= bonus;
        }
        public void IncreaseFireDmg(int amount, int bonus)
        {
            _fireDmg += amount;
            _fireDmg *= bonus;
        }
        public void IncreaseLightningDmg(int amount, int bonus)
        { 
            _lightningDmg += amount;
            _lightningDmg *= bonus;
        }
        public void IncreaseHolyDmg(int amount, int bonus)
        {
            _holyDmg += amount;
            _holyDmg *= bonus;
        }
        public void IncreaseDarkDmg(int amount, int bonus)
        {
            _darkDmg += amount;
            _darkDmg *= bonus;
        }
        public void IncreaseFrostDmg(int amount, int bonus)
        {
            _frostDmg += amount;
            _frostDmg *= bonus;
        }
        public void IncreaseWaterDmg(int amount, int bonus)
        {
            _waterDmg += amount;
            _waterDmg *= bonus;
        }
        public void IncreasePoisonDmg(int amount, int bonus)
        {
            _poisonDmg += amount;
            _poisonDmg *= bonus;
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

        public void IncreasePhysDef(int amount, int bonus)
        {
            _physDef += amount;
            _physDef *= bonus;
        }

        public void IncreaseMagDef(int amount, int bonus)
        {
            _magDef += amount;
            _magDef *= bonus;
        }
        public void IncreaseFireDef(int amount, int bonus)
        {
            _fireDef += amount;
            _fireDef *= bonus;
        }
        public void IncreaseLightningDef(int amount, int bonus)
        {
            _lightningDef += amount;
            _lightningDef *= bonus;
        }
        public void IncreaseHolyDef(int amount, int bonus)
        {
            _holyDef += amount;
            _holyDef *= bonus;
        }
        public void IncreaseDarkDef(int amount, int bonus)
        {
            _darkDef += amount;
            _darkDef *= bonus;
        }
        public void IncreaseFrostDef(int amount, int bonus)
        {
            _frostDef += amount;
            _frostDef *= bonus;
        }
        public void IncreaseWaterDef(int amount, int bonus)
        {
            _waterDef += amount;
            _waterDef *= bonus;
        }
        public void IncreasePoisonDef(int amount, int bonus)
        {
            _poisonDef += amount;
            _poisonDef *= bonus;
        }
    }
}