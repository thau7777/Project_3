using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraController : Singleton<CinemachineCameraController>
{
    [TabGroup("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [TabGroup("References")]
    [SerializeField] private InputReader _inputReader;
    [TabGroup("References")]
    [SerializeField] private Animator _animator;
    [TabGroup("References")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin _noiseChannel;

    [SerializeField, TabGroup("CameraNoiseShake")] private float _targetNoiseAmplitude = 0.6f;
    [SerializeField, TabGroup("CameraNoiseShake")] private float _targetNoiseFrequency = 1f; // ← add this
    [SerializeField, TabGroup("CameraNoiseShake")] private float _shakeDuration = 3f;
    [SerializeField, TabGroup("CameraNoiseShake")] private float _lerpDuration = 1f;

    private CinemachineInputAxisController _cinemachineInputAxisController;
    private bool _canRotate = false;
    private CancellationTokenSource _noiseCts;

    private int DieHash = Animator.StringToHash("PlayerDie");

    private EventBinding<TopDownEndGameEvent> _playerDeadEventBinding;

    
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        // Get the component reference
        if (cinemachineCamera == null)
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
        }
        if(_noiseChannel == null)
        {
            _noiseChannel = GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        _cinemachineInputAxisController = cinemachineCamera.GetComponent<CinemachineInputAxisController>();

        if (_cinemachineInputAxisController == null)
        {
            Debug.LogError("CinemachineInputAxisController not found on CinemachineCamera!");
        }
    }

    private void OnEnable()
    {
        _inputReader.playerTopDownActions.onMiddleClick += OnRightClick;
        _playerDeadEventBinding = new(TriggerDeathAnimation);
        EventBus<TopDownEndGameEvent>.Register(_playerDeadEventBinding);
    }

    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onMiddleClick -= OnRightClick;
        EventBus<TopDownEndGameEvent>.Deregister(_playerDeadEventBinding);
        CancelNoise();
    }

    private void OnRightClick(bool value)
    {
        _canRotate = value;

        if (_cinemachineInputAxisController != null && _cinemachineInputAxisController.Controllers.Count > 0)
        {
            _cinemachineInputAxisController.Controllers[0].Enabled = _canRotate; // Look Orbit X only
        }

        // Lock cursor when rotating
        if (_canRotate)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void TriggerDeathAnimation(TopDownEndGameEvent topDownEndGameEvent)
    {
        if(topDownEndGameEvent.endGameExecuteState == UIEndGameExecuteState.Lose)
            _animator.Play(DieHash);
    }
    public void ShakeSequence(float duration)
    {
        CancelNoise();
        _noiseCts = new CancellationTokenSource();
        RunShakeSequenceAsync(duration, _noiseCts.Token).Forget();
    }

    private async UniTaskVoid RunShakeSequenceAsync(float duration, CancellationToken ct)
    {
        // 1. Shake in
        await LerpNoiseAsync(0f, _targetNoiseAmplitude, _lerpDuration, ct);
        if (ct.IsCancellationRequested) return;

        // 2. Hold
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration), cancellationToken: ct);
        if (ct.IsCancellationRequested) return;

        // 3. Shake out
        await LerpNoiseAsync(_targetNoiseAmplitude, 0f, _lerpDuration, ct);
    }
    public void ShakeIn()
    {
        CancelNoise();
        _noiseCts = new CancellationTokenSource();
        LerpNoiseAsync(0f, _targetNoiseAmplitude, _lerpDuration, _noiseCts.Token).Forget();
    }

    public void ShakeOut()
    {
        CancelNoise();
        _noiseCts = new CancellationTokenSource();
        LerpNoiseAsync(_noiseChannel.AmplitudeGain, 0f, _lerpDuration, _noiseCts.Token).Forget();
    }

    private async UniTask LerpNoiseAsync(float from, float to, float duration, CancellationToken ct)
    {
        float freqFrom = from == 0f ? 0f : _targetNoiseFrequency;
        float freqTo = to == 0f ? 0f : _targetNoiseFrequency;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (ct.IsCancellationRequested) return;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _noiseChannel.AmplitudeGain = Mathf.Lerp(from, to, t);
            _noiseChannel.FrequencyGain = Mathf.Lerp(freqFrom, freqTo, t);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        _noiseChannel.AmplitudeGain = to;
        _noiseChannel.FrequencyGain = freqTo;
    }

    private void CancelNoise()
    {
        _noiseCts?.Cancel();
        _noiseCts?.Dispose();
        _noiseCts = null;
    }
}