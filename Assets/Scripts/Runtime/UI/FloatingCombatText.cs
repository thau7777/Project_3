using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Spawned as a world-space flyweight to display floating combat numbers
/// (damage, heals, crits, etc.) that pop, drift, and fade away.
/// </summary>

[RequireComponent(typeof(TextMeshPro))]
public class FloatingCombatText : Flyweight
{
    #region Custom Types
    public enum CombatTextType
    {
        Fire,
        Water,
        Frost,
        Lightning,
        Poison,
        Normal,
        Holy,
        Dark,
        Heal,
        ManaRegen
    }

    [System.Serializable]
    private struct TextStyle
    {
        [ColorUsage(true, true)] public Color faceColor;
        [ColorUsage(true, true)] public Color outlineColor;
        [ColorUsage(true, true)] public Color underlayColor;
    }
    #endregion

    #region Text Styles
    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle fireStyle = new TextStyle
    { faceColor = new Color(1f, 0.35f, 0.05f), outlineColor = new Color(0.5f, 0.05f, 0f), underlayColor = new Color(0.4f, 0f, 0f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle waterStyle = new TextStyle
    { faceColor = new Color(0.3f, 0.75f, 1f), outlineColor = new Color(0f, 0.25f, 0.6f), underlayColor = new Color(0f, 0.1f, 0.4f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle frostStyle = new TextStyle
    { faceColor = new Color(0.8f, 0.95f, 1f), outlineColor = new Color(0.3f, 0.6f, 0.85f), underlayColor = new Color(0.1f, 0.3f, 0.55f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle lightningStyle = new TextStyle
    { faceColor = new Color(1f, 0.95f, 0.3f), outlineColor = new Color(0.6f, 0.3f, 0f), underlayColor = new Color(0.35f, 0.2f, 0f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle poisonStyle = new TextStyle
    { faceColor = new Color(0.6f, 1f, 0.15f), outlineColor = new Color(0.15f, 0.4f, 0f), underlayColor = new Color(0.1f, 0.25f, 0f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle normalStyle = new TextStyle
    { faceColor = new Color(1f, 1f, 1f), outlineColor = new Color(0.15f, 0.15f, 0.15f), underlayColor = new Color(0f, 0f, 0f, 0.8f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle holyStyle = new TextStyle
    { faceColor = new Color(1f, 0.97f, 0.7f), outlineColor = new Color(0.7f, 0.55f, 0f), underlayColor = new Color(0.45f, 0.35f, 0f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle darkStyle = new TextStyle
    { faceColor = new Color(0.7f, 0.3f, 1f), outlineColor = new Color(0.25f, 0f, 0.5f), underlayColor = new Color(0.15f, 0f, 0.3f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle healStyle = new TextStyle
    { faceColor = new Color(0.35f, 1f, 0.45f), outlineColor = new Color(0f, 0.4f, 0.1f), underlayColor = new Color(0f, 0.2f, 0.05f, 0.85f) };

    [TabGroup("Styles")]
    [SerializeField]
    private TextStyle manaRegenStyle = new TextStyle
    { faceColor = new Color(0.4f, 0.6f, 1f), outlineColor = new Color(0.1f, 0.2f, 0.7f), underlayColor = new Color(0.05f, 0.1f, 0.45f, 0.85f) };

    #endregion

    #region Settings
    [TabGroup("Scale")]
    [SerializeField] private float normalScale = 1f;
    [TabGroup("Scale")]
    [SerializeField] private float critScale = 1.6f;
    [TabGroup("Scale")]
    [SerializeField] private float punchScale = 1.3f;

    [TabGroup("Animation Timing")]
    [SerializeField] private float popDuration = 0.12f;
    [TabGroup("Animation Timing")]
    [SerializeField] private float holdDuration = 0.25f;
    [TabGroup("Animation Timing")]
    [SerializeField] private float fadeDuration = 0.55f;

    [TabGroup("Movement")]
    [SerializeField] private float minSpeed = 1.2f;
    [TabGroup("Movement")]
    [SerializeField] private float maxSpeed = 2.5f;
    [TabGroup("Movement")]
    [SerializeField] private float maxHorizontalAngle = 70f;
    #endregion
    // ── Private ────────────────
    #region Private Fields
    private TextMeshPro _tmp;
    private Coroutine _activeAnim;

    private static readonly int FaceColorProp = Shader.PropertyToID("_FaceColor");
    private static readonly int OutlineColorProp = Shader.PropertyToID("_OutlineColor");
    private static readonly int UnderlayColorProp = Shader.PropertyToID("_UnderlayColor");
    #endregion

    new FloatingCombatTextSettings settings => (FloatingCombatTextSettings)base.settings;
    // ── Unity lifecycle ───────────────────────────────────────────────────────
    protected virtual void Awake()
    {
        _tmp = GetComponent<TextMeshPro>();
        //Init()
    }
    // ── Public API ────────────────────────────────────────────────────────────

    public void Init(string text, CombatTextType type, Vector3 originSpawnPos, bool isCrit = false)
    {
        if (_activeAnim != null)
            StopCoroutine(_activeAnim);

        transform.position = originSpawnPos;
        transform.localScale = Vector3.zero;

        TextStyle style = GetStyle(type);
        _tmp.text = text; // ← assign directly
        ApplyColors(style.faceColor, style.outlineColor, style.underlayColor, 1f);

        float targetScale = isCrit ? critScale : normalScale;
        Vector3 driftDir = GetRandomUpwardDirection();
        float speed = Random.Range(minSpeed, maxSpeed);

        _activeAnim = StartCoroutine(AnimateRoutine(style, targetScale, driftDir, speed));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private TextStyle GetStyle(CombatTextType type) => type switch
    {
        CombatTextType.Fire => fireStyle,
        CombatTextType.Water => waterStyle,
        CombatTextType.Frost => frostStyle,
        CombatTextType.Lightning => lightningStyle,
        CombatTextType.Poison => poisonStyle,
        CombatTextType.Normal => normalStyle,
        CombatTextType.Holy => holyStyle,
        CombatTextType.Dark => darkStyle,
        CombatTextType.Heal => healStyle,
        CombatTextType.ManaRegen => manaRegenStyle,
        _ => normalStyle
    };


    /// <summary>
    /// Sets Face, Outline, and Underlay on the per-instance material.
    /// The alpha parameter fades all three in lock-step during the fade phase.
    /// HDR intensity is baked into the Color values set in the Inspector —
    /// Unity stores it as RGB values above 1.0 when you raise the intensity slider.
    /// </summary>
    private void ApplyColors(Color face, Color outline, Color underlay, float alpha)
    {
        Material mat = _tmp.fontMaterial;
        mat.SetColor(FaceColorProp, new Color(face.r, face.g, face.b, face.a * alpha));
        mat.SetColor(OutlineColorProp, new Color(outline.r, outline.g, outline.b, outline.a * alpha));
        // Underlay keeps its own base alpha, just scaled down as the number fades
        mat.SetColor(UnderlayColorProp, new Color(underlay.r, underlay.g, underlay.b, underlay.a * alpha));
    }

    private Vector3 GetRandomUpwardDirection()
    {
        float radians = Random.Range(-maxHorizontalAngle, maxHorizontalAngle) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0f).normalized;
    }

    // ── Animation coroutine ───────────────────────────────────────────────────

    private IEnumerator AnimateRoutine(TextStyle style, float targetScale, Vector3 driftDir, float speed)
    {
        float punchPeak = targetScale * punchScale;
        float elapsed = 0f;

        // ── 1. Pop in ────────────────────────────────────────────────────────
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;

            float overshoot = Mathf.Sin(t * Mathf.PI) * (punchPeak - targetScale);
            transform.localScale = Vector3.one * (Mathf.Lerp(0f, targetScale, t) + overshoot);
            transform.position += driftDir * (speed * Time.deltaTime);
            yield return null;
        }

        transform.localScale = Vector3.one * targetScale;

        // ── 2. Hold ──────────────────────────────────────────────────────────
        elapsed = 0f;
        while (elapsed < holdDuration)
        {
            elapsed += Time.deltaTime;
            transform.position += driftDir * (speed * Time.deltaTime);
            yield return null;
        }

        // ── 3. Fade out ───────────────────────────────────────────────────────
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, t * t); // ease-in

            ApplyColors(style.faceColor, style.outlineColor, style.underlayColor, alpha);

            transform.position += driftDir * (speed * (1f - t) * Time.deltaTime);
            yield return null;
        }

        // ── 4. Return to pool ─────────────────────────────────────────────────
        ReturnToPool();
    }
}