using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalProjectorController : Flyweight
{
    private DecalProjector _decalProjector;
    private Coroutine _fadeCoroutine;
    private Material _materialInstance; // Store the unique material instance

    void Awake()
    {
        _decalProjector = GetComponent<DecalProjector>();
        if (_decalProjector == null)
        {
            Debug.LogError("DecalProjector component not found!");
        }
        else
        {
            // Create a unique material instance for this DecalProjector
            _materialInstance = new Material(_decalProjector.material);
            _decalProjector.material = _materialInstance;
        }
    }

    private void OnEnable()
    {
        ResetAllStat();
    }

    private void OnDestroy()
    {
        // Clean up the material instance to prevent memory leaks
        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
        }
    }

    // Set width of the decal projection
    public void SetWidth(float width)
    {
        if (_decalProjector == null) return;

        Vector3 size = _decalProjector.size;
        size.x = width;
        _decalProjector.size = size;
    }

    // Set height of the decal projection
    public void SetHeight(float height)
    {
        if (_decalProjector == null) return;

        Vector3 size = _decalProjector.size;
        size.y = height;
        _decalProjector.size = size;
    }

    // Set both width and height
    public void SetSize(float width, float height)
    {
        if (_decalProjector == null) return;

        Vector3 size = _decalProjector.size;
        size.x = width;
        size.y = height;
        _decalProjector.size = size;
    }

    // Set projection depth
    public void SetDepth(float depth)
    {
        if (_decalProjector == null) return;

        Vector3 size = _decalProjector.size;
        size.z = depth;
        _decalProjector.size = size;
    }

    // Change the decal material
    public void SetMaterial(Material newMaterial)
    {
        if (_decalProjector == null || newMaterial == null) return;

        // Clean up old material instance
        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
        }

        // Create new material instance
        _materialInstance = new Material(newMaterial);
        _decalProjector.material = _materialInstance;
    }

    // Set opacity immediately (0 to 1) - Uses DecalProjector's fadeFactor
    public void SetOpacity(float opacity)
    {
        if (_decalProjector == null) return;

        opacity = Mathf.Clamp01(opacity);
        _decalProjector.fadeFactor = opacity;
    }

    // Fade from current opacity to target opacity over duration
    public void FadeTo(float targetOpacity, float duration)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeCoroutine(targetOpacity, duration));
    }

    // Fade out (opacity 1 -> 0) over duration
    public void FadeOut(float duration)
    {
        FadeTo(0f, duration);
    }

    // Fade in (opacity 0 -> 1) over duration
    public void FadeIn(float duration)
    {
        FadeTo(1f, duration);
    }

    private IEnumerator FadeCoroutine(float targetOpacity, float duration)
    {
        if (_decalProjector == null) yield break;

        float startOpacity = _decalProjector.fadeFactor;
        float startFadeAmount = _materialInstance != null && _materialInstance.HasProperty("_FadeAmount")
            ? _materialInstance.GetFloat("_FadeAmount")
            : 1f - startOpacity; // FadeAmount is inverse of opacity
        float targetFadeAmount = 1f - targetOpacity; // When opacity=0, FadeAmount=1

        float elapsed = 0f;
        float fadeAmountElapsed = 0f;
        float fadeAmountDuration = duration * 3f; // Make FadeAmount 50% slower

        while (elapsed < duration || fadeAmountElapsed < fadeAmountDuration)
        {
            elapsed += Time.deltaTime;
            fadeAmountElapsed += Time.deltaTime;

            // Update DecalProjector's fade factor (normal speed): 1 -> 0 when fading out
            if (elapsed < duration)
            {
                float t = elapsed / duration;
                float currentOpacity = Mathf.Lerp(startOpacity, targetOpacity, t);
                _decalProjector.fadeFactor = currentOpacity;
            }
            else
            {
                _decalProjector.fadeFactor = targetOpacity;
            }

            // Update material's FadeAmount (slower speed): 0 -> 1 when fading out
            if (fadeAmountElapsed < fadeAmountDuration)
            {
                float tFade = fadeAmountElapsed / fadeAmountDuration;
                float currentFadeAmount = Mathf.Lerp(startFadeAmount, targetFadeAmount, tFade);

                if (_materialInstance != null && _materialInstance.HasProperty("_FadeAmount"))
                {
                    _materialInstance.SetFloat("_FadeAmount", currentFadeAmount);
                }
            }
            else
            {
                if (_materialInstance != null && _materialInstance.HasProperty("_FadeAmount"))
                {
                    _materialInstance.SetFloat("_FadeAmount", targetFadeAmount);
                }
            }

            yield return null;
        }

        // Ensure final values are set
        _decalProjector.fadeFactor = targetOpacity;
        if (_materialInstance != null && _materialInstance.HasProperty("_FadeAmount"))
        {
            _materialInstance.SetFloat("_FadeAmount", targetFadeAmount);
        }

        if (targetOpacity == 0)
            ReturnToPool();
    }

    private void ResetAllStat()
    {
        SetOpacity(1);
        if (_materialInstance != null && _materialInstance.HasProperty("_FadeAmount"))
        {
            _materialInstance.SetFloat("_FadeAmount", 0);
        }
    }
}