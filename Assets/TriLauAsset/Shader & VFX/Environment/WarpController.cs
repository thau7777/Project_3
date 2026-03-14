using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

namespace MyRule
{
    public class WarpController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VisualEffect effect;
        [SerializeField] private MeshRenderer cylinder1;
        [SerializeField] private MeshRenderer cylinder2;

        [SerializeField] private float powerFrom1 = 20f;
        [SerializeField] private float powerFrom2 = 4f;

        [Header("Timing")]
        [SerializeField] private float effectDuration = 5f;
        [SerializeField] private float delayToActiveWarp = 4f;
        [SerializeField] private float warpDuration = 5f;
        [SerializeField] private float delayPower = 1f;
        [SerializeField] private float powerDuration = 2f;

        private CancellationTokenSource cts;

        private void Start()
        {
            effect.Stop();
            cylinder1.material.SetFloat("_Active", 0f);
            cylinder2.material.SetFloat("_Active", 0f);
            cylinder1.material.SetFloat("_Power", powerFrom1);
            cylinder2.material.SetFloat("_Power", powerFrom1);
        }

        public async void StartRunWarpDrive()
        {
            cts = new CancellationTokenSource();
            
            effect.Play();
            effect.SetFloat("WarpAmount", 0f);
            cylinder1.material.SetFloat("_Active", 0f);
            cylinder2.material.SetFloat("_Active", 0f);
            cylinder1.material.SetFloat("_Power", powerFrom1);
            cylinder2.material.SetFloat("_Power", powerFrom1);

            Transition.TransitionValue(
                value => effect.SetFloat("WarpAmount", value),
                from: 0f,
                to: 1f,
                duration: effectDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => cylinder1.material.SetFloat("_Active", value),
                from: 0f,
                to: 1f,
                duration: warpDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => cylinder2.material.SetFloat("_Active", value),
                from: 0f,
                to: 1f,
                duration: warpDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => cylinder1.material.SetFloat("_Power", value),
                from: powerFrom1,
                to: powerFrom2,
                duration: warpDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => cylinder2.material.SetFloat("_Power", value),
                from: powerFrom1,
                to: powerFrom2,
                duration: warpDuration,
                cts.Token).Forget();

            await UniTask.Delay((int)(delayPower * 1000));

            Transition.TransitionValue(
                value => cylinder1.material.SetFloat("_Power", value),
                from: powerFrom2,
                to: 0f,
                duration: powerDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => cylinder2.material.SetFloat("_Power", value),
                from: powerFrom2,
                to: 0f,
                duration: powerDuration,
                cts.Token).Forget();
        }
    }
}
