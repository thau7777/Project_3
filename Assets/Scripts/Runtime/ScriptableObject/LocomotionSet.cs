using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public enum CharacterClass
{
    SwordAndShield,
    GreatsSword,
    PoleArm,
    Mage,
    Bow
}
[Serializable]
public struct AttackData
{
    public string animName;
    public float damage;
    public float dashForce;
}

[CreateAssetMenu(fileName = "NewLocomotionSet", menuName = "ScriptableObjects/LocomotionSet", order = 1)]
public class LocomotionSet : ScriptableObject
{
    public CharacterClass characterClass;
    public AnimatorController animationController;
    public List<AttackData> attackAnims = new();
    public bool IsNextAttackNull => _currentIndex >= attackAnims.Count - 1;
    private int _currentIndex = -1; // start before the first

    private void OnEnable()
    {
        ResetAttackAnimCycle();
    }

    /// <summary>
    /// Returns the next attack animation name in the list,
    /// or null if there are no more.
    /// </summary>
    public AttackData GetNextAttackAnim()
    {
        _currentIndex++;
        return attackAnims[_currentIndex];
    }

    public void ResetAttackAnimCycle()
    {
        _currentIndex = -1;
    }
}

