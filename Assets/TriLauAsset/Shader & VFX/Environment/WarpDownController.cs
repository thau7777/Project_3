using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace MyRule
{
    public class WarpDownController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VisualEffect effect;
        [SerializeField] private MeshRenderer cylinder1;
        [SerializeField] private MeshRenderer cylinder2;
        [SerializeField] private Volume flareVol;
        [SerializeField] private ParticleSystem scanMap;

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

            StartRunWarpDrive();
        }

        public async void StartRunWarpDrive()
        {
            await UniTask.Delay(1000);

            cts?.Cancel(); 
            cts = new CancellationTokenSource();

            effect.Play();
            effect.SetFloat("WarpAmount", 0f);
            cylinder1.material.SetFloat("_Active", 0f);
            cylinder2.material.SetFloat("_Active", 0f);
            cylinder1.material.SetFloat("_Power", powerFrom1);
            cylinder2.material.SetFloat("_Power", powerFrom1);

            transform.DOScaleZ(80, 1f);

            Transition.TransitionValue(
                value => effect.SetFloat("WarpAmount", value),
                from: 0f,
                to: 1f,
                duration: effectDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => flareVol.weight = value,
                from: 0f,
                to: 1f,
                duration: effectDuration,
                cts.Token).Forget();

            await UniTask.Delay(1000);

            scanMap.Play();

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

            CameraShaker.Instance.ShakeRandomDirection(force: 10f, duration: 1f);

            await UniTask.Delay(3000);

            transform.DOScale(new Vector3(0, 0, 80), 1f);

            Transition.TransitionValue(
                value => cylinder1.material.SetFloat("_Power", value),
                from: powerFrom1,
                to: powerFrom2,
                duration: powerDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => cylinder2.material.SetFloat("_Power", value),
                from: powerFrom1,
                to: powerFrom2,
                duration: powerDuration,
                cts.Token).Forget();

            Transition.TransitionValue(
                value => flareVol.weight = value,
                from: 1f,
                to: 0f,
                duration: 0.4f,
                cts.Token).Forget();
        }
    }
}