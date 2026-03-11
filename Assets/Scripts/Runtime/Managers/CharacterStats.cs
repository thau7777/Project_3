using MyRule;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField]
    private CharacterStatsSO _statsSO;

    [field: SerializeField]
    public ElementalType ElementalType { get; set; } = ElementalType.Normal;

    [field: SerializeField]
    public int InitialHealth { get; private set; } = 100;

    [field: SerializeField]
    public int InitialMana { get; private set; } = 0;

    [field: SerializeField]
    public int AttackDamage { get; private set; } = 10;

    [field: SerializeField]
    public int MagicAttackDamage { get; private set; } = 8;

    [field: SerializeField]
    public int FireDamage { get; private set; } = 0;

    [field: SerializeField]
    public int WaterDamage { get; private set; } = 0;

    [field: SerializeField]
    public int FrostDamage { get; private set; } = 0;

    [field: SerializeField]
    public int LightningDamage { get; private set; } = 0;

    [field: SerializeField]
    public int HolyDamage { get; private set; } = 0;

    [field: SerializeField]
    public int DarkDamage { get; private set; } = 0;

    [field: SerializeField]
    public int PoisonDamage { get; private set; } = 0;

    [field: SerializeField]
    public int PhysicalDefense { get; private set; } = 5;

    [field: SerializeField]
    public int MagicDefense { get; private set; } = 5;

    [field: SerializeField]
    public int FireDefense { get; private set; } = 0;

    [field: SerializeField]
    public int WaterDefense { get; private set; } = 0;

    [field: SerializeField]
    public int FrostDefense { get; private set; } = 0;

    [field: SerializeField]
    public int LightningDefense { get; private set; } = 0;

    [field: SerializeField]
    public int HolyDefense { get; private set; } = 0;

    [field: SerializeField]
    public int DarkDefense { get; private set; } = 0;

    [field: SerializeField]
    public int PoisonDefense { get; private set; } = 0;

    [field: SerializeField]
    public float Speed { get; private set; } = 5;

    [field: SerializeField]
    public float CriticalRate { get; private set; } = 0.1f;

    [field: SerializeField]
    public float CriticalMultiplier { get; private set; } = 1.5f;

    [field: SerializeField]
    public float AttackSizeScale { get; private set; } = 1f;

    private void Awake()
    {
        if(_statsSO)
            Setup(ElementalType.Normal, _statsSO.hp, 0, _statsSO.fp, _statsSO.attackDmg,
                _statsSO.magicDmg, _statsSO.fireDmg, _statsSO.waterDmg, _statsSO.frostDmg,
                _statsSO.lightningDmg, _statsSO.holyDmg, _statsSO.darkDmg, _statsSO.poisonDmg,
                _statsSO.phyDef, _statsSO.magicDef, _statsSO.fireDef, _statsSO.waterDef,
                _statsSO.frostDef, _statsSO.lightningDef, _statsSO.holyDef, _statsSO.darkDef,
                _statsSO.poisonDef, _statsSO.speed, _statsSO.critChance, _statsSO.critMult);
    }

    public void Setup(ElementalType elementalType, int initialHealth, int stamina, int initialMana, int attackDamage,
        int magicAttackDamage, int fireDamage, int waterDamage, int frostDamage, int lightningDamage,
        int holyDamage, int darkDamage, int poisonDamage, int physicalDefense, int magicDefense,
        int fireDefense, int waterDefense, int frostDefense, int lightningDefense, int holyDefense,
        int darkDefense, int poisonDefense, float speed, float criticalRate, float criticalMultiplier)
    {
        ElementalType = elementalType;
        InitialHealth = initialHealth;
        InitialMana = initialMana;
        AttackDamage = attackDamage;
        MagicAttackDamage = magicAttackDamage;
        FireDamage = fireDamage;
        WaterDamage = waterDamage;
        FrostDamage = frostDamage;
        LightningDamage = lightningDamage;
        HolyDamage = holyDamage;
        DarkDamage = darkDamage;
        PoisonDamage = poisonDamage;
        PhysicalDefense = physicalDefense;
        MagicDefense = magicDefense;
        FireDefense = fireDefense;
        WaterDefense = waterDefense;
        FrostDefense = frostDefense;
        LightningDefense = lightningDefense;
        HolyDefense = holyDefense;
        DarkDefense = darkDefense;
        PoisonDefense = poisonDefense;
        Speed = speed;
        CriticalRate = criticalRate;
        CriticalMultiplier = criticalMultiplier;
        AttackSizeScale = 1;

        GetComponent<Damageable>().Initialize((float)initialHealth,(float)stamina);

    }

    public void ModifyAttackDamage(int amount) => AttackDamage += amount;
    public void ModifyMagicAttackDamage(int amount) => MagicAttackDamage += amount;
    public void ModifyFireDamage(int amount) => FireDamage += amount;
    public void ModifyWaterDamage(int amount) => WaterDamage += amount;
    public void ModifyFrostDamage(int amount) => FrostDamage += amount;
    public void ModifyLightningDamage(int amount) => LightningDamage += amount;
    public void ModifyHolyDamage(int amount) => HolyDamage += amount;
    public void ModifyDarkDamage(int amount) => DarkDamage += amount;
    public void ModifyPoisonDamage(int amount) => PoisonDamage += amount;

    public void ModifyPhysicalDefense(int amount) => PhysicalDefense += amount;
    public void ModifyMagicDefense(int amount) => MagicDefense += amount;
    public void ModifyFireDefense(int amount) => FireDefense += amount;
    public void ModifyWaterDefense(int amount) => WaterDefense += amount;
    public void ModifyFrostDefense(int amount) => FrostDefense += amount;
    public void ModifyLightningDefense(int amount) => LightningDefense += amount;
    public void ModifyHolyDefense(int amount) => HolyDefense += amount;
    public void ModifyDarkDefense(int amount) => DarkDefense += amount;
    public void ModifyPoisonDefense(int amount) => PoisonDefense += amount;

    public void ModifySpeed(float amount)
    {
        Speed += amount;
        if (TryGetComponent(out PlayerTopDownStateDriver playerTopDownStateDriver))
        {
            playerTopDownStateDriver.SetBaseSpeed(Speed);
        }
        else if (TryGetComponent(out EnemyTopdownStateDriver enemyTopdownStateDriver))
        {
            enemyTopdownStateDriver.SetMoveSpeed(Speed);
        }
    }

    public void ModifyCriticalRate(float amount) => CriticalRate += amount;
    public void ModifyCriticalDamage(float amount) => CriticalMultiplier += amount;
    public void ModifyAttackSizeScale(float amount) => AttackSizeScale += amount;
}