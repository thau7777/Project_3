using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopDownEnemyUIController : MonoBehaviour
{
    [TabGroup("Bar References")]
    [SerializeField] private Slider healthBarSlider;
    [TabGroup("Bar References")]
    [SerializeField] private Image healthBarBackFill;
    [TabGroup("Bar References")]
    [SerializeField] private Image healthBarSliderHandle;
    [TabGroup("Bar References")]
    [SerializeField] private Slider shieldBarSlider;
    [TabGroup("Bar References")]
    [SerializeField] private Image shieldBarBackFill;

    [TabGroup("Bar Settings")]
    [SerializeField] private float healthBarBackFillDelayDuration = 0.5f;
    [TabGroup("Bar Settings")]
    [SerializeField] private float healthBarBackFillAnimationDuration = 0.5f;
    [TabGroup("Bar Settings")]
    [SerializeField] private AnimationCurve healthBarBackFillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Bar Settings")]
    [SerializeField] private float healthBarSliderHandleMaxScale = 1.5f;
    [TabGroup("Bar Settings")]
    [SerializeField] private float healthBarSliderHandleScaleDuration = 0.3f;
    [TabGroup("Bar Settings")]
    [SerializeField] private float healthBarSliderHandleHoldDuration = 0.2f;
    [TabGroup("Bar Settings")]
    [SerializeField] private AnimationCurve healthBarSliderHandleScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Bar Settings")]
    [SerializeField] private Color healthBarBackFillHurtColor = Color.white;

    [TabGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillDelayDuration = 0.5f;
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillAnimationDuration = 0.5f;
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private AnimationCurve shieldBarBackFillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private Color shieldBarBackFillHurtColor = Color.cyan;

    [TabGroup("Optional Text Labels")]
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [TabGroup("Optional Text Labels")]
    [SerializeField] private TMPro.TextMeshProUGUI shieldText;
    [TabGroup("Optional Text Labels")]
    [SerializeField] private bool showPercentage = true;

    private float currentHealth;
    private float maxHealth;
    private float currentShield;
    private float maxShield;

    private Coroutine healthBackFillCoroutine;
    private Coroutine handleScaleCoroutine;
    private Coroutine shieldBackFillCoroutine;


    public void InitializeValue(float healthCurrent, float healthMax, float shieldCurrent, float shieldMax)
    {
        currentHealth = healthCurrent;
        maxHealth = healthMax;
        currentShield = shieldCurrent;
        maxShield = shieldMax;

        healthBarSlider.value = healthMax > 0 ? healthCurrent / healthMax : 0;
        healthBarBackFill.fillAmount = healthMax > 0 ? healthCurrent / healthMax : 0;

        shieldBarSlider.value = shieldMax > 0 ? shieldCurrent / shieldMax : 0;
        shieldBarBackFill.fillAmount = shieldMax > 0 ? shieldCurrent / shieldMax : 0;

        if (healthBarSliderHandle != null)
            healthBarSliderHandle.transform.localScale = Vector3.zero;
    }

    public void SetHealth(float current, float max)
    {
        float newValue = max > 0 ? Mathf.Clamp(current, 0, max) / max : 0;
        bool isDamage = newValue < healthBarSlider.value;

        currentHealth = Mathf.Clamp(current, 0, max);
        maxHealth = max;

        if (healthBarSlider != null)
            healthBarSlider.value = newValue;

        UpdateHealthText();

        if (isDamage)
        {
            // Damage: backfill lingers then animates down
            SetHealthBarBackFillColor(healthBarBackFillHurtColor);

            if (healthBackFillCoroutine != null) StopCoroutine(healthBackFillCoroutine);
            healthBackFillCoroutine = StartCoroutine(AnimateHealthBarBackFill());

            if (handleScaleCoroutine != null) StopCoroutine(handleScaleCoroutine);
            handleScaleCoroutine = StartCoroutine(AnimateHealthBarSliderHandle());
        }
        else
        {
            // Heal: snap backfill immediately to match main bar, no animation
            if (healthBackFillCoroutine != null)
            {
                StopCoroutine(healthBackFillCoroutine);
                healthBackFillCoroutine = null;
            }
            if (healthBarBackFill != null)
                healthBarBackFill.fillAmount = newValue;
        }
    }

    private IEnumerator AnimateHealthBarBackFill()
    {
        if (healthBarBackFill == null || healthBarSlider == null) yield break;

        yield return Helpers.GetWaitForSeconds(healthBarBackFillDelayDuration);

        float startFillAmount = healthBarBackFill.fillAmount;
        float targetFillAmount = healthBarSlider.value;
        float elapsed = 0f;

        while (elapsed < healthBarBackFillAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healthBarBackFillAnimationDuration;
            healthBarBackFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, healthBarBackFillCurve.Evaluate(t));
            yield return null;
        }

        healthBarBackFill.fillAmount = targetFillAmount;
    }

    private IEnumerator AnimateHealthBarSliderHandle()
    {
        if (healthBarSliderHandle == null) yield break;

        Vector3 targetScale = Vector3.one * healthBarSliderHandleMaxScale;
        Vector3 startScale = healthBarSliderHandle.transform.localScale;
        float elapsed = 0f;

        while (elapsed < healthBarSliderHandleScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healthBarSliderHandleScaleDuration;
            healthBarSliderHandle.transform.localScale = Vector3.Lerp(startScale, targetScale, healthBarSliderHandleScaleCurve.Evaluate(t));
            yield return null;
        }

        healthBarSliderHandle.transform.localScale = targetScale;
        yield return Helpers.GetWaitForSeconds(healthBarSliderHandleHoldDuration);

        elapsed = 0f;
        while (elapsed < healthBarSliderHandleScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healthBarSliderHandleScaleDuration;
            healthBarSliderHandle.transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, healthBarSliderHandleScaleCurve.Evaluate(t));
            yield return null;
        }

        healthBarSliderHandle.transform.localScale = Vector3.zero;
    }

    public void SetShield(float current, float max)
    {
        float newValue = max > 0 ? Mathf.Clamp(current, 0, max) / max : 0;
        bool isDamage = newValue < shieldBarSlider.value;

        currentShield = Mathf.Clamp(current, 0, max);
        maxShield = max;

        if (shieldBarSlider != null)
            shieldBarSlider.value = newValue;

        UpdateShieldText();

        if (isDamage)
        {
            // Damage: backfill lingers then animates down
            SetShieldBarBackFillColor(shieldBarBackFillHurtColor);

            if (shieldBackFillCoroutine != null) StopCoroutine(shieldBackFillCoroutine);
            shieldBackFillCoroutine = StartCoroutine(AnimateShieldBarBackFill());
        }
        else
        {
            // Heal/restore: snap backfill immediately to match main bar
            if (shieldBackFillCoroutine != null)
            {
                StopCoroutine(shieldBackFillCoroutine);
                shieldBackFillCoroutine = null;
            }
            if (shieldBarBackFill != null)
                shieldBarBackFill.fillAmount = newValue;
        }
    }

    private IEnumerator AnimateShieldBarBackFill()
    {
        if (shieldBarBackFill == null || shieldBarSlider == null) yield break;

        yield return Helpers.GetWaitForSeconds(shieldBarBackFillDelayDuration);

        float startFillAmount = shieldBarBackFill.fillAmount;
        float targetFillAmount = shieldBarSlider.value;
        float elapsed = 0f;

        while (elapsed < shieldBarBackFillAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shieldBarBackFillAnimationDuration;
            shieldBarBackFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, shieldBarBackFillCurve.Evaluate(t));
            yield return null;
        }

        shieldBarBackFill.fillAmount = targetFillAmount;
    }

    public float GetHealthPercentage() => maxHealth > 0 ? currentHealth / maxHealth : 0;
    public float GetShieldPercentage() => maxShield > 0 ? currentShield / maxShield : 0;

    private void UpdateHealthText()
    {
        if (healthText == null) return;
        healthText.text = showPercentage
            ? $"{Mathf.RoundToInt(GetHealthPercentage() * 100)}%"
            : $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
    }

    private void UpdateShieldText()
    {
        if (shieldText == null) return;
        shieldText.text = showPercentage
            ? $"{Mathf.RoundToInt(GetShieldPercentage() * 100)}%"
            : $"{Mathf.RoundToInt(currentShield)}/{Mathf.RoundToInt(maxShield)}";
    }

    public void SetHealthBarBackFillColor(Color color)
    {
        if (healthBarBackFill != null) healthBarBackFill.color = color;
    }

    public void SetShieldBarBackFillColor(Color color)
    {
        if (shieldBarBackFill != null) shieldBarBackFill.color = color;
    }
}