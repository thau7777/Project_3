using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LightningLightController : Singleton<LightningLightController>
{
    [Header("Target")]
    [SerializeField] private Light _light;

    [Header("Strike Interval")]
    [MinMaxSlider(0, 15)]
    [SerializeField] private Vector2 _strikeInterval = new(3f, 8f);

    [Header("Flicker")]
    [MinMaxSlider(0, 10)]
    [SerializeField] private Vector2 _flickerCount = new(2, 5);
    [MinMaxSlider(0, 1)]
    [SerializeField] private Vector2 _flickLerpInDuration = new(0.01f, 0.05f);
    [MinMaxSlider(0, 1)]
    [SerializeField] private Vector2 _flickHoldDuration = new(0.03f, 0.1f);
    [MinMaxSlider(0, 1)]
    [SerializeField] private Vector2 _flickLerpOutDuration = new(0.01f, 0.05f);
    [MinMaxSlider(0, 1)]
    [SerializeField] private Vector2 _flickGapDuration = new(0.02f, 0.08f);

    [Header("Intensity")]
    [MinMaxSlider(0, 3)]
    [SerializeField] private Vector2 _intensity = new(0.8f, 2f);

    private CancellationTokenSource _cts;

    public void StartLightningLoop()
    {
        _light.enabled = true;
        _light.intensity = 0f;
        _cts = new CancellationTokenSource();
        RunLightningLoop(_cts.Token).Forget();
    }

    private async UniTaskVoid RunLightningLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(Random.Range(_strikeInterval.x, _strikeInterval.y), cancellationToken: ct);
            await StrikeAsync(ct);
        }
    }

    private async UniTask StrikeAsync(CancellationToken ct)
    {
        int flickerCount = Random.Range((int)_flickerCount.x, (int)_flickerCount.y + 1);

        for (int i = 0; i < flickerCount; i++)
        {
            float targetIntensity = Random.Range(_intensity.x, _intensity.y);

            // Lerp in
            await LerpIntensityAsync(0f, targetIntensity, Random.Range(_flickLerpInDuration.x, _flickLerpInDuration.y), ct);

            // Hold
            await UniTask.WaitForSeconds(Random.Range(_flickHoldDuration.x, _flickHoldDuration.y), cancellationToken: ct);

            // Lerp out
            await LerpIntensityAsync(targetIntensity, 0f, Random.Range(_flickLerpOutDuration.x, _flickLerpOutDuration.y), ct);

            // Gap before next flicker
            if (i < flickerCount - 1)
                await UniTask.WaitForSeconds(Random.Range(_flickGapDuration.x, _flickGapDuration.y), cancellationToken: ct);
        }
    }

    private async UniTask LerpIntensityAsync(float from, float to, float duration, CancellationToken ct)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = t * t * (3f - 2f * t);
            _light.intensity = Mathf.Lerp(from, to, smoothT);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        _light.intensity = to;
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}