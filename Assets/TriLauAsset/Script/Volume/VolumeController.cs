using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyRule
{
    public class VolumeController : Singleton<VolumeController>
    {
        [SerializeField] private Volume uiVolume;
        [SerializeField] private float transitionDuration = 1f;
        
        private bool toggle = false;

        private CancellationTokenSource cts;

        public void AdjustVolumeWeight()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            if (toggle)
            {
                toggle = false;
                Transition.TransitionValue(
                    setter: value => uiVolume.weight = value,
                    from: uiVolume.weight,
                    to: 0f,
                    duration: transitionDuration,
                    cts.Token).Forget();
            }
            else
            {
                toggle = true;
                Transition.TransitionValue(
                    setter: value => uiVolume.weight = value,
                    from: uiVolume.weight,
                    to: 1f,
                    duration: transitionDuration,
                    cts.Token).Forget();
            }
        }    
    }
}