using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraCullingMaskController : MonoBehaviour
{
    [SerializeField] private LayerMask _defaultCullingMask = -1; // -1 means "Everything"
    [SerializeField] private LayerMask _targetCullingMask = 0; 
    [SerializeField] private float _delayBeforeCulling = 0.5f; // Delay in seconds

    private Camera _mainCamera;
    private EventBinding<TopDownEndGameEvent> _playerDeadEventBinding;
    private CancellationTokenSource _cancellationTokenSource;

    private void OnEnable()
    {
        _playerDeadEventBinding = new EventBinding<TopDownEndGameEvent>(OnPlayerDead);
        EventBus<TopDownEndGameEvent>.Register(_playerDeadEventBinding);

        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        EventBus<TopDownEndGameEvent>.Deregister(_playerDeadEventBinding);

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        // Reset to default culling mask
        ResetToDefault();
    }

    /// <summary>
    /// Reset culling mask to default
    /// </summary>
    public void ResetToDefault()
    {
        _mainCamera.cullingMask = _defaultCullingMask;
    }

    private async void OnPlayerDead(TopDownEndGameEvent topDownEndGameEvent)
    {
        if(topDownEndGameEvent.endGameExecuteState == UIEndGameExecuteState.Lose)
            await SetCullingMaskToNothingAsync(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// Set culling mask to nothing after a delay
    /// </summary>
    public async UniTask SetCullingMaskToNothingAsync(CancellationToken cancellationToken = default)
    {
        // Wait for the specified duration
        await UniTask.Delay(System.TimeSpan.FromSeconds(_delayBeforeCulling), cancellationToken: cancellationToken);

        // Set culling mask to nothing
        _mainCamera.cullingMask = _targetCullingMask;
    }
}