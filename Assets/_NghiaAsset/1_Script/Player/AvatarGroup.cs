using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Turnbase
{
    public class AvatarGroup : MonoBehaviour
    {
        public Image avatar;
        public Image hpBar;
        public Image hpBarDelayed;
        public float hpLerpSpeed = 2f;

        public Image mpBar;
        public Image mpBarDelayed;
        public float mpLerpSpeed = 2f;
        public Image shieldBar;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI mpText;

        public Transform statusEffectContainer;
        public GameObject statusEffectIconPrefab;

        private Character ownerCharacter;
        private CharacterStatusDataProvider dataProvider;
        private Coroutine hpLerpCoroutine;
        private Coroutine mpLerpCoroutine;

        public GameObject isPetAvatar;
        public bool petAvatar = false;


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
                if (isPetAvatar != null)
                {
                    if (ownerCharacter.isPet) isPetAvatar.SetActive(false);
                }

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
                if (isPetAvatar != null && ownerCharacter.isPet) isPetAvatar.SetActive(false);
                UpdateUI(ownerCharacter.stats, ownerCharacter.info);
            }
            else
            {
                ownerCharacter = GetComponentInParent<Character>();
                if (ownerCharacter != null)
                {
                    if (isPetAvatar != null && ownerCharacter.isPet) isPetAvatar.SetActive(false);
                    UpdateUI(ownerCharacter.stats, ownerCharacter.info);
                }
            }
        }

        public void UpdateUI(CharacterStats stats, CharacterInfo info)
        {
            avatar.sprite = info.Avatar;

            float targetFill = (float)stats.currentHP / stats.maxHP;
            hpBar.fillAmount = targetFill;

            if (hpBarDelayed != null)
            {
                if (hpLerpCoroutine != null) StopCoroutine(hpLerpCoroutine);
                hpLerpCoroutine = StartCoroutine(LerpHpDelayed(targetFill));
            }

            float targetMpFill = (float)stats.currentMP / stats.maxMP;
            mpBar.fillAmount = targetMpFill;

            if (mpBarDelayed != null)
            {
                if (mpLerpCoroutine != null) StopCoroutine(mpLerpCoroutine);
                mpLerpCoroutine = StartCoroutine(LerpMpDelayed(targetMpFill));
            }

            //shieldBar.fillAmount = (float)stats.currentShield / stats.maxHP;
            hpText.text = $"{stats.currentHP} / {stats.maxHP}";
            mpText.text = $"{stats.currentMP} / {stats.maxMP}";

            if (dataProvider != null)
            {
                UpdateStatusEffect(dataProvider.GetActiveStatusEffects());
            }
        }

        private IEnumerator LerpHpDelayed(float targetFill)
        {
            yield return new WaitForSeconds(0.2f);

            while (Mathf.Abs(hpBarDelayed.fillAmount - targetFill) > 0.001f)
            {
                hpBarDelayed.fillAmount = Mathf.Lerp(hpBarDelayed.fillAmount, targetFill, Time.deltaTime * hpLerpSpeed);
                yield return null;
            }
            hpBarDelayed.fillAmount = targetFill;
        }

        private IEnumerator LerpMpDelayed(float targetFill)
        {
            yield return new WaitForSeconds(0.2f);

            while (Mathf.Abs(mpBarDelayed.fillAmount - targetFill) > 0.001f)
            {
                mpBarDelayed.fillAmount = Mathf.Lerp(mpBarDelayed.fillAmount, targetFill, Time.deltaTime * mpLerpSpeed);
                yield return null;
            }
            mpBarDelayed.fillAmount = targetFill;
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
