using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Turnbase
{
    public class AvatarGroup : MonoBehaviour
    {
        public Image avatar;
        public Image hpBar;
        public Image mpBar;
        public Image shieldBar;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI mpText;

        public Transform statusEffectContainer;
        public GameObject statusEffectIconPrefab;

        private Character ownerCharacter;
        private CharacterStatusDataProvider dataProvider;


        private void Awake()
        {
            ownerCharacter = GetComponentInParent<Character>();
            if (ownerCharacter != null)
            {
                dataProvider = ownerCharacter.GetComponent<CharacterStatusDataProvider>();
            }
        }
        public void SetOwner(Character character)
        {
            ownerCharacter = character;
            if (ownerCharacter != null)
            {
                dataProvider = ownerCharacter.GetComponent<CharacterStatusDataProvider>();
                UpdateUI(ownerCharacter.stats, ownerCharacter.info);
            }
            else
            {
                dataProvider = null;
            }
        }

        private void Start()
        {
            if (ownerCharacter != null)
            {
                UpdateUI(ownerCharacter.stats, ownerCharacter.info);
            }
            else
            {
                ownerCharacter = GetComponentInParent<Character>();
                if (ownerCharacter != null)
                {
                    UpdateUI(ownerCharacter.stats, ownerCharacter.info);
                }
            }
        }

        public void UpdateUI(CharacterStats stats, CharacterInfo info)
        {
            avatar.sprite = info.Avatar;
            hpBar.fillAmount = (float)stats.currentHP / stats.maxHP;
            mpBar.fillAmount = (float)stats.currentMP / stats.maxMP;
            shieldBar.fillAmount = (float)stats.currentShield / stats.maxHP;
            hpText.text = $"{stats.currentHP} / {stats.maxHP}";
            mpText.text = $"{stats.currentMP} / {stats.maxMP}";

            UpdateStatusEffect(dataProvider.GetActiveStatusEffects());
        }

        public void UpdateStatusEffect(List<StatusEffectData> statusEffects)
        {
            foreach(Transform child in statusEffectContainer)
            {
                Destroy(child.gameObject);
            }

            if(statusEffectIconPrefab == null)
            {
                return;
            }

            foreach (var effect in statusEffects)
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
