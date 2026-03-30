using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

namespace Turnbase
{
    public class EnemyStatsUI : MonoBehaviour
    {
        public Image hpBarFill;
        public Image hpBarFillDelay;
        public float lerpSpeed = 2f;

        public Image mpBarFill;
        public Image trailblazeFill;
        public Image trailblazeFillDelay;
        public Image shieldBarFill;
        public Image elementImage;

        public Transform statusEffectContainer;
        public GameObject statusEffectIconPrefab;

        public ElementMapping elementMapping;

        private Character ownerCharacter;
        private CharacterStatusDataProvider dataProvider;
        private Coroutine hpLerpCoroutine;
        private Coroutine trailblazeLerpCoroutine;

        void Awake()
        {
            ownerCharacter = GetComponentInParent<Character>();
            if (ownerCharacter != null)
            {
                dataProvider = ownerCharacter.GetComponent<CharacterStatusDataProvider>();
            }
        }

        void Start()
        {
            UpdateUI();
        }

        void OnEnable()
        {
            EventBusUI<StatusEffectChangedEvent>.Subscribe(OnStatusEffectChanged);

            UpdateUI();
        }

        void OnDisable()
        {
            EventBusUI<StatusEffectChangedEvent>.Unsubscribe(OnStatusEffectChanged);
        }

        private void OnStatusEffectChanged(StatusEffectChangedEvent eventData)
        {
            if (eventData.TargetCharacter == ownerCharacter)
            {
                UpdateUI();
            }
        }

        public void UpdateUI()
        {
            if (ownerCharacter == null) return;

            CharacterStats stats = ownerCharacter.stats;

            UpdateHpBar(stats);
            UpdateMpBar(stats);
            //UpdateShieldBar(stats);
            UpdateElementImage();
            UpdateTrailblazeBar();
            UpdateStatusEffects(dataProvider.GetActiveStatusEffects());
        }

        private void UpdateHpBar(CharacterStats stats)
        {
            if (hpBarFill != null)
            {
                float targetFill = (float)stats.currentHP / stats.maxHP;    
                hpBarFill.fillAmount = targetFill;

                if (hpBarFillDelay != null)
                {
                    if (hpLerpCoroutine != null) StopCoroutine(hpLerpCoroutine);
                    hpLerpCoroutine = StartCoroutine(LerpHpDelayed(targetFill));
                }
            }

        }

        private IEnumerator LerpHpDelayed(float targetFill)
        {
            yield return new WaitForSeconds(0.2f);

            while (Mathf.Abs(hpBarFillDelay.fillAmount - targetFill) > 0.001f)
            {
                hpBarFillDelay.fillAmount = Mathf.Lerp(hpBarFillDelay.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
                yield return null;
            }
            hpBarFillDelay.fillAmount = targetFill;
        }

        private void UpdateMpBar(CharacterStats stats)
        {
            if (mpBarFill != null && stats.maxMP > 0)
            {
                mpBarFill.fillAmount = (float)stats.currentMP / stats.maxMP;
            }
        }

        ////private void UpdateShieldBar(CharacterStats stats)
        ////{
        ////    if (shieldBarFill != null && stats.maxHP > 0)
        ////    {
        ////        shieldBarFill.fillAmount = (float)stats.currentShield / stats.maxShield;

        ////    }
        ////}

        public void UpdateTrailblazeBar()
        {
            if (!(ownerCharacter is Enemy enemyOwner))
            {
                Debug.LogError("EnemyStatsUI chỉ nên được sử dụng cho Enemy.");
                return;
            }

            float currentTrailblaze = enemyOwner.traildblaze;

            float maxTrailblaze = 100f; 

            if (trailblazeFill != null && maxTrailblaze > 0f)
            {
                float targetFill = currentTrailblaze / maxTrailblaze;
                trailblazeFill.fillAmount = targetFill;

                if (trailblazeFillDelay != null)
                {
                    if (trailblazeLerpCoroutine != null) StopCoroutine(trailblazeLerpCoroutine);
                    trailblazeLerpCoroutine = StartCoroutine(LerpTrailblazeDelayed(targetFill));
                }
            }
            else
            {
                if (trailblazeFill != null)
                {
                    trailblazeFill.fillAmount = 0f;
                }
                if (trailblazeFillDelay != null)
                {
                    trailblazeFillDelay.fillAmount = 0f;
                }
            }
        }

        private IEnumerator LerpTrailblazeDelayed(float targetFill)
        {
            yield return new WaitForSeconds(0.2f);

            while (Mathf.Abs(trailblazeFillDelay.fillAmount - targetFill) > 0.001f)
            {
                trailblazeFillDelay.fillAmount = Mathf.Lerp(trailblazeFillDelay.fillAmount, targetFill, Time.deltaTime * lerpSpeed);
                yield return null;
            }
            trailblazeFillDelay.fillAmount = targetFill;
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