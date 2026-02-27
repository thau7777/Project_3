// PlayerSkillUiController.cs
using UnityEngine;

public class SkillUiController : MonoBehaviour
{
    private EventBinding<TopDownInitializeSkillsEvent> _initializeSkillsEventBinding;

    [SerializeField] private SkillSlotUI[] _slots = new SkillSlotUI[6];

    private SkillRuntimeInstance[] _skillRuntimeInstances;

    private void OnEnable()
    {
        _initializeSkillsEventBinding = new EventBinding<TopDownInitializeSkillsEvent>(OnSkillInitialize);
        EventBus<TopDownInitializeSkillsEvent>.Register(_initializeSkillsEventBinding);
    }

    private void OnDisable()
    {
        EventBus<TopDownInitializeSkillsEvent>.Deregister(_initializeSkillsEventBinding);
    }

    private void OnSkillInitialize(TopDownInitializeSkillsEvent evt)
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
}