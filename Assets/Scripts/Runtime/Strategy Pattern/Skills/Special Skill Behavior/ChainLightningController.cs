using UnityEngine;
using System.Collections;

// ============= LIGHTNING BOLT COMPONENT =============
public class ChainLightningController : OneShotVFX
{
    public LineRenderer lineRenderer;

    private Transform startTarget;
    private Transform endTarget;
    private int segments;
    private float arcIntensity;
    private float[] randomOffsets;

    private Material _runtimeMat;
    private Color _ogHDRColor;
    private Coroutine _fadeCoroutine;

    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            // Creates instance material (important for pooling)
            _runtimeMat = lineRenderer.material;

            if (_runtimeMat.HasProperty(ColorID))
                _ogHDRColor = _runtimeMat.GetColor(ColorID);
        }
    }

    private void OnEnable()
    {
        if (_runtimeMat != null && _runtimeMat.HasProperty(ColorID))
        {
            // Restore original HDR color
            _runtimeMat.SetColor(ColorID, _ogHDRColor);
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }

        startTarget = null;
        endTarget = null;
    }

    public void InitializeLine(
        Transform start,
        Transform end,
        int segmentCount,
        float intensity)
    {
        startTarget = start;
        endTarget = end;
        segments = segmentCount;
        arcIntensity = intensity;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = segments;
        }

        randomOffsets = new float[segments];
        for (int i = 0; i < segments; i++)
        {
            randomOffsets[i] = Random.Range(-arcIntensity, arcIntensity);
        }
    }

    private void Update()
    {
        if (!startTarget || !endTarget || !lineRenderer)
        {
            Destroy(gameObject);
            return;
        }

        UpdateLightning();
    }

    private void UpdateLightning()
    {
        Vector3 start = startTarget.position;
        Vector3 end = endTarget.position;

        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(segments - 1, end);

        for (int i = 1; i < segments - 1; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 pos = Vector3.Lerp(start, end, t);

            pos += perpendicular * randomOffsets[i];
            pos.y += Random.Range(
                -arcIntensity * 0.3f,
                 arcIntensity * 0.3f
            );

            lineRenderer.SetPosition(i, pos);
        }
    }

    // ================= HDR COLOR FADE =================
    public void StartFadeOut(float duration)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        if (_runtimeMat == null || !_runtimeMat.HasProperty(ColorID))
            yield break;

        float time = 0f;
        Color startColor = _runtimeMat.GetColor(ColorID);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Lerp HDR color to black (keeps HDR behavior)
            Color faded = Color.Lerp(startColor, Color.black, t);
            _runtimeMat.SetColor(ColorID, faded);

            yield return null;
        }

        _runtimeMat.SetColor(ColorID, Color.black);
        ReturnToPool();
    }
}
