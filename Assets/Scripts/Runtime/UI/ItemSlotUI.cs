// ItemSlotUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _cooldownProgressImage;
    [SerializeField] private Image _cooldownFinishImage;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private TextMeshProUGUI _quantityText;

    private bool _finishEffectTriggered;
    private ItemRuntimeInstance _item;
    private Material _progressMaterial;
    private Material _finishMaterial;
    public void Bind(ItemRuntimeInstance item)
    {
        _progressMaterial = _cooldownProgressImage.material = new Material(_cooldownProgressImage.material);
        _finishMaterial = _cooldownFinishImage.material = new Material(_cooldownFinishImage.material);

        _item = item;
        _finishEffectTriggered = false;

        ResetCooldownVisuals();

        if (_item.Definition == null)
        {
            SetIconAlpha(0f);
            _quantityText.gameObject.SetActive(false);
            return;
        }

        _icon.sprite = _item.Definition.itemIcon;
        SetIconAlpha(1f);
        RefreshQuantityText();
    }

    public void Tick()
    {
        if (_item?.Definition == null) return;

        RefreshQuantityText();

        if (_item.IsOnCooldown)
        {
            SetIconAlpha(0.25f);

            _cooldownText.gameObject.SetActive(true);
            _cooldownText.text = Mathf.Ceil(_item.CurrentCooldownRemaining).ToString();

            _cooldownProgressImage.gameObject.SetActive(true);
            _cooldownProgressImage.material.SetFloat("_FillAmount", _item.CurrentCooldownNormalized);

            if (_item.CurrentCooldownRemaining <= 0.4f && !_finishEffectTriggered)
            {
                _finishEffectTriggered = true;
                PlayFinishEffect().Forget();
            }
            if (_item.CurrentCooldownRemaining <= 0.1f)
                _cooldownText.gameObject.SetActive(false);
        }
        else
        {
            if (_icon.color.a == 1) return;
            SetIconAlpha(1f);
        }
    }

    private void RefreshQuantityText()
    {
        bool showQuantity = _item.Definition.loseQuantityOnUse && _item.currentQuantity > 1;
        _quantityText.gameObject.SetActive(showQuantity);
        if (showQuantity)
            _quantityText.text = _item.currentQuantity.ToString();
    }

    private void ResetCooldownVisuals()
    {
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