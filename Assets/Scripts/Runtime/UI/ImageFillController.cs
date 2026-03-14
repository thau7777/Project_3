using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

[RequireComponent(typeof(Image))]
public class ImageFillController : MonoBehaviour
{
    [SerializeField] private bool _useMaterial = false;
    [SerializeField] private bool _showWhenEndGame = false;
    [SerializeField, ShowIf("_showWhenEndGame")] private UIEndGameExecuteState _endGameExecuteState;

    [SerializeField, TabGroup("FillInSettings")] private float _delayBeforeFillIn = 0f;
    [SerializeField, TabGroup("FillInSettings")] private float _fillInDuration = 1f;

    [SerializeField, TabGroup("FillOutSettings"), ShowIf("_showWhenEndGame", true)] private float _delayBeforeFillOut = 0f;
    [SerializeField, TabGroup("FillOutSettings"), ShowIf("_showWhenEndGame", true)] private float _fillOutDuration = 1f;

    private float FillAmount
    {
        get 
        {
            if(!_useMaterial)
                return _image.fillAmount;
            return _material.GetFloat("_FillAmount");
        }
        set
        {
            if(!_useMaterial)
                _image.fillAmount = value;
            else
                _material.SetFloat("_FillAmount", value);
        }
    }

    private Image _image;
    private CancellationTokenSource _fillCts;

    private EventBinding<TopdownStartGameEvent> _startGameEventBinding;
    private EventBinding<TopDownEndGameEvent> _endGameEventBinding;
    private EventBinding<TopdownOnEndGameContinueEvent> _onEndGameContinueEventBinding;

    private Material _material;
    private void Awake()
    {
        _image = GetComponent<Image>();

        if (_useMaterial)
            _material = _image.material = new Material(_image.material);

        FillAmount = 0f;
    }

    private void OnEnable()
    {
        FillAmount = 0f;

        if (!_showWhenEndGame)
        {
            _startGameEventBinding = new EventBinding<TopdownStartGameEvent>(FillIn);
            EventBus<TopdownStartGameEvent>.Register(_startGameEventBinding);
            _endGameEventBinding = new EventBinding<TopDownEndGameEvent>(FillOut);
            EventBus<TopDownEndGameEvent>.Register(_endGameEventBinding);
            return;
        }

        _endGameEventBinding = new(OnEndGame);
        EventBus<TopDownEndGameEvent>.Register(_endGameEventBinding);
        _onEndGameContinueEventBinding = new(FillOut);
        EventBus<TopdownOnEndGameContinueEvent>.Register(_onEndGameContinueEventBinding);
        
    }

    private void OnDisable()
    {
        if (!_showWhenEndGame)
            EventBus<TopdownStartGameEvent>.Deregister(_startGameEventBinding);

        EventBus<TopDownEndGameEvent>.Deregister(_endGameEventBinding);

        if(_showWhenEndGame)
            EventBus<TopdownOnEndGameContinueEvent>.Deregister(_onEndGameContinueEventBinding);
    }

    private void OnDestroy()
    {
        if(_material)
            Destroy(_material);
        _fillCts?.Cancel();
        _fillCts?.Dispose();
    }
    private void OnEndGame(TopDownEndGameEvent topDownEndGameEvent)
    {
        if (topDownEndGameEvent.endGameExecuteState == _endGameExecuteState || _endGameExecuteState == UIEndGameExecuteState.Both)
            FillIn();
    }
    private async void FillIn()
    {
        await FillIn(_fillInDuration);
    }

    private async void FillOut()
    {
        await FillOut(_fillOutDuration);
    }

    /// <summary>Fills the image from its current fill amount to 1.</summary>
    public async UniTask FillIn(float duration)
    {
        _fillCts?.Cancel();
        _fillCts?.Dispose();
        _fillCts = new CancellationTokenSource();

        try
        {
            if (_delayBeforeFillIn > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeFillIn), cancellationToken: _fillCts.Token);

            await Fill(FillAmount, 1f, duration, _fillCts.Token);
        }
        catch (OperationCanceledException) { }
    }

    /// <summary>Fills the image from its current fill amount to 0.</summary>
    public async UniTask FillOut(float duration)
    {
        _fillCts?.Cancel();
        _fillCts?.Dispose();
        _fillCts = new CancellationTokenSource();

        try
        {
            if (_delayBeforeFillOut > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeFillOut), cancellationToken: _fillCts.Token);

            await Fill(FillAmount, 0f, duration, _fillCts.Token);
        }
        catch (OperationCanceledException) { }
    }

    private async UniTask Fill(float startFill, float targetFill, float duration, CancellationToken token)
    {
        float elapsed = 0f;
        FillAmount = startFill;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            FillAmount = Mathf.Lerp(startFill, targetFill, t);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        FillAmount = targetFill;
    }
}