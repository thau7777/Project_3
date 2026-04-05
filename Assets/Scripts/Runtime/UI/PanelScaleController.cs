using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PanelScaleController : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CancellationTokenSource _cts;
    private EventBinding<TopdownStartGameEvent> _startGameEventBinding;

    private void OnEnable()
    {
        _startGameEventBinding = new EventBinding<TopdownStartGameEvent>(ScaleIn);
        EventBus<TopdownStartGameEvent>.Register(_startGameEventBinding);
    }
    private void OnDisable()
    {
        EventBus<TopdownStartGameEvent>.Deregister(_startGameEventBinding);
    }
    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        // Start fully hidden
        SetScaleX(0f);
    }

    public void ScaleIn()
    {
        if(!TopDownGameManager.Instance.isBossFighting) return;
        CancelCurrentTween();
        _cts = new CancellationTokenSource();
        ScaleXAsync(0f, 1f, _cts.Token).Forget();
    }

    private async UniTaskVoid ScaleXAsync(float from, float to, CancellationToken ct)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (ct.IsCancellationRequested) return;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = scaleCurve.Evaluate(t);
            SetScaleX(Mathf.LerpUnclamped(from, to, curveValue));

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        SetScaleX(to);
    }

    private void SetScaleX(float x)
    {
        Vector3 scale = rectTransform.localScale;
        scale.x = x;
        rectTransform.localScale = scale;
    }

    private void CancelCurrentTween()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private void OnDestroy()
    {
        CancelCurrentTween();
    }
}