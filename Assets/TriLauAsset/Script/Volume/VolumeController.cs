using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyRule
{
    public class VolumeController : Singleton<VolumeController>
    {
        [SerializeField] private Volume uiVolume;
        [SerializeField] private Volume flareVolume;
        [SerializeField] private float transitionDuration = 1f;
        
        private bool uiToggle = false;
        private bool flareToggle = false;

        private CancellationTokenSource cts;

        public void AdjustUIVolumeWeight()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            if (uiToggle)
            {
                uiToggle = false;
                Transition.TransitionValue(
                    setter: value => uiVolume.weight = value,
                    from: uiVolume.weight,
                    to: 0f,
                    duration: transitionDuration,
                    cts.Token).Forget();
            }
            else
            {
                uiToggle = true;
                Transition.TransitionValue(
                    setter: value => uiVolume.weight = value,
                    from: uiVolume.weight,
                    to: 1f,
                    duration: transitionDuration,
                    cts.Token).Forget();
            }
        }

        public void AdjustFlareVolumeWeight()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            if (flareToggle)
            {
                uiToggle = false;
                Transition.TransitionValue(
                    setter: value => flareVolume.weight = value,
                    from: uiVolume.weight,
                    to: 0f,
                    duration: transitionDuration,
                    cts.Token).Forget();
            }
            else
            {
                uiToggle = true;
                Transition.TransitionValue(
                    setter: value => flareVolume.weight = value,
                    from: uiVolume.weight,
                    to: 1f,
                    duration: transitionDuration,
                    cts.Token).Forget();
            }
        }
    }
}