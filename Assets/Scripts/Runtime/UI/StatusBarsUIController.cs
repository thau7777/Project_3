using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusBarsUIController : MonoBehaviour
{
    [FoldoutGroup("Bar References")]
    [SerializeField] private Slider healthBarSlider;
    [FoldoutGroup("Bar References")]
    [SerializeField] private Image healthBarBackFill;
    [FoldoutGroup("Bar References")]
    [SerializeField] private Image healthBarSliderHandle;
    [FoldoutGroup("Bar References")]
    [SerializeField] private Slider shieldBarSlider;
    [FoldoutGroup("Bar References")]
    [SerializeField] private Image shieldBarBackFill;

    [FoldoutGroup("Bar Settings")]
    [SerializeField] private float healthBarBackFillDelayDuration = 0.5f;
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private float healthBarBackFillAnimationDuration = 0.5f;
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private AnimationCurve healthBarBackFillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private float healthBarSliderHandleMaxScale = 1.5f;
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private float healthBarSliderHandleScaleDuration = 0.3f;
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private float healthBarSliderHandleHoldDuration = 0.2f;
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private AnimationCurve healthBarSliderHandleScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private Color healthBarBackFillHurtColor = Color.white;
    [FoldoutGroup("Bar Settings")]
    [SerializeField] private Color healthBarBackFillHealColor = Color.green;

    [FoldoutGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillDelayDuration = 0.5f;
    [FoldoutGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillAnimationDuration = 0.5f;
    [FoldoutGroup("Shield Bar Settings")]
    [SerializeField] private AnimationCurve shieldBarBackFillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [FoldoutGroup("Shield Bar Settings")]
    [SerializeField] private Color shieldBarBackFillHurtColor = Color.cyan;
    [FoldoutGroup("Shield Bar Settings")]
    [SerializeField] private Color shieldBarBackFillHealColor = Color.blue;

    [FoldoutGroup("Camera Settings")]
    [SerializeField] private bool alwaysFaceCamera = true;
    [FoldoutGroup("Camera Settings")]
    [SerializeField] private bool smoothRotation = true;
    [FoldoutGroup("Camera Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    [FoldoutGroup("Optional Text Labels")]
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [FoldoutGroup("Optional Text Labels")]
    [SerializeField] private TMPro.TextMeshProUGUI shieldText;
    [FoldoutGroup("Optional Text Labels")]
    [SerializeField] private bool showPercentage = true;

    private Camera mainCamera;
    private float currentHealth;
    private float maxHealth;
    private float currentShield;
    private float maxShield;

    private Coroutine healthBackFillCoroutine;
    private Coroutine handleScaleCoroutine;
    private Coroutine shieldBackFillCoroutine;

    private void Start()
    {
        mainCamera = Camera.main;
    }

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

    private void LateUpdate()
    {
        if (alwaysFaceCamera && mainCamera != null)
        {
            FaceCamera();
        }
    }

    /// <summary>
    /// Makes the UI always face the camera plane
    /// </summary>
    public void FaceCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        if (smoothRotation)
        {
            // Smooth rotation to match camera's forward direction
            Quaternion targetRotation = Quaternion.LookRotation(mainCamera.transform.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Instant rotation to match camera's forward direction
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
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
    /// Updates health without changing max health
    /// </summary>
    public void UpdateHealth(float current)
    {
        SetHealth(current, maxHealth);
    }

    /// <summary>
    /// Updates shield without changing max shield
    /// </summary>
    public void UpdateShield(float current)
    {
        SetShield(current, maxShield);
    }

    /// <summary>
    /// Shows or hides the health bar
    /// </summary>
    public void SetHealthBarVisible(bool visible)
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.transform.parent.gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// Shows or hides the shield bar
    /// </summary>
    public void SetShieldBarVisible(bool visible)
    {
        if (shieldBarSlider != null)
        {
            shieldBarSlider.transform.parent.gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// Gets current health value
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Gets max health value
    /// </summary>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Gets current shield value
    /// </summary>
    public float GetCurrentShield()
    {
        return currentShield;
    }

    /// <summary>
    /// Gets max shield value
    /// </summary>
    public float GetMaxShield()
    {
        return maxShield;
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

    /// <summary>
    /// Enables or disables camera facing behavior
    /// </summary>
    public void SetAlwaysFaceCamera(bool enabled)
    {
        alwaysFaceCamera = enabled;
    }
}