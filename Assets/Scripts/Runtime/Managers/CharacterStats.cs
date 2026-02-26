using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CharacterStats : MonoBehaviour
{

    [field: SerializeField]
    public float InitialHealth { get; private set; } = 100;

    [field: SerializeField]
    public float InitialMana { get; private set; } = 100;

    [field: SerializeField]
    public float AttackDamage { get; private set; } = 10;

    [field: SerializeField]
    public float MagicAttackDamage { get; private set; } = 8;

    [field: SerializeField]
    public float PhysicalDefense { get; private set; } = 5;

    [field: SerializeField]
    public float MagicDefense { get; private set; } = 5;

    [field: SerializeField]
    public float Agility { get; private set; } = 5;

    [field: SerializeField]
    public float CriticalRate { get; private set; } = 0.1f;

    [field: SerializeField]
    public float CriticalDamage { get; private set; } = 1.5f;

    [field: SerializeField]
    public float AttackSizeScale { get; private set; } = 1f;
    private void Awake()
    {
        
    }

    public void ModifyAttackDamage(float amount)
    {
        AttackDamage += amount;
    }

    public void ModifyMagicAttackDamage(float amount)
    {
        MagicAttackDamage += amount;
    }

    public void ModifyPhysicalDefense(float amount)
    {
        PhysicalDefense += amount;
    }

    public void ModifyMagicDefense(float amount)
    {
        MagicDefense += amount;
    }

    public void ModifyAgility(float amount)
    {
        Agility += amount;
        if (TryGetComponent(out PlayerTopDownStateDriver playerTopDownStateDriver))
        {
            playerTopDownStateDriver.SetBaseSpeed(Agility);
        }
        else if (TryGetComponent(out EnemyTopdownStateDriver enemyTopdownStateDriver))
        {
            enemyTopdownStateDriver.SetMoveSpeed(Agility);
        }
    }

    public void ModifyCriticalRate(float amount)
    {
        CriticalRate += amount;
    }

    public void ModifyCriticalDamage(float amount)
    {
        CriticalDamage += amount;
    }

    public void ModifyAttackSizeScale(float amount)
    {
        AttackSizeScale += amount;
    }
}
