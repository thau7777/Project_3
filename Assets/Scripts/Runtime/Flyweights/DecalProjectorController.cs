using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalProjectorController : Flyweight
{
    private DecalProjector _decalProjector;
    private Coroutine _fadeCoroutine;

    void Awake()
    {
        _decalProjector = GetComponent<DecalProjector>();
        if (_decalProjector == null)
        {
            Debug.LogError("DecalProjector component not found!");
        }
    }
    private void OnEnable()
    {
        _decalProjector.fadeFactor = 1;
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

        _decalProjector.material = newMaterial;
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
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentOpacity = Mathf.Lerp(startOpacity, targetOpacity, t);
            _decalProjector.fadeFactor = currentOpacity;

            yield return null;
        }

        // Ensure final value is set
        _decalProjector.fadeFactor = targetOpacity;
        if(targetOpacity == 0)
            ReturnToPool();
    }
}