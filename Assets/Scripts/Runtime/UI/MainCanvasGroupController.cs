using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public enum UIEndGameExecuteState 
{
    Both,
    Win,
    Lose
}


[RequireComponent(typeof(CanvasGroup))]
public class MainCanvasGroupController : MonoBehaviour
{
    [SerializeField] private bool _showWhenEndGame = false;
    [SerializeField, ShowIf("_showWhenEndGame")] private UIEndGameExecuteState _endGameExecuteState;
    [SerializeField, TabGroup("FadeInSettings")] private float _delayBeforeFadeIn = 0;
    [SerializeField, TabGroup("FadeInSettings")] private float fadeInDuration = 1f;

    [SerializeField, TabGroup("FadeOutSettings"), ShowIf("_isEndGameUI", true)] private float _delayBeforeFadeOut = 0;
    [SerializeField, TabGroup("FadeOutSettings"), ShowIf("_isEndGameUI", true)] private float fadeOutDuration = 1f;

    private CanvasGroup canvasGroup;
    private CancellationTokenSource fadeCts;

    private EventBinding<TopdownStartGameEvent> _startGameEventBinding;
    private EventBinding<TopDownEndGameEvent> _endGameEventBinding;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        canvasGroup.alpha = 0;

        if (!_showWhenEndGame)
        {
            _startGameEventBinding = new EventBinding<TopdownStartGameEvent>(FadeIn);
            EventBus<TopdownStartGameEvent>.Register(_startGameEventBinding);
            _endGameEventBinding = new EventBinding<TopDownEndGameEvent>(FadeOut);
            EventBus<TopDownEndGameEvent>.Register(_endGameEventBinding);
            return;
        }

        _endGameEventBinding = new(OnEndGame);
        EventBus<TopDownEndGameEvent>.Register(_endGameEventBinding);

    }

    private void OnDisable()
    {
        if (!_showWhenEndGame)
            EventBus<TopdownStartGameEvent>.Deregister(_startGameEventBinding);
        EventBus<TopDownEndGameEvent>.Deregister(_endGameEventBinding);
    }

    private void OnDestroy()
    {
        fadeCts?.Cancel();
        fadeCts?.Dispose();
    }
    private void OnEndGame(TopDownEndGameEvent topDownEndGameEvent)
    {
        if (topDownEndGameEvent.endGameExecuteState == _endGameExecuteState || _endGameExecuteState == UIEndGameExecuteState.Both)
            FadeIn();
    }
    private async void FadeIn()
    {
        await FadeIn(fadeInDuration);
    }

    private async void FadeOut()
    {
        await FadeOut(fadeOutDuration);
    }

    /// <summary>
    /// Fades the CanvasGroup alpha from current value to 1
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds</param>
    public async UniTask FadeIn(float duration)
    {
        // Cancel any existing fade operation
        fadeCts?.Cancel();
        fadeCts?.Dispose();
        fadeCts = new CancellationTokenSource();

        try
        {
            // Apply delay before fading in
            if (_delayBeforeFadeIn > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeFadeIn), cancellationToken: fadeCts.Token);
            }
            await Fade(canvasGroup.alpha, 1f, duration, fadeCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Fade was cancelled, this is expected behavior
        }
    }

    /// <summary>
    /// Fades the CanvasGroup alpha from current value to 0
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds</param>
    public async UniTask FadeOut(float duration)
    {
        // Cancel any existing fade operation
        fadeCts?.Cancel();
        fadeCts?.Dispose();
        fadeCts = new CancellationTokenSource();

        try
        {
            // Apply delay before fading out
            if (_delayBeforeFadeOut > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeFadeOut), cancellationToken: fadeCts.Token);
            }

            await Fade(canvasGroup.alpha, 0f, duration, fadeCts.Token);
        }
        catch (OperationCanceledException)
        {
            // Fade was cancelled, this is expected behavior
        }
    }

    private async UniTask Fade(float startAlpha, float targetAlpha, float duration, CancellationToken token)
    {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        canvasGroup.alpha = targetAlpha;
    }
}