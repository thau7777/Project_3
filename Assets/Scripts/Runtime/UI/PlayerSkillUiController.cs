using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class PlayerSkillUiController : MonoBehaviour
{
    private EventBinding<TopDownInitializeSkillsEvent> initializeSkillsEventBinding;

    [SerializeField]
    private Image[] _skillIcons = new Image[6];
    private TextMeshProUGUI[] _cooldownTexts = new TextMeshProUGUI[6];
    private Image[] _coolDownProgressImage = new Image[6];
    private Image[] _coolDownFinishImage = new Image[6];
    private bool[] _finishEffectTriggered = new bool[6];

    private SkillRuntimeInstance[] _skillRuntimeInstances;
    private void OnEnable()
    {
        initializeSkillsEventBinding = new EventBinding<TopDownInitializeSkillsEvent>(InitializeSkillsUI);
        EventBus<TopDownInitializeSkillsEvent>.Register(initializeSkillsEventBinding);
    }
    private void OnDisable()
    {
        EventBus<TopDownInitializeSkillsEvent>.Deregister(initializeSkillsEventBinding);
    }
    private void InitializeSkillsUI(TopDownInitializeSkillsEvent receive)
    {
        for (int i = 0; i < _skillIcons.Length; i++)
        {
            _cooldownTexts[i] = _skillIcons[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            _cooldownTexts[i].gameObject.SetActive(false);

            _coolDownProgressImage[i] = _skillIcons[i].transform.GetChild(0).GetComponent<Image>();
            _coolDownProgressImage[i].gameObject.SetActive(false);
            _coolDownProgressImage[i].material.SetFloat("_FillAmount", 0f);

            _coolDownFinishImage[i] = _skillIcons[i].transform.GetChild(1).GetComponent<Image>();
            _coolDownFinishImage[i].gameObject.SetActive(false);
            _coolDownFinishImage[i].material.SetFloat("_FillAmount", 0f);

            _finishEffectTriggered[i] = false;
        }

        _skillRuntimeInstances = receive.skillRuntimeInstances;
        List<SkillRuntimeInstance> skillList = _skillRuntimeInstances.ToList();
        for (int i = 0; i < _skillIcons.Length; i++)
        {
            SkillRuntimeInstance skillInSlot = skillList.FirstOrDefault(x => x.slotIndex == i);

            if (skillInSlot != null)
            {
                _skillIcons[i].sprite = _skillRuntimeInstances[i].Definition.skillIcon;
                Color fullAplha = _skillIcons[i].color;
                fullAplha.a = 1f;
                _skillIcons[i].color = fullAplha;
                continue;
            }
            Color zeroAlpha = _skillIcons[i].color;
            zeroAlpha.a = 0f;
            _skillIcons[i].color = zeroAlpha;

        }
    }

    private void Update()
    {
        SkillCooldownCounter();
    }

    private void SkillCooldownCounter()
    {
        if (_skillRuntimeInstances.IsNullOrEmpty()) return;
        foreach (var skillInstance in _skillRuntimeInstances)
        {
            if (skillInstance.IsOnCooldown)
            {
                Color fadeColor = _skillIcons[skillInstance.slotIndex].color;
                fadeColor.a = 0.25f;
                _skillIcons[skillInstance.slotIndex].color = fadeColor;

                _cooldownTexts[skillInstance.slotIndex].gameObject.SetActive(true);
                _cooldownTexts[skillInstance.slotIndex].text = Mathf.Ceil(skillInstance.CurrentCooldownRemaining).ToString();

                _coolDownProgressImage[skillInstance.slotIndex].gameObject.SetActive(true);
                _coolDownProgressImage[skillInstance.slotIndex].material.SetFloat("_FillAmount", skillInstance.CurrentCooldownNormalized);

                // only trigger this once per cooldown
                float currentFillAmount = _coolDownProgressImage[skillInstance.slotIndex].material.GetFloat("_FillAmount");
                if (skillInstance.CurrentCooldownRemaining <= 0.4f && !_finishEffectTriggered[skillInstance.slotIndex])
                {
                    _finishEffectTriggered[skillInstance.slotIndex] = true;
                    StartFinishCooldownEffect(skillInstance.slotIndex).Forget();
                }
            }
            else
            {
                if (_skillIcons[skillInstance.slotIndex].color.a != 1f)
                {
                    Color fullAplha = _skillIcons[skillInstance.slotIndex].color;
                    fullAplha.a = 1f;
                    _skillIcons[skillInstance.slotIndex].color = fullAplha;
                }

                if (_coolDownProgressImage[skillInstance.slotIndex].gameObject.activeSelf)
                {
                    _coolDownProgressImage[skillInstance.slotIndex].gameObject.SetActive(false);
                    _coolDownProgressImage[skillInstance.slotIndex].material.SetFloat("_FillAmount", 0f);
                    _finishEffectTriggered[skillInstance.slotIndex] = false;
                }

                if (_cooldownTexts[skillInstance.slotIndex].gameObject.activeSelf)
                    _cooldownTexts[skillInstance.slotIndex].gameObject.SetActive(false);
            }
        }
    }

    private async UniTaskVoid StartFinishCooldownEffect(int slotIndex)
    {
        _coolDownFinishImage[slotIndex].gameObject.SetActive(true);
        _coolDownFinishImage[slotIndex].material.SetFloat("_FillAmount", 0f);

        float duration = 0.6f; // Short duration for the effect
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _coolDownFinishImage[slotIndex].material.SetFloat("_FillAmount", t);
            await UniTask.Yield();
        }

        // Ensure it reaches exactly 1
        _coolDownFinishImage[slotIndex].material.SetFloat("_FillAmount", 1f); 

        // Optional: deactivate after a brief delay
        //await UniTask.Delay(100);
        _coolDownFinishImage[slotIndex].gameObject.SetActive(false);
        _coolDownFinishImage[slotIndex].material.SetFloat("_FillAmount", 0f);
    }
}