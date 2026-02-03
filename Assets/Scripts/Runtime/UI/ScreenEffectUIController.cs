using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectUIController : MonoBehaviour
{
    [TabGroup("Damaged Effect")]
    [SerializeField]
    private Image _damagedImage;
    [TabGroup("Damaged Effect")]
    [SerializeField]
    private float _turnOnDuration = 0.2f;
    [TabGroup("Damaged Effect")]
    [SerializeField]
    private float _holdDuration = 0.2f;
    [TabGroup("Damaged Effect")]
    [SerializeField]
    private float _turnOffDuration = 0.2f;
    [SerializeField]
    private Image _frozenImage;
    [SerializeField]
    private Image _burnImage;

    private CancellationTokenSource _damagedEffectCts;

    public async void OnDamaged()
    {
        // Cancel the previous effect if it's running
        _damagedEffectCts?.Cancel();
        _damagedEffectCts?.Dispose();

        // Create new cancellation token
        _damagedEffectCts = new CancellationTokenSource();

        _damagedImage.gameObject.SetActive(true);

        try
        {
            await LerpDamagedEffect(_damagedEffectCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Effect was cancelled, this is expected
        }
    }

    private async UniTask LerpDamagedEffect(CancellationToken ct)
    {
        float elapsed = 0f;

        // Lerp from 0 to 1 (Turn On)
        while (elapsed < _turnOnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _turnOnDuration;
            _damagedImage.material.SetFloat("_FillAmount", Mathf.Lerp(0f, 1f, t));
            await UniTask.Yield(ct);
        }

        // Ensure we hit exactly 1
        _damagedImage.material.SetFloat("_FillAmount", 1f);

        // Hold at 1
        await UniTask.Delay(System.TimeSpan.FromSeconds(_holdDuration), cancellationToken: ct);

        elapsed = 0f;

        // Lerp from 1 back to 0 (Turn Off)
        while (elapsed < _turnOffDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _turnOffDuration;
            _damagedImage.material.SetFloat("_FillAmount", Mathf.Lerp(1f, 0f, t));
            await UniTask.Yield(ct);
        }

        // Ensure we hit exactly 0 and deactivate
        _damagedImage.material.SetFloat("_FillAmount", 0f);
        _damagedImage.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _damagedEffectCts?.Cancel();
        _damagedEffectCts?.Dispose();
    }
}