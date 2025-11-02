using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 

namespace Turnbase
{
    public class EnemyStatsUI : MonoBehaviour
    {
        public Image hpBarFill;
        public Image mpBarFill;
        public Image shieldBarFill;
        public Image elementImage;

        public Transform statusEffectContainer;
        public GameObject statusEffectIconPrefab;

        public ElementMapping elementMapping;

        private Character ownerCharacter;

        void Awake()
        {
            ownerCharacter = GetComponentInParent<Character>();
        }

        void Start()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (ownerCharacter == null) return;

            CharacterStats stats = ownerCharacter.stats;

            UpdateHpBar(stats);
            UpdateMpBar(stats);
            UpdateShieldBar(stats);
            UpdateElementImage();

            UpdateStatusEffects(ownerCharacter.GetActiveStatusEffects());
        }

        private void UpdateHpBar(CharacterStats stats)
        {
            if (hpBarFill != null)
            {
                hpBarFill.fillAmount = (float)stats.currentHP / stats.maxHP;
            }
        }

        private void UpdateMpBar(CharacterStats stats)
        {
            if (mpBarFill != null && stats.maxMP > 0)
            {
                mpBarFill.fillAmount = (float)stats.currentMP / stats.maxMP;
            }
        }

        private void UpdateShieldBar(CharacterStats stats)
        {
            if (shieldBarFill != null && stats.maxHP > 0)
            {
                shieldBarFill.fillAmount = (float)stats.currentShield / stats.maxHP;
            }
        }

        private void UpdateElementImage()
        {
            if (elementImage == null || elementMapping == null) return;

            Sprite elementSprite = elementMapping.GetElementSprite(ownerCharacter.characterElement);

            if (elementSprite != null)
            {
                elementImage.sprite = elementSprite;
                elementImage.enabled = true;
            }
            else
            {
                elementImage.enabled = false;
            }
        }

        public void UpdateStatusEffects(List<StatusEffectData> activeEffects)
        {
            if (statusEffectContainer == null || statusEffectIconPrefab == null)
            {
                return;
            }

            foreach (Transform child in statusEffectContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var effect in activeEffects)
            {
                GameObject iconObject = Instantiate(statusEffectIconPrefab, statusEffectContainer);

                Image iconImage = iconObject.GetComponent<Image>();
                TextMeshProUGUI turnText = iconObject.GetComponentInChildren<TextMeshProUGUI>();

                if (iconImage != null && effect.Icon != null)
                {
                    iconImage.sprite = effect.Icon;
                }

                if (turnText != null)
                {
                    turnText.text = effect.TurnsRemaining.ToString();
                }
            }
        }
    }
}