using UnityEngine;
using UnityEngine.VFX;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.Rendering;

public class TopdownWarpDriveController : MonoBehaviour
{
    [SerializeField] private Volume TPVolume;
    [SerializeField] private VisualEffect _visualEffect;

    [SerializeField, TabGroup("Warp Settings")] private float _waitDuration = 1f;
    [SerializeField, TabGroup("Warp Settings")] private float _fadeInDuration = 1f;
    [SerializeField, TabGroup("Warp Settings")] private float _holdDuration = 2f;
    [SerializeField, TabGroup("Warp Settings")] private float _fadeOutDuration = 1f;

    private Material _firstCylinderMaterial;
    private Material _secondCylinderMaterial;
    private CancellationTokenSource _cts;

    private float _activeAmount = 0;
    private float _vfxDuration;
    public float ActiveAmount
    {
        get { return _activeAmount; }
        set
        {
            _activeAmount = value;
            _visualEffect.SetFloat("WarpAmount", value);
            _firstCylinderMaterial.SetFloat("_Active", value);
            _secondCylinderMaterial.SetFloat("_Active", value);

            TPVolume.weight = value;
        }
    }

    private void Awake()
    {
        _visualEffect = GetComponent<VisualEffect>();
        _firstCylinderMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().material = new Material(transform.GetChild(0).GetComponent<MeshRenderer>().material);
        _secondCylinderMaterial = transform.GetChild(1).GetComponent<MeshRenderer>().material = new Material(transform.GetChild(1).GetComponent<MeshRenderer>().material);
        ActiveAmount = 0;

        PlayWarpSequence().Forget();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        Destroy(_firstCylinderMaterial);
        Destroy(_secondCylinderMaterial);
    }

    /// <summary>Runs the full warp sequence: fade in → hold → fade out.</summary>
    public async UniTask PlayWarpSequence()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_waitDuration), cancellationToken: _cts.Token);
            await Tween(0f, 1f, _fadeInDuration, _cts.Token);
            await UniTask.Delay(TimeSpan.FromSeconds(_holdDuration), cancellationToken: _cts.Token);
            await Tween(1f, 0f, _fadeOutDuration, _cts.Token);
            await UniTask.Delay(TimeSpan.FromSeconds(_fadeInDuration + _holdDuration + _fadeOutDuration + 5), cancellationToken: _cts.Token);
            gameObject.SetActive(false);
        }
        catch (OperationCanceledException) { }
    }

    /// <summary>Cancels any running sequence and immediately resets to 0.</summary>
    public void StopWarp()
    {
        _cts?.Cancel();
        ActiveAmount = 0f;
    }

    private async UniTask Tween(float from, float to, float duration, CancellationToken token)
    {
        float elapsed = 0f;
        ActiveAmount = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            ActiveAmount = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        ActiveAmount = to;
    }
}