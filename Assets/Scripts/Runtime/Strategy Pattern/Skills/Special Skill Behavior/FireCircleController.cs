using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class FireCircleController : MonoBehaviour
{
    private GameObject _mainSkillVfxGo;

    private float _activeDuration = 1f;
    private float _inactiveDuration = 2f;

    private CancellationTokenSource _cts;
    private void Awake()
    {
        _mainSkillVfxGo = transform.GetChild(0).gameObject;
    }
    private void Start()
    {
        StartCycle();
    }
    private void OnDisable()
    {
        StopCycle();
    }
    public void StartCycle()
    {
        StopCycle();
        _cts = new CancellationTokenSource();
        ToggleCycleAsync(_cts.Token).Forget();
    }

    public void StopCycle()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        if (_mainSkillVfxGo != null)
            _mainSkillVfxGo.SetActive(false);
    }

    private async UniTaskVoid ToggleCycleAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            _mainSkillVfxGo.SetActive(true);
            _mainSkillVfxGo.GetComponent<HitBoxHandler>().StartHitBoxCoroutine(_activeDuration);
            await UniTask.WaitForSeconds(_activeDuration, cancellationToken: ct);

            _mainSkillVfxGo.SetActive(false);
            await UniTask.WaitForSeconds(_inactiveDuration, cancellationToken: ct);
        }
    }

}