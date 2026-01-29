using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillUiController : MonoBehaviour
{
    private EventBinding<TopDownInitializeSkillsEvent> initializeSkillsEventBinding;

    [SerializeField]
    private Image[] _skillIcons = new Image[6];
    private TextMeshProUGUI[] _cooldownTexts = new TextMeshProUGUI[6];
    private Image[] _coolDownImage = new Image[6];

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
        for(int i = 0; i < _skillIcons.Length; i++)
        {
            _cooldownTexts[i] = _skillIcons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            _cooldownTexts[i].gameObject.SetActive(false);

            _coolDownImage[i] = _skillIcons[i].transform.GetChild(0).GetComponent<Image>();
            _coolDownImage[i].gameObject.SetActive(false);
            _coolDownImage[i].fillAmount = 0f;
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
        foreach(var skillInstance in _skillRuntimeInstances)
        {
            if (skillInstance.IsOnCooldown)
            {
                Color fadeColor = _skillIcons[skillInstance.slotIndex].color;
                fadeColor.a = 0.25f;
                _skillIcons[skillInstance.slotIndex].color = fadeColor;

                _cooldownTexts[skillInstance.slotIndex].gameObject.SetActive(true);
                _cooldownTexts[skillInstance.slotIndex].text = Mathf.Ceil(skillInstance.CurrentCooldownRemaining).ToString();

                _coolDownImage[skillInstance.slotIndex].gameObject.SetActive(true);
                _coolDownImage[skillInstance.slotIndex].fillAmount = skillInstance.CurrentCooldownNormalized;
            }
            else
            {
                if (_skillIcons[skillInstance.slotIndex].color.a != 1f)
                {
                    Color fullAplha = _skillIcons[skillInstance.slotIndex].color;
                    fullAplha.a = 1f;
                    _skillIcons[skillInstance.slotIndex].color = fullAplha;
                }

                if (_coolDownImage[skillInstance.slotIndex].gameObject.activeSelf)
                {
                    _coolDownImage[skillInstance.slotIndex].gameObject.SetActive(false);
                    _coolDownImage[skillInstance.slotIndex].fillAmount = 0f;
                }

                if (_cooldownTexts[skillInstance.slotIndex].gameObject.activeSelf)
                    _cooldownTexts[skillInstance.slotIndex].gameObject.SetActive(false);
            }
        }
    }
}
