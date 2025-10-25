using UnityEngine;
using UnityEngine.UI;


namespace Turnbase
{
    public class EnemyStatsUI : MonoBehaviour
    {
        public Image hpBarFill;
        public Image mpBarFill;
        public Image shieldBarFill;
        public Image elementImage;

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
            Enemy enemy = ownerCharacter as Enemy;

            if (enemy == null || elementImage == null || elementMapping == null) return;

            Sprite elementSprite = elementMapping.GetElementSprite(enemy.characterElement);

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
    }
}