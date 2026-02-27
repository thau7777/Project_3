// EffectIcon.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectIcon : Flyweight
{
    new EffectIconSettings settings => (EffectIconSettings)base.settings;

    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _stackText;
    [SerializeField] private Image _durationImage;

    private ActiveEffect _activeEffect;

    private void OnEnable()
    {
        // Reset fill on every spawn from pool - icon sprite is handled by Initialize
        _durationImage.fillAmount = 0f;
    }

    private void OnDisable()
    {
        // Clear reference when returned to pool so stale Tick() calls are harmless
        _activeEffect = null;
    }

    // Called once when the icon is first spawned from pool
    public void Initialize(ActiveEffect activeEffect, GameObject parent)
    {
        transform.SetParent(parent.transform, false);
        Refresh(activeEffect);
    }

    // Called on first spawn AND whenever the effect updates (stacks, re-apply, etc.)
    public void Refresh(ActiveEffect activeEffect)
    {
        _activeEffect = activeEffect;

        // Always enforce the icon from the ScriptableObject - never stale, never missing
        _iconImage.sprite = activeEffect.effect.icon;
        _iconImage.gameObject.SetActive(true);

        RedrawState();
    }

    // Called every frame by EffectUIController
    public void Tick()
    {
        if (_activeEffect == null) return;
        RedrawState();
    }

    private void RedrawState()
    {
        if (!_activeEffect.IsApplied)
        {
            // --- Stacking phase: icon + stack count only, no duration UI ---
            _stackText.text = _activeEffect.currentStacks.ToString();
            _stackText.gameObject.SetActive(true);
            _durationImage.fillAmount = 0f;
            _iconImage.color = new Color(1f, 1f, 1f, 0.4f);
        }
        else
        {
            // --- Applied phase: icon + duration UI only, no stack count ---
            _stackText.gameObject.SetActive(false);
            _iconImage.color = Color.white;
            float remaining = _activeEffect.remainingTime;
            float max = _activeEffect.maxDuration;
            if (max > 0f)
            {
                _durationImage.fillAmount = 1f - (remaining / max);
            }
            if (remaining <= 0f)
                _durationImage.fillAmount = 1f;
        }
    }
}