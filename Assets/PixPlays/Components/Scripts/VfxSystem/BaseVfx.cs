using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PixPlays.ElementalVFX
{
    public class BaseVfx : OneShotVFX
    {
        [SerializeField, TabGroup("BaseVfxSettings")] protected bool _forTopdown = true;
        [SerializeField, TabGroup("BaseVfxSettings")] protected float _SafetyDestroy;
        [SerializeField, TabGroup("BaseVfxSettings")] protected float _DestoyDelay;
        [SerializeField, TabGroup("BaseVfxSettings")] protected HitBoxHandler _hitboxObject;
        public float hitboxVfxLifeTime = 1;
        protected VfxData _data;

        private CancellationTokenSource _cts;

        public virtual void Play(VfxData data)
        {
            _data = data;

            // Cancel any previous running tasks (e.g. if Play is called again before Stop)
            CancelAndReset();
            _cts = new CancellationTokenSource();

            if (_data.Duration > _SafetyDestroy)
            {
                _SafetyDestroy += _data.Duration;
            }

            StopAllCoroutines();
            ScheduleStop(_cts.Token).Forget();
            ScheduleSafetyReturn(_cts.Token).Forget();
        }

        public virtual void Stop()
        {
            StopAllCoroutines();
            CancelAndReset(); // Cancel safety task since we're stopping cleanly
            DelayedReturnToPool(_DestoyDelay).Forget();
        }

        private async UniTaskVoid ScheduleStop(CancellationToken token)
        {
            await UniTask.WaitForSeconds(_data.Duration, cancellationToken: token);
            Stop();
        }

        private async UniTaskVoid ScheduleSafetyReturn(CancellationToken token)
        {
            await UniTask.WaitForSeconds(_SafetyDestroy, cancellationToken: token);
            // Safety net: force return if Stop() was never called properly
            CancelAndReset();
            ReturnToPool();
        }

        private async UniTaskVoid DelayedReturnToPool(float delay)
        {
            if (delay > 0)
                await UniTask.WaitForSeconds(delay);
            CancelAndReset();
            ReturnToPool();
        }

        private void CancelAndReset()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }


        private void OnDestroy()
        {
            // Cleanup if the object is destroyed externally
            CancelAndReset();
        }
    }
}