using System.Collections;
using UnityEngine;
public class EmissionController : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private string propertyName = "_FlashValue";
    [SerializeField] private float _holdFlashDuration = 0.1f;
    [SerializeField] private float _turnOffFlashDuration = 0.1f;

    private MaterialPropertyBlock _mpb;
    private Coroutine _holdFlashCoroutine;
    private Coroutine _flashingCoroutine;

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();
    }
    private void OnDisable()
    {
        if(_holdFlashCoroutine != null)
        {
            StopCoroutine(_holdFlashCoroutine);
            _holdFlashCoroutine = null;
        }
        if (_flashingCoroutine != null)
        {
            StopCoroutine(_flashingCoroutine);
            _flashingCoroutine = null;
        }

    }
    public void StartFlash()
    {
        if (_holdFlashCoroutine != null)
        {
            StopCoroutine(_holdFlashCoroutine);
            _holdFlashCoroutine = null;
        }
        if (_flashingCoroutine != null)
        {
            StopCoroutine(_flashingCoroutine);
        }

        _holdFlashCoroutine = StartCoroutine(StartFlashing(_holdFlashDuration));

    }

    private IEnumerator StartFlashing(float holdDuration)
    {
        // Set to 1 immediately
        SetEmission(1f);
        float elapsed = 0f;
        while (elapsed < holdDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        _flashingCoroutine = StartCoroutine(Helpers.LerpValue<float>(1f, 0f, _turnOffFlashDuration, Mathf.Lerp, SetEmission));

    }

    public void SetEmission(float value)
    {
        targetRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(propertyName, value);
        targetRenderer.SetPropertyBlock(_mpb);
    }
}
