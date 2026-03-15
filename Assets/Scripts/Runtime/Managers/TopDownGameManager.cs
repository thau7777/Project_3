using Cysharp.Threading.Tasks;
using MyRule;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class TopDownGameManager : Singleton<TopDownGameManager>
{
    [SerializeField, TabGroup("References")]
    private InputReader _inputReader;

    [SerializeField, TabGroup("References")]
    private GameObject _player;

    [SerializeField, TabGroup("References")]
    private GameObject _tpEffect;

    [SerializeField, TabGroup("References")]
    private Volume _lowHealthVolume;

    [SerializeField, TabGroup("References")]
    private Volume _deathVolume;

    [SerializeField, TabGroup("References")]
    private Volume _parryVolume;

    private bool _slowMoCooldownReady = true;
    [SerializeField, TabGroup("SlowMotionSettings")] private float _slowMoCooldown = 5f;
    [SerializeField, TabGroup("SlowMotionSettings")] private float _slowMoDuration = 0.5f;
    [SerializeField, TabGroup("SlowMotionSettings")] private float _slowMoFadeInDuration = 0.4f;
    [SerializeField, TabGroup("SlowMotionSettings")] private float _slowMoFadeOutDuration = 0.2f;
    [SerializeField, TabGroup("SlowMotionSettings")] private float _slowMoTimeScale = 0.2f;

    [SerializeField, TabGroup("ParrySettings")] private float _parryVolumeLerpInDuration = 0.2f;
    [SerializeField, TabGroup("ParrySettings")] private float _parryVolumeLerpOutDuration = 0.2f;

    [SerializeField, TabGroup("LowHealthEffectSettings")] private float _lowHealthLerpInDuration = 0.5f;
    [SerializeField, TabGroup("LowHealthEffectSettings")] private float _lowHealthLerpOutDuration = 0.5f;

    private bool _isFlashing = false;
    private bool _isWinning;
    private bool _continueBtnClickable;
    private int _damageReceived;
    private int _parriedDamage;
    private int _damageDealt;

    private bool _isTriggeringLowHealthEffect;

    private CancellationTokenSource cancellationTokenSource; 
    private CancellationTokenSource _lowHealthCTS;

    private EventBinding<TopDownEndGameEvent> _topdownEndGameEventBinding;
    private void OnEnable()
    {
        ResetEndGameInfo();
        _topdownEndGameEventBinding = new EventBinding<TopDownEndGameEvent>(OnEndGame);
        EventBus<TopDownEndGameEvent>.Register(_topdownEndGameEventBinding);

        _inputReader.playerTopDownActions.onSkillUse += OnContinueBtn;
    }


    private void OnDisable()
    {
        EventBus<TopDownEndGameEvent>.Deregister(_topdownEndGameEventBinding);
        _inputReader.playerTopDownActions.onSkillUse -= OnContinueBtn;

    }
    private void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        _continueBtnClickable = false;
        if(_deathVolume)
            _deathVolume.weight = 0f;
        StartMatch();

    }

    void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        _lowHealthCTS?.Cancel();      // add these
        _lowHealthCTS?.Dispose();     // add these
    }
    private async void StartMatch()
    {
        await UniTask.Delay(3000);
        EnableTPEffect();
        await UniTask.Delay(700);
        EnablePlayer();
        await UniTask.Delay(3000);
        EventBus<TopdownStartGameEvent>.Raise(new TopdownStartGameEvent());
    }
    private void EnablePlayer()
    {
        if (_player)
        {
            _player.SetActive(true);
            //_playerPrefab.transform.position = Vector3.zero;
        }
        
    }
    private void EnableTPEffect()
    {
        if (_tpEffect)
        {
            if(_tpEffect.activeSelf)
                _tpEffect.SetActive(false);
            _tpEffect.SetActive(true);
            _tpEffect.transform.position = _player.transform.position;
        }
    }
    private async void OnEndGame(TopDownEndGameEvent topDownEndGameEvent)
    {
        _continueBtnClickable = true;
        if (topDownEndGameEvent.endGameExecuteState == UIEndGameExecuteState.Lose)
        {
            _isWinning = false;
            Time.timeScale = 0;
            await UniTask.Delay(500, true);
            Time.timeScale = 1;
            await FadeToBlack(cancellationTokenSource.Token);

        }
        else if (topDownEndGameEvent.endGameExecuteState == UIEndGameExecuteState.Win)
            _isWinning = true;


        GameStatsUIManager.Instance.Init(_damageDealt, _damageReceived, _parriedDamage).Forget();
    }
    private async UniTask FadeToBlack(CancellationToken cancellationToken)
    {
        float elapsed = 0f;

        while (elapsed < 1)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 1;

            // Fade from default to black
            _deathVolume.weight = Mathf.Lerp(0, 1, t);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        // Ensure final values
        _deathVolume.weight = 1;
    }
    private void OnContinueBtn(bool isPressed, int skillIndex)
    {
        if (!isPressed || skillIndex != 5) return;
        OnEndGameContinueButton();
    }
    private void ResetEndGameInfo()
    {
        _damageReceived = 0;
        _damageDealt = 0;
        _parriedDamage = 0;
    }
    public void AddDamageReceived(int amount)
    {
        _damageReceived += amount;
    }
    public void AddDamageDealt(int amount)
    {
        _damageDealt += amount;
    }
    public void AddParriedDamage(int amount)
    {
        _parriedDamage += amount;
    }


    public async void OnEndGameContinueButton()
    {
        if (!_continueBtnClickable) return;


        _continueBtnClickable = false;
        EventBus<TopdownOnEndGameContinueEvent>.Raise(new TopdownOnEndGameContinueEvent());
        if (_isWinning)
        {
            _player.GetComponent<PlayerTopDownStateDriver>().Despawn();
            CombatManager.Instance.SetCombatResultWin();
        }else
            CombatManager.Instance.SetCombatResultLose();

        await UniTask.Delay(1000, true);
        EnableTPEffect();

        await UniTask.Delay(1500, true);
        _player.SetActive(false);

        await UniTask.Delay(2000, true);
        SceneManager.LoadScene("MazeScene");
    }
    public void TrigerLowHealthEffect(float currentHealth, float maxHealth)
    {
        if (currentHealth / maxHealth <= 0.2f && !_isTriggeringLowHealthEffect)
        {
            _lowHealthCTS = new CancellationTokenSource();
            LowHealthVolumeLerp(_lowHealthCTS.Token).Forget();
        }

        if (currentHealth / maxHealth > 0.2f && _isTriggeringLowHealthEffect)
        {
            _lowHealthCTS?.Cancel();
            _lowHealthCTS?.Dispose();
            _lowHealthCTS = null;
            // finally block in LowHealthVolumeLerp handles weight reset and flag cleanup
        }
    }

    private async UniTask LowHealthVolumeLerp(CancellationToken cancellationToken)
    {
        _isTriggeringLowHealthEffect = true;

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Lerp in
                float elapsed = 0f;
                while (elapsed < _lowHealthLerpInDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _lowHealthVolume.weight = Mathf.Lerp(0f, 1f, elapsed / _lowHealthLerpInDuration);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
                _lowHealthVolume.weight = 1f;

                // Lerp out
                elapsed = 0f;
                while (elapsed < _lowHealthLerpOutDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _lowHealthVolume.weight = Mathf.Lerp(1f, 0f, elapsed / _lowHealthLerpOutDuration);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
                _lowHealthVolume.weight = 0f;
            }
        }
        finally
        {
            // Always clean up when loop exits, whether cancelled or not
            _lowHealthVolume.weight = 0f;
            _isTriggeringLowHealthEffect = false;
        }
    }
    public void TriggerParryEffect()
    {
        if (_isFlashing) return;

        ParryVolumeFlash(cancellationTokenSource.Token).Forget();

        //if (_slowMoCooldownReady)
        //    TriggerSlowMotion();
    }

    public void TriggerSlowMotion()
    {
        SlowMotionSequence(cancellationTokenSource.Token).Forget();
    }

    private async UniTask SlowMotionSequence(CancellationToken cancellationToken)
    {
        _slowMoCooldownReady = false;

        float elapsed = 0f;

        // Lerp timescale down
        while (elapsed < _slowMoFadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1f, _slowMoTimeScale, elapsed / _slowMoFadeInDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        Time.timeScale = _slowMoTimeScale;

        // Hold
        await UniTask.Delay(TimeSpan.FromSeconds(_slowMoDuration), ignoreTimeScale: true, cancellationToken: cancellationToken);

        elapsed = 0f;

        // Lerp timescale back up
        while (elapsed < _slowMoFadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(_slowMoTimeScale, 1f, elapsed / _slowMoFadeOutDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        Time.timeScale = 1f;

        // Cooldown
        await UniTask.Delay(TimeSpan.FromSeconds(_slowMoCooldown), ignoreTimeScale: true, cancellationToken: cancellationToken);
        _slowMoCooldownReady = true;
    }

    private async UniTask ParryVolumeFlash(CancellationToken cancellationToken)
    {
        if (!_parryVolume) return;

        _isFlashing = true;
        float elapsed = 0f;

        while (elapsed < _parryVolumeLerpInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _parryVolume.weight = Mathf.Lerp(0f, 1f, elapsed / _parryVolumeLerpInDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        _parryVolume.weight = 1f;

        elapsed = 0f;

        while (elapsed < _parryVolumeLerpOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _parryVolume.weight = Mathf.Lerp(1f, 0f, elapsed / _parryVolumeLerpOutDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        _parryVolume.weight = 0f;

        _isFlashing = false;
    }
}
