using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MyRule
{
    public static class Transition
    {
        public static async UniTask TransitionValue(
            System.Action<float> setter,
            float from,
            float to,
            float duration,
            CancellationToken token = default)
        {
            if (duration <= 0f)
            {
                setter(to);
                return;
            }

            float t = 0f;

            setter(from);

            while (t < duration)
            {
                token.ThrowIfCancellationRequested();

                t += Time.deltaTime;

                setter(Mathf.Lerp(from, to, t / duration));

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            setter(to);
        }
    }
}