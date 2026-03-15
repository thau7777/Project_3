// PlayerSkillUiController.cs
using System;
using UnityEngine;

public class SkillUiController : MonoBehaviour
{

    [SerializeField] private SkillSlotUI[] _slots = new SkillSlotUI[6];

    [SerializeField, ColorUsage(true, true)] private Color _onUseColor;
    [SerializeField, ColorUsage(true, true)] private Color _onCantUseColor;



    private SkillRuntimeInstance[] _skillRuntimeInstances;
    private EventBinding<TopdownInitializeSkillsEvent> _initializeSkillsEventBinding;
    private EventBinding<TopdownSkillOnUseEvent> _skillOnUseEventBinding;
    private void OnEnable()
    {
        _initializeSkillsEventBinding = new EventBinding<TopdownInitializeSkillsEvent>(OnSkillInitialize);
        EventBus<TopdownInitializeSkillsEvent>.Register(_initializeSkillsEventBinding);

        _skillOnUseEventBinding = new(OnSkillUse);
        EventBus<TopdownSkillOnUseEvent>.Register(_skillOnUseEventBinding);
    }


    private void OnDisable()
    {
        EventBus<TopdownInitializeSkillsEvent>.Deregister(_initializeSkillsEventBinding);
        EventBus<TopdownSkillOnUseEvent>.Deregister(_skillOnUseEventBinding);
    }

    private void OnSkillInitialize(TopdownInitializeSkillsEvent evt)
    {
        _skillRuntimeInstances = evt.skillRuntimeInstances;
        for (int i = 0; i < _slots.Length; i++)
            _slots[i].Bind(_skillRuntimeInstances[i]);
    }

    private void Update()
    {
        if (_skillRuntimeInstances.IsNullOrEmpty()) return;
        foreach (var slot in _slots)
            slot.Tick();
    }
    private void OnSkillUse(TopdownSkillOnUseEvent topdownSkillOnUseEvent)
    {
        switch (topdownSkillOnUseEvent.skillOnUseState)
        {
            case SkillOnUseState.Reset:
                {
                    _slots[topdownSkillOnUseEvent.skillIndex].OnReset().Forget();
                    break;
                }
            case SkillOnUseState.Use:
                {
                    _slots[topdownSkillOnUseEvent.skillIndex].OnUse(_onUseColor).Forget();
                    break;
                }
            case SkillOnUseState.OnCooldown:
            case SkillOnUseState.NotEnoughMana:
                {
                    _slots[topdownSkillOnUseEvent.skillIndex].OnCantUse(_onCantUseColor).Forget();
                    break;
                }
            
        }
    }
}