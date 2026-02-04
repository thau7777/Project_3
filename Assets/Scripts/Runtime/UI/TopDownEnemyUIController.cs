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
    [TabGroup("Bar Settings")]
    [SerializeField] private Color healthBarBackFillHealColor = Color.green;

    [TabGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillDelayDuration = 0.5f;
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillAnimationDuration = 0.5f;
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private AnimationCurve shieldBarBackFillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private Color shieldBarBackFillHurtColor = Color.cyan;
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private Color shieldBarBackFillHealColor = Color.blue;

    

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

        // Only initialize handle if it exists
        if (healthBarSliderHandle != null)
        {
            healthBarSliderHandle.transform.localScale = Vector3.zero;
        }
    }

    

    /// <summary>
    /// Sets the health bar value
    /// </summary>
    public void SetHealth(float current, float max)
    {
        currentHealth = Mathf.Clamp(current, 0, max);
        maxHealth = max;
        if (healthBarSlider != null)
        {
            Color healthBarBackFillColor = healthBarSlider.value > currentHealth / maxHealth ? healthBarBackFillHurtColor : healthBarBackFillHealColor;
            SetHealthBarBackFillColor(healthBarBackFillColor);
            healthBarSlider.value = maxHealth > 0 ? currentHealth / maxHealth : 0;
        }

        UpdateHealthText();

        // Reset and restart the back fill animation
        if (healthBackFillCoroutine != null)
        {
            StopCoroutine(healthBackFillCoroutine);
        }
        healthBackFillCoroutine = StartCoroutine(AnimateHealthBarBackFill());

        // Animate the slider handle scale
        if (handleScaleCoroutine != null)
        {
            StopCoroutine(handleScaleCoroutine);
        }
        handleScaleCoroutine = StartCoroutine(AnimateHealthBarSliderHandle());
    }

    /// <summary>
    /// Animates the health bar back fill after a delay
    /// </summary>
    private IEnumerator AnimateHealthBarBackFill()
    {
        if (healthBarBackFill == null || healthBarSlider == null)
            yield break;

        // Wait for the delay
        yield return Helpers.GetWaitForSeconds(healthBarBackFillDelayDuration);

        // Get start and target values
        float startFillAmount = healthBarBackFill.fillAmount;
        float targetFillAmount = healthBarSlider.value;
        float elapsed = 0f;

        // Animate the fill amount
        while (elapsed < healthBarBackFillAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healthBarBackFillAnimationDuration;
            float curveValue = healthBarBackFillCurve.Evaluate(t);

            healthBarBackFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, curveValue);

            yield return null;
        }

        // Ensure we reach the exact target value
        healthBarBackFill.fillAmount = targetFillAmount;
    }

    /// <summary>
    /// Animates the health bar slider handle scale
    /// </summary>
    private IEnumerator AnimateHealthBarSliderHandle()
    {
        if (healthBarSliderHandle == null)
            yield break;

        Vector3 targetScale = new Vector3(healthBarSliderHandleMaxScale, healthBarSliderHandleMaxScale, healthBarSliderHandleMaxScale);

        // Get current scale to start from (in case animation was interrupted)
        Vector3 startScale = healthBarSliderHandle.transform.localScale;
        float elapsed = 0f;

        // Scale up from current scale to max scale
        while (elapsed < healthBarSliderHandleScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healthBarSliderHandleScaleDuration;
            float curveValue = healthBarSliderHandleScaleCurve.Evaluate(t);

            healthBarSliderHandle.transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);

            yield return null;
        }

        healthBarSliderHandle.transform.localScale = targetScale;

        // Hold at max scale
        yield return Helpers.GetWaitForSeconds(healthBarSliderHandleHoldDuration);

        // Scale down
        elapsed = 0f;
        while (elapsed < healthBarSliderHandleScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / healthBarSliderHandleScaleDuration;
            float curveValue = healthBarSliderHandleScaleCurve.Evaluate(t);

            healthBarSliderHandle.transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, curveValue);

            yield return null;
        }

        healthBarSliderHandle.transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Sets the shield bar value
    /// </summary>
    public void SetShield(float current, float max)
    {
        currentShield = Mathf.Clamp(current, 0, max);
        maxShield = max;

        if (shieldBarSlider != null)
        {
            Color shieldBarBackFillColor = shieldBarSlider.value > currentShield / maxShield ? shieldBarBackFillHurtColor : shieldBarBackFillHealColor;
            SetShieldBarBackFillColor(shieldBarBackFillColor);
            shieldBarSlider.value = maxShield > 0 ? currentShield / maxShield : 0;
        }

        UpdateShieldText();

        // Reset and restart the back fill animation
        if (shieldBackFillCoroutine != null)
        {
            StopCoroutine(shieldBackFillCoroutine);
        }
        shieldBackFillCoroutine = StartCoroutine(AnimateShieldBarBackFill());
    }

    /// <summary>
    /// Animates the shield bar back fill after a delay
    /// </summary>
    private IEnumerator AnimateShieldBarBackFill()
    {
        if (shieldBarBackFill == null || shieldBarSlider == null)
            yield break;

        // Wait for the delay
        yield return Helpers.GetWaitForSeconds(shieldBarBackFillDelayDuration);

        // Get start and target values
        float startFillAmount = shieldBarBackFill.fillAmount;
        float targetFillAmount = shieldBarSlider.value;
        float elapsed = 0f;

        // Animate the fill amount
        while (elapsed < shieldBarBackFillAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shieldBarBackFillAnimationDuration;
            float curveValue = shieldBarBackFillCurve.Evaluate(t);

            shieldBarBackFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, curveValue);

            yield return null;
        }

        // Ensure we reach the exact target value
        shieldBarBackFill.fillAmount = targetFillAmount;
    }



    /// <summary>
    /// Gets health percentage (0-1)
    /// </summary>
    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? currentHealth / maxHealth : 0;
    }

    /// <summary>
    /// Gets shield percentage (0-1)
    /// </summary>
    public float GetShieldPercentage()
    {
        return maxShield > 0 ? currentShield / maxShield : 0;
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            if (showPercentage)
            {
                healthText.text = $"{Mathf.RoundToInt(GetHealthPercentage() * 100)}%";
            }
            else
            {
                healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
            }
        }
    }

    private void UpdateShieldText()
    {
        if (shieldText != null)
        {
            if (showPercentage)
            {
                shieldText.text = $"{Mathf.RoundToInt(GetShieldPercentage() * 100)}%";
            }
            else
            {
                shieldText.text = $"{Mathf.RoundToInt(currentShield)}/{Mathf.RoundToInt(maxShield)}";
            }
        }
    }

    /// <summary>
    /// Sets the color of the health bar back fill
    /// </summary>
    public void SetHealthBarBackFillColor(Color color)
    {
        if (healthBarBackFill != null)
        {
            healthBarBackFill.color = color;
        }
    }

    /// <summary>
    /// Sets the color of the shield bar back fill
    /// </summary>
    public void SetShieldBarBackFillColor(Color color)
    {
        if (shieldBarBackFill != null)
        {
            shieldBarBackFill.color = color;
        }
    }

}