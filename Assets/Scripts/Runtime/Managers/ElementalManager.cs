using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ElementalType
{
    Fire,
    Water,
    Frost,
    Holy,
    Dark,
    Lightning,
    Poison,
    Normal
}

[System.Serializable]
public class DamageCalculationEvent : UnityEvent<float, ElementalType, ElementalType, float> { }

public class ElementalManager : Singleton<ElementalManager>
{

    [Header("Damage Multipliers")]
    [SerializeField] private float strongMultiplier = 1.5f;
    [SerializeField] private float weakMultiplier = 0.5f;
    [SerializeField] private float neutralMultiplier = 1.0f;


    private Dictionary<ElementalType, List<ElementalType>> _strongAgainst;
    private Dictionary<ElementalType, List<ElementalType>> _weakAgainst;

    protected override void Awake()
    {
        base.Awake();
        InitializeElementalRelationships();
    }

    private void InitializeElementalRelationships()
    {
        _strongAgainst = new Dictionary<ElementalType, List<ElementalType>>();
        _weakAgainst = new Dictionary<ElementalType, List<ElementalType>>();

        foreach (ElementalType element in System.Enum.GetValues(typeof(ElementalType)))
        {
            _strongAgainst[element] = new List<ElementalType>();
            _weakAgainst[element] = new List<ElementalType>();
        }

        AddRelationship(ElementalType.Water, ElementalType.Fire);
        AddRelationship(ElementalType.Fire, ElementalType.Frost);
        AddRelationship(ElementalType.Frost, ElementalType.Water);
        AddRelationship(ElementalType.Water, ElementalType.Poison);
        AddRelationship(ElementalType.Poison, ElementalType.Lightning);
        AddRelationship(ElementalType.Lightning, ElementalType.Water);
        AddRelationship(ElementalType.Lightning, ElementalType.Fire);
        AddRelationship(ElementalType.Holy, ElementalType.Dark);
        AddRelationship(ElementalType.Dark, ElementalType.Holy);
    }

    private void AddRelationship(ElementalType strongElement, ElementalType weakElement)
    {
        _strongAgainst[strongElement].Add(weakElement);
        _weakAgainst[weakElement].Add(strongElement);
    }

    /// <summary>
    /// Calculates the damage multiplier when an attacker element hits a defender element.
    /// Returns strongMultiplier if attacker is strong against defender.
    /// Returns weakMultiplier if attacker is weak against defender.
    /// Returns neutralMultiplier for neutral matchups or Normal element.
    /// </summary>
    public float GetDamageMultiplier(ElementalType attackerElement, ElementalType defenderElement)
    {
        if (attackerElement == ElementalType.Normal && defenderElement != ElementalType.Normal)
        {
            return neutralMultiplier;
        }
        if (attackerElement != ElementalType.Normal && defenderElement == ElementalType.Normal)
        {
            return weakMultiplier;
        }
        if (attackerElement == ElementalType.Normal && defenderElement == ElementalType.Normal)
        {
            return strongMultiplier;
        }

        if (attackerElement == defenderElement)
        {
            return neutralMultiplier;
        }

        if (_strongAgainst[attackerElement].Contains(defenderElement))
        {
            return strongMultiplier;
        }

        if (_weakAgainst[attackerElement].Contains(defenderElement))
        {
            return weakMultiplier;
        }

        return neutralMultiplier;
    }
    /// <summary>
    /// Checks if the attacker element is strong against the defender element.
    /// </summary>
    public bool IsStrongAgainst(ElementalType attacker, ElementalType defender)
    {
        return _strongAgainst.ContainsKey(attacker) && _strongAgainst[attacker].Contains(defender);
    }

    /// <summary>
    /// Checks if the attacker element is weak against the defender element.
    /// </summary>
    public bool IsWeakAgainst(ElementalType attacker, ElementalType defender)
    {
        return _weakAgainst.ContainsKey(attacker) && _weakAgainst[attacker].Contains(defender);
    }
    /// <summary>
    /// Calculates final damage after applying elemental multiplier and additional buff multipliers.
    /// Invokes OnDamageCalculated event with the results.
    /// </summary>
    public float CalculateDamage(float baseDamage, ElementalType attackerElement, ElementalType defenderElement)
    {
        float elementalMultiplier = GetDamageMultiplier(attackerElement, defenderElement);
        float finalDamage = baseDamage * elementalMultiplier;

        return finalDamage;
    }

}