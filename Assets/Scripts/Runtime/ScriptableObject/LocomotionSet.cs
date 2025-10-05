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
    public bool isDashForward;
}


[CreateAssetMenu(fileName = "NewLocomotionSet", menuName = "ScriptableObjects/LocomotionSet", order = 1)]
public class LocomotionSet : ScriptableObject
{
    public CharacterClass characterClass;
    public AnimatorController animationController;
    public List<AttackData> comboAttackAnims = new();
    public List<AttackData> SpecialActions ;
    public AttackData currentAnimData;
    public bool IsNextComboAttackNull => _currentIndex >= comboAttackAnims.Count - 1;
    private int _currentIndex = -1; // start before the first
    public int CurrentIndex => _currentIndex;

    private void OnEnable()
    {
        ResetAttackAnimCycle();
    }

    /// <summary>
    /// Returns the next attack animation name in the list,
    /// or null if there are no more.
    /// </summary>
    public AttackData GetNextAttackComboAnim()
    {
        _currentIndex++;
        currentAnimData = comboAttackAnims[_currentIndex];
        return comboAttackAnims[_currentIndex];
    }

    public void ResetAttackAnimCycle()
    {
        _currentIndex = -1;
    }
    public void FindAnimDataByName(string animName)
    {

    }
    /// <summary>
    /// use for special actions only
    /// </summary>
    /// <param name="anim"></param>
    public void SetCurrentAnimDataByName(string animName)
    {
        foreach(var anim in SpecialActions)
        {
            if (anim.animName == animName)
            {
                currentAnimData = anim;
                return;
            }
        }
    }
}

