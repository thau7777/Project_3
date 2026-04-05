using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class BossCameraController : Singleton<BossCameraController>
{
    [Header("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform bossTransform;

    [Header("Timing")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float holdDuration = 3f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Transform _dummyTarget;
    private CancellationTokenSource _cts;
   
  

    // Call this to trigger the boss focus sequence
    public void FocusOnBoss()
    {
        if(_dummyTarget == null)
        {
            _dummyTarget = new GameObject("CameraDummyTarget").transform;
            _dummyTarget.SetParent(null);
        }

        if (playerTransform != null)
            SetDummyPosition(playerTransform.position);

        cinemachineCamera.Target.TrackingTarget = _dummyTarget;
        if (bossTransform == null)
            bossTransform = TopDownEnemyManager.Instance.BossTransform;
        CancelCurrentSequence();
        _cts = new CancellationTokenSource();
        RunBossSequenceAsync(_cts.Token).Forget();
    }
    private void SetDummyPosition(Vector3 pos)
    {
        _dummyTarget.position = new Vector3(pos.x, 0f, pos.z);
    }
    private async UniTaskVoid RunBossSequenceAsync(CancellationToken ct)
    {
        // Re-attach dummy target every time, in case a previous run handed off to playerTransform
        SetDummyPosition(playerTransform.position);
        cinemachineCamera.Target.TrackingTarget = _dummyTarget; // ← add this

        InputManager.Instance.DisableAllAction();
        await LerpDummyAsync(playerTransform.position, bossTransform.position, ct);
        if (ct.IsCancellationRequested) return;

        // 2. Hold on boss, keep tracking it in case it moves
        await TrackTargetForDuration(bossTransform, holdDuration, ct);
        if (ct.IsCancellationRequested) return;

        // 3. Transition back to player
        InputManager.Instance.EnableActionMap(ActionMap.PlayerTopDown);
        await LerpDummyAsync(bossTransform.position, playerTransform.position, ct);
        if (ct.IsCancellationRequested) return;

        // 4. Hand off tracking back to player directly
        cinemachineCamera.Target.TrackingTarget = playerTransform;
    }

    private async UniTask LerpDummyAsync(Vector3 from, Vector3 to, CancellationToken ct)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            if (ct.IsCancellationRequested) return;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curveValue = transitionCurve.Evaluate(t);
            SetDummyPosition(Vector3.LerpUnclamped(from, to, curveValue));

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        SetDummyPosition(to);
    }

    private async UniTask TrackTargetForDuration(Transform target, float duration, CancellationToken ct)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (ct.IsCancellationRequested) return;

            elapsed += Time.deltaTime;
            // Keep dummy glued to the target in case it moves during the hold
            SetDummyPosition(target.position);

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
    }

    private void CancelCurrentSequence()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private void OnDestroy()
    {
        CancelCurrentSequence();

        if (_dummyTarget != null)
            Destroy(_dummyTarget.gameObject);
    }
}