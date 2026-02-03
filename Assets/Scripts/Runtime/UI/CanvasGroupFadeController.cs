using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFadeController
    : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private CancellationTokenSource fadeCts;
    private EventBinding<TopDownStartGameEvent> _startGameEventBinding;
    private EventBinding<TopDownPlayerDeadEvent> _playerDeadEventBinding;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>(); 
    }
    private void OnEnable()
    {
        canvasGroup.alpha = 0;

        _startGameEventBinding = new EventBinding<TopDownStartGameEvent>(StartGame);
        EventBus<TopDownStartGameEvent>.Register(_startGameEventBinding);

        _playerDeadEventBinding = new EventBinding<TopDownPlayerDeadEvent>(EndGame);
        EventBus<TopDownPlayerDeadEvent>.Register(_playerDeadEventBinding);
    }

    private void OnDisable()
    {
        EventBus<TopDownStartGameEvent>.Deregister(_startGameEventBinding);
        EventBus<TopDownPlayerDeadEvent>.Deregister(_playerDeadEventBinding);
    }

    private void OnDestroy()
    {
        fadeCts?.Cancel();
        fadeCts?.Dispose();
    }

    private void StartGame()
    {
        FadeIn(1f);
    }
    private void EndGame()
    {
        FadeOut(1f);
    }

    /// <summary>
    /// Fades the CanvasGroup alpha from current value to 1
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds</param>
    public async UniTask FadeIn(float duration)
    {
        await Fade(canvasGroup.alpha, 1f, duration);
    }

    /// <summary>
    /// Fades the CanvasGroup alpha from current value to 0
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds</param>
    public async UniTask FadeOut(float duration)
    {
        await Fade(canvasGroup.alpha, 0f, duration);
    }

    private async UniTask Fade(float startAlpha, float targetAlpha, float duration)
    {
        // Cancel any existing fade operation
        fadeCts?.Cancel();
        fadeCts?.Dispose();
        fadeCts = new CancellationTokenSource();

        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        try
        {
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                await UniTask.Yield(PlayerLoopTiming.Update, fadeCts.Token);
            }

            canvasGroup.alpha = targetAlpha;
        }
        catch (System.OperationCanceledException)
        {
            // Fade was cancelled, this is expected behavior
        }
    }
}