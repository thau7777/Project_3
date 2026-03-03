// SkillSlotUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _cooldownProgressImage;
    [SerializeField] private Image _cooldownFinishImage;
    [SerializeField] private TextMeshProUGUI _cooldownText;

    private bool _finishEffectTriggered;
    private SkillRuntimeInstance _skill;
    private Material _progressMaterial;
    private Material _finishMaterial;
    public void Bind(SkillRuntimeInstance skill)
    {
        // get unique material instances per slot
        _progressMaterial = _cooldownProgressImage.material = new Material(_cooldownProgressImage.material);
        _finishMaterial = _cooldownFinishImage.material = new Material(_cooldownFinishImage.material);

        _skill = skill;
        _finishEffectTriggered = false;

        ResetCooldownVisuals();

        if (_skill.Definition == null)
        {
            SetIconAlpha(0f);
            return;
        }

        _icon.sprite = _skill.Definition.skillIcon;
        SetIconAlpha(1f);
    }

    public void Tick()
    {
        if (_skill?.Definition == null) return;

        if (_skill.IsOnCooldown)
        {
            SetIconAlpha(0.25f);

            _cooldownText.gameObject.SetActive(true);
            _cooldownText.text = Mathf.Ceil(_skill.CurrentCooldownRemaining).ToString();

            _cooldownProgressImage.gameObject.SetActive(true);
            _cooldownProgressImage.material.SetFloat("_FillAmount", _skill.CurrentCooldownNormalized);

            if (_skill.CurrentCooldownRemaining <= 0.4f && !_finishEffectTriggered)
            {
                _finishEffectTriggered = true;
                PlayFinishEffect().Forget();
            }
            if(_skill.CurrentCooldownRemaining <= 0.1f)
                _cooldownText.gameObject.SetActive(false);
        }
        else
        {
            if (_icon.color.a == 1) return;
            SetIconAlpha(1f);
        }
    }

    private void ResetCooldownVisuals()
    {
        if(!gameObject.activeSelf) return; // avoid resetting visuals when slot is disabled (e.g. no skill assigned)
        _cooldownText.gameObject.SetActive(false);
        _cooldownProgressImage.gameObject.SetActive(false);
        _cooldownProgressImage.material.SetFloat("_FillAmount", 0f);
        _cooldownFinishImage.gameObject.SetActive(false);
        _cooldownFinishImage.material.SetFloat("_FillAmount", 0f);
        _finishEffectTriggered = false;
    }

    private void SetIconAlpha(float alpha)
    {
        Color c = _icon.color;
        c.a = alpha;
        _icon.color = c;
    }

    private async UniTaskVoid PlayFinishEffect()
    {
        _cooldownFinishImage.gameObject.SetActive(true);
        _cooldownFinishImage.material.SetFloat("_FillAmount", 0f);

        float duration = 0.6f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _cooldownFinishImage.material.SetFloat("_FillAmount", Mathf.Clamp01(elapsed / duration));
            await UniTask.Yield();
        }

        _cooldownFinishImage.material.SetFloat("_FillAmount", 1f);
        ResetCooldownVisuals();
    }
    private void OnDestroy()
    {
        Destroy(_progressMaterial);
        Destroy(_finishMaterial);
    }
}