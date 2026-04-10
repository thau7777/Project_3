using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class TopDownEnemyUIController : MonoBehaviour
{
    [TabGroup("Bar References")]
    [SerializeField] private Slider healthBarSlider;
    [TabGroup("Bar References")]
    [SerializeField] private Image healthBarBackFill;
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
    [SerializeField] private Color healthBarBackFillHurtColor = Color.white;

    [TabGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillDelayDuration = 0.5f;
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private float shieldBarBackFillAnimationDuration = 0.5f;
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private AnimationCurve shieldBarBackFillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [TabGroup("Shield Bar Settings")]
    [SerializeField] private Color shieldBarBackFillHurtColor = Color.cyan;

    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private Image shieldBreakObj;
    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private float targetshieldBreakScale;
    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private float shieldBreakScaleDuration = 0.8f;
    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private float shieldBreakFadeInDuration = 0.15f;
    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private float shieldBreakHoldDuration = 0.25f;
    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private float shieldBreakFadeOutDuration = 0.4f;

    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private Image shieldBreakBackGlow;
    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private float shieldBreakBackGlowEmissionLerpDuration = 0.6f;
    [TabGroup("Shield Bar Break Settings")]
    [SerializeField] private float shieldBreakBackGlowTargetScale = 2.5f;

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

    private CancellationTokenSource healthBackFillCTS;
    private CancellationTokenSource shieldBackFillCTS;

    private Vector3 shieldBreakOriginalScale;
    private bool isShieldBreakPlaying;

    private Material shieldBreakMaterial; 
    private Material shieldBreakBackGlowMaterial;
    private Vector3 shieldBreakBackGlowOriginalScale;
    private float shieldBreakBackGlowOriginalEmission;

    private void Awake()
    {
        if (shieldBreakObj != null)
        {
            shieldBreakOriginalScale = shieldBreakObj.transform.localScale;
            shieldBreakMaterial = new Material(shieldBreakObj.material);
            shieldBreakObj.material = shieldBreakMaterial;
            shieldBreakMaterial.SetFloat("_NoisePower", 1f);
            shieldBreakObj.gameObject.SetActive(false);
        }

        if (shieldBreakBackGlow != null)
        {
            shieldBreakBackGlowOriginalScale = shieldBreakBackGlow.transform.localScale;
            shieldBreakBackGlowMaterial = new Material(shieldBreakBackGlow.material);
            shieldBreakBackGlow.material = shieldBreakBackGlowMaterial;
            shieldBreakBackGlowOriginalEmission = shieldBreakBackGlowMaterial.GetFloat("_Emission");
            shieldBreakBackGlow.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        healthBackFillCTS?.Cancel();
        healthBackFillCTS?.Dispose();
        shieldBackFillCTS?.Cancel();
        shieldBackFillCTS?.Dispose();

        if (shieldBreakMaterial != null)
            Destroy(shieldBreakMaterial);
        if (shieldBreakBackGlowMaterial != null)
            Destroy(shieldBreakBackGlowMaterial);
    }

    public void InitializeValue(float maxHealth, float maxShield)
    {
        currentHealth = maxHealth;
        this.maxHealth = maxHealth;
        currentShield = maxShield;
        this.maxShield = maxShield;

        healthBarSlider.value = 1;
        healthBarBackFill.fillAmount = 1;

        shieldBarSlider.value = 1;
        shieldBarBackFill.fillAmount = 1;
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
            SetHealthBarBackFillColor(healthBarBackFillHurtColor);

            healthBackFillCTS?.Cancel();
            healthBackFillCTS?.Dispose();
            healthBackFillCTS = new CancellationTokenSource();
            AnimateHealthBarBackFillAsync(healthBackFillCTS.Token).Forget();
        }
        else
        {
            healthBackFillCTS?.Cancel();
            healthBackFillCTS?.Dispose();
            healthBackFillCTS = null;

            if (healthBarBackFill != null)
                healthBarBackFill.fillAmount = newValue;
        }
    }

    private async UniTaskVoid AnimateHealthBarBackFillAsync(CancellationToken ct)
    {
        if (healthBarBackFill == null || healthBarSlider == null) return;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(healthBarBackFillDelayDuration), cancellationToken: ct);

            float startFillAmount = healthBarBackFill.fillAmount;
            float targetFillAmount = healthBarSlider.value;
            float elapsed = 0f;

            while (elapsed < healthBarBackFillAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / healthBarBackFillAnimationDuration;
                healthBarBackFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, healthBarBackFillCurve.Evaluate(t));
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            healthBarBackFill.fillAmount = targetFillAmount;
        }
        catch (OperationCanceledException) { }
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
            SetShieldBarBackFillColor(shieldBarBackFillHurtColor);

            shieldBackFillCTS?.Cancel();
            shieldBackFillCTS?.Dispose();
            shieldBackFillCTS = new CancellationTokenSource();
            AnimateShieldBarBackFillAsync(shieldBackFillCTS.Token).Forget();

            if (current <= 0 && !isShieldBreakPlaying)
                PlayShieldBreakAnimationAsync().Forget();
        }
        else
        {
            shieldBackFillCTS?.Cancel();
            shieldBackFillCTS?.Dispose();
            shieldBackFillCTS = null;

            if (shieldBarBackFill != null)
                shieldBarBackFill.fillAmount = newValue;
        }
    }

    private async UniTaskVoid AnimateShieldBarBackFillAsync(CancellationToken ct)
    {
        if (shieldBarBackFill == null || shieldBarSlider == null) return;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(shieldBarBackFillDelayDuration), cancellationToken: ct);

            float startFillAmount = shieldBarBackFill.fillAmount;
            float targetFillAmount = shieldBarSlider.value;
            float elapsed = 0f;

            while (elapsed < shieldBarBackFillAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shieldBarBackFillAnimationDuration;
                shieldBarBackFill.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, shieldBarBackFillCurve.Evaluate(t));
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            shieldBarBackFill.fillAmount = targetFillAmount;
        }
        catch (OperationCanceledException) { }
    }

    private async UniTaskVoid PlayShieldBreakAnimationAsync()
    {
        if (shieldBreakObj == null) return;

        isShieldBreakPlaying = true;
        shieldBreakObj.gameObject.SetActive(true);

        shieldBreakObj.transform.localScale = shieldBreakOriginalScale;
        Vector3 targetScale = shieldBreakOriginalScale * targetshieldBreakScale;

        shieldBreakMaterial.SetFloat("_Alpha", 0f);

        bool hasBackGlow = shieldBreakBackGlow != null;
        Vector3 backGlowTargetScale = shieldBreakBackGlowOriginalScale * shieldBreakBackGlowTargetScale;
        if (hasBackGlow)
        {
            shieldBreakBackGlow.gameObject.SetActive(true);
            shieldBreakBackGlow.transform.localScale = shieldBreakBackGlowOriginalScale;
            shieldBreakBackGlowMaterial.SetFloat("_Emission", shieldBreakBackGlowOriginalEmission);
        }

        float totalFadeDuration = shieldBreakFadeInDuration + shieldBreakHoldDuration + shieldBreakFadeOutDuration;
        float scaleElapsed = 0f;
        float fadeElapsed = 0f;
        float backGlowEmissionElapsed = 0f;

        var ct = this.GetCancellationTokenOnDestroy();

        while (scaleElapsed < shieldBreakScaleDuration || fadeElapsed < totalFadeDuration)
        {
            float dt = Time.deltaTime;

            // --- Scale ---
            if (scaleElapsed < shieldBreakScaleDuration)
            {
                scaleElapsed += dt;
                float scaleT = Mathf.Clamp01(scaleElapsed / shieldBreakScaleDuration);
                shieldBreakObj.transform.localScale = Vector3.Lerp(shieldBreakOriginalScale, targetScale, scaleT);

                if (hasBackGlow)
                    shieldBreakBackGlow.transform.localScale = Vector3.Lerp(shieldBreakBackGlowOriginalScale, backGlowTargetScale, scaleT);
            }

            // --- _Alpha: fade in → hold → fade out ---
            if (fadeElapsed < totalFadeDuration)
            {
                fadeElapsed += dt;

                float alpha;
                if (fadeElapsed < shieldBreakFadeInDuration)
                {
                    alpha = fadeElapsed / shieldBreakFadeInDuration;
                }
                else if (fadeElapsed < shieldBreakFadeInDuration + shieldBreakHoldDuration)
                {
                    alpha = 1f;
                }
                else
                {
                    float fadeOutElapsed = fadeElapsed - shieldBreakFadeInDuration - shieldBreakHoldDuration;
                    alpha = 1f - Mathf.Clamp01(fadeOutElapsed / shieldBreakFadeOutDuration);
                }

                shieldBreakMaterial.SetFloat("_Alpha", alpha);
            }

            // --- Backglow _Emission: og → 0 ---
            if (hasBackGlow && backGlowEmissionElapsed < shieldBreakBackGlowEmissionLerpDuration)
            {
                backGlowEmissionElapsed += dt;
                float emissionT = Mathf.Clamp01(backGlowEmissionElapsed / shieldBreakBackGlowEmissionLerpDuration);
                shieldBreakBackGlowMaterial.SetFloat("_Emission", Mathf.Lerp(shieldBreakBackGlowOriginalEmission, 0f, emissionT));
            }

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        shieldBreakMaterial.SetFloat("_Alpha", 0f);
        shieldBreakObj.transform.localScale = shieldBreakOriginalScale;
        shieldBreakObj.gameObject.SetActive(false);

        if (hasBackGlow)
        {
            shieldBreakBackGlowMaterial.SetFloat("_Emission", 0f);
            shieldBreakBackGlow.transform.localScale = shieldBreakBackGlowOriginalScale;
            shieldBreakBackGlow.gameObject.SetActive(false);
        }

        isShieldBreakPlaying = false;
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