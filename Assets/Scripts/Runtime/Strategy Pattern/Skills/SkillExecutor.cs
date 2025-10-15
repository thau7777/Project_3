using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    [SerializeField] 
    private List<SkillStrategy> _skillDatas;
    [SerializeField]
    private List<Transform> _skillSpawnPoints;


    private List<SkillRuntimeInstance> _skillInstance;

    private SkillRuntimeInstance _skillToCast;
    private SkillDataForClass? _storedSkillData;
    public SkillDataForClass? StoredSkillData => _storedSkillData;
    private Flyweight _chargedSkillFlyweight;
    private Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _skillInstance = new List<SkillRuntimeInstance>();
        foreach (var s in _skillDatas)
            _skillInstance.Add(new SkillRuntimeInstance(s));
    }

    // always get that skill data first if return ok then we can cast it later
    public bool SetSkillData(int index, CharacterClass characterClass)
    {
        var skillData = _skillInstance[index].Definition.GetSkillDataByClass(characterClass);
        if (skillData == null) return false;
        _skillToCast = _skillInstance[index];
        if(_skillToCast.IsOnCooldown) return false;
        // store the data for the cast
        _storedSkillData = skillData;
        return true;
    }
    public bool UseSkill(int index, CharacterClass characterClass,PlayerContext context, Action onCastInstantly = null)
    {
        if (!SetSkillData(index, characterClass)) return false;

        bool isAimNeeded = _storedSkillData.Value.aimType != AimType.None;
        if (isAimNeeded)
        {
            // run aim anim first
            context.IsStrafing = true;
            _animator.CrossFade(_storedSkillData.Value.aimType.ToString(), 0.1f, 1);

            if (_skillToCast.Definition.CanCharge)
            {
                _chargedSkillFlyweight = FlyweightFactory.Spawn(_skillToCast.Definition.FlyweightSettings);
                _chargedSkillFlyweight.transform.position = GetSkillSpawnPoint(_storedSkillData.Value.spawnLocation);
                _chargedSkillFlyweight.transform.SetParent(transform);
            }
                
            return true;
        }
        onCastInstantly?.Invoke();
        CastSkill(context);
        return false;
    }
    public void CastSkill(PlayerContext context)// run the actual skill animation
    {
        context.IsInSpecialMove = true;
        context.NeedHoldStill = _skillToCast.Definition.NeedHoldStill;
        bool isAimNeeded = _storedSkillData.Value.aimType != AimType.None;
        string animName = _storedSkillData.Value.animName;
        _animator.CrossFade(animName, 0.1f, isAimNeeded ? 1 : 0);
    }

    // animation event
    public void OnSkillTrigger()
    {
        ExecuteSkill();
    }
    public void OnSkillEnd()
    {

    }
    private void ExecuteSkill()
    {
        if(_skillToCast == null) return;
        Vector3 spawnPoint = _chargedSkillFlyweight ? GetSkillSpawnPoint(_storedSkillData.Value.spawnLocation) : Vector3.zero;
        var ctx = new SkillStrategyContext(transform, spawnPoint, _chargedSkillFlyweight);

        _skillToCast.Cast(ctx);
        _skillToCast = null;
        _storedSkillData = null;
        _chargedSkillFlyweight = null;
    }
    private Vector3 GetSkillSpawnPoint(VFXSpawnLocation location)
    {
        Vector3 skillSpawnPoint = transform.position;
        foreach (var sp in _skillSpawnPoints)
        {
            if (sp.name == location.ToString())
            {
                skillSpawnPoint = sp.position;
                break;
            }
        }
        return skillSpawnPoint;
    }
    public void AddOrReplaceSkill(int index, SkillStrategy newSkill)
    {
        if (index < 0 || index >= _skillInstance.Count) return;
        _skillInstance[index] = new SkillRuntimeInstance(newSkill);
    }
}
