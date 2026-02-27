using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;
using System.Threading;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2f;

    // Store original values - set these in Inspector to your desired defaults
    [Header("Default Values")]
    [SerializeField] private float defaultExposure = 0f;
    [SerializeField] private float defaultSaturation = 0f;

    private ColorAdjustments colorAdjustments;
    private CancellationTokenSource cancellationTokenSource;

    private EventBinding<TopDownPlayerDeadEvent> _playerDeadEventBinding;
    private void OnEnable()
    {
        _playerDeadEventBinding = new EventBinding<TopDownPlayerDeadEvent>(TriggerDeathEffect);
        EventBus<TopDownPlayerDeadEvent>.Register(_playerDeadEventBinding);
    }
    private void OnDisable()
    {
        EventBus<TopDownPlayerDeadEvent>.Deregister(_playerDeadEventBinding);
    }
    void Awake()
    {
        // Get color adjustments from volume
        GetComponent<Volume>().profile.TryGet(out colorAdjustments);

        // Reset to original values every time scene starts
        ResetToDefault();
    }

    void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    public void ResetToDefault()
    {
        colorAdjustments.postExposure.value = defaultExposure;
        colorAdjustments.saturation.value = defaultSaturation;
    }

    public async void TriggerDeathEffect()
    {
        await FadeToBlack(cancellationTokenSource.Token);
    }

    private async UniTask FadeToBlack(CancellationToken cancellationToken)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Fade from default to black
            colorAdjustments.postExposure.value = Mathf.Lerp(defaultExposure, -10f, t);
            colorAdjustments.saturation.value = Mathf.Lerp(defaultSaturation, -100f, t);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        // Ensure final values
        colorAdjustments.postExposure.value = -10f;
        colorAdjustments.saturation.value = -100f;
    }
}