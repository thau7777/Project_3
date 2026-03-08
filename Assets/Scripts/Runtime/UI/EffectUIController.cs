// EffectUIController.cs
using System.Collections.Generic;
using UnityEngine;

public class EffectUIController : MonoBehaviour
{
    [SerializeField] private GameObject _buffContainer;
    [SerializeField] private GameObject _debuffContainer;
    [SerializeField] private EffectIconSettings _effectIconSettings;

    private Dictionary<string, EffectIcon> _activeIcons = new Dictionary<string, EffectIcon>();

    private void Update()
    {
        foreach (var kvp in _activeIcons)
            kvp.Value.Tick();
    }

    private void OnDestroy()
    {
        // Return all icons to pool cleanly if this controller is destroyed mid-session
        foreach (var kvp in _activeIcons)
            kvp.Value.ReturnToPool();
        _activeIcons.Clear();
    }
    private void Start()
    {
        
    }
    // Wire to EffectsManager.OnEffectAdded
    public void OnEffectAdded(ActiveEffect activeEffect)
    {
        string key = activeEffect.effect.name;

        if (_activeIcons.TryGetValue(key, out EffectIcon existingIcon))
        {
            existingIcon.Refresh(activeEffect);
            return;
        }

        EffectIcon newIcon = FlyweightFactory.Spawn(_effectIconSettings) as EffectIcon;
        GameObject container = activeEffect.effect.effectType == EffectType.Buff
            ? _buffContainer
            : _debuffContainer;

        newIcon.Initialize(activeEffect, container);
        _activeIcons[key] = newIcon;
    }

    // Wire to EffectsManager.OnEffectRemoved
    public void OnEffectRemoved(ActiveEffect activeEffect)
    {
        string key = activeEffect.effect.name;
        if (_activeIcons.TryGetValue(key, out EffectIcon icon))
        {
            icon.ReturnToPool();
            _activeIcons.Remove(key);
        }
    }
}