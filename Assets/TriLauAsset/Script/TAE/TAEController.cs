using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace MyRule
{
    public class TAEController : MonoBehaviour
    {
        private const string Voice = "VoiceStreght";

        [Header("Target")]
        [SerializeField] private VisualEffect effect;

        [Header("Curve")]
        [SerializeField] private AnimationCurve talkCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private float curveCycleDuration = 0.2f;

        [Header("Default Value")]
        [SerializeField] private float defaultValue = 0f;

        [Header("Runtime")]
        [SerializeField] private float currentValue;

        private CancellationTokenSource talkingCts;

        private void Reset()
        {
            effect = GetComponent<VisualEffect>();
        }

        private void Awake()
        {
            ResetStrength();
        }

        private void OnDisable()
        {
            CancelTalking();
            ResetStrength();
        }

        private void OnDestroy()
        {
            CancelTalking();
        }

        public async void TriggerTalking(float duration)
        {
            CancelTalking();

            talkingCts = new CancellationTokenSource();
            CancellationTokenSource localCts = talkingCts;
            CancellationToken token = localCts.Token;

            try
            {
                if (effect == null || duration <= 0f)
                {
                    ResetStrength();
                    return;
                }

                float elapsed = 0f;

                while (elapsed < duration)
                {
                    token.ThrowIfCancellationRequested();

                    elapsed += Time.deltaTime;

                    float normalizedTime = curveCycleDuration > 0f
                        ? (elapsed % curveCycleDuration) / curveCycleDuration
                        : 0f;

                    currentValue = talkCurve.Evaluate(normalizedTime);
                    ApplyStrength(currentValue);

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (talkingCts == localCts)
                {
                    currentValue = defaultValue;
                    ResetStrength();

                    localCts.Dispose();
                    talkingCts = null;
                }
                else
                {
                    localCts.Dispose();
                }
            }
        }

        public void CancelTalking()
        {
            if (talkingCts == null) return;

            if (!talkingCts.IsCancellationRequested)
                talkingCts.Cancel();
        }

        private void ApplyStrength(float value)
        {
            if (effect == null) return;

            effect.SetFloat(Voice, value);
        }

        private void ResetStrength()
        {
            if (effect == null) return;

            currentValue = defaultValue;
            effect.SetFloat(Voice, defaultValue);
        }
    }
}
