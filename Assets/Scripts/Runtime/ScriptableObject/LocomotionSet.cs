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
public enum VFXSpawnLocation
{
    Slash,
    ShieldBash,
    LeftHand,
    RightHand,
}
[Serializable]
public struct AttackData
{
    public string animName;
    public float damage;
    public FlyweightSettings flyweightSettings;
    public VFXSpawnLocation spawnLocation;
    public float dashForce;
    public bool isDashForward;
}


[CreateAssetMenu(fileName = "NewLocomotionSet", menuName = "Scriptable Objects/LocomotionSet", order = 1)]
public class LocomotionSet : ScriptableObject
{
    public CharacterClass characterClass;
    public AnimatorController animationController;
    public List<AttackData> comboAttackAnims = new();
    public AttackData FirstComboAttack 
    { get
        {
            ResetAttackAnimCycle();
            _currentIndex++;
            SetCurrentAttackData(comboAttackAnims[_currentIndex]);
            return CurrentAttackData;
        }

        private set { }
    }
    private AttackData _queuedAttackData;
    public AttackData QueuedAttackData
    {
        get
        {
            SetCurrentAttackData(_queuedAttackData);
            return CurrentAttackData;
        }

        private set { _queuedAttackData = value; }
    }
    public AttackData CurrentAttackData { get; private set; }
    public bool HasNextCombo => _currentIndex < comboAttackAnims.Count - 1;
    private int _currentIndex = -1; // start before the first

    private void OnEnable()
    {
        ResetAttackAnimCycle();
    }

    public void QueueNextComboAttack()
    {
        if (!HasNextCombo) ResetAttackAnimCycle();
        _currentIndex++;
        QueuedAttackData = comboAttackAnims[_currentIndex];
        
    }
    public void SetCurrentAttackData(AttackData data)
    {
        CurrentAttackData = data;
    }
    public void ResetAttackAnimCycle()
    {
        _currentIndex = -1;
    }
}

