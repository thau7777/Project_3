using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HSM
{
    public class LayerWeightActivity : Activity
    {
        readonly Animator animator;
        readonly int layerIndex;
        readonly float enterWeight;
        readonly float exitWeight;
        readonly float duration;

        public LayerWeightActivity(Animator animator, int layerIndex, float enterWeight, float exitWeight, float duration = 0.25f)
        {
            this.animator = animator;
            this.layerIndex = layerIndex;
            this.enterWeight = enterWeight;
            this.exitWeight = exitWeight;
            this.duration = duration;
        }

        public override Task ActivateAsync(CancellationToken ct)
        {
            if (Mode != ActivityMode.Inactive)
                return Task.CompletedTask;

            Mode = ActivityMode.Activating;

            _ = LerpLayerWeight(animator, layerIndex, enterWeight, duration, ct); // fire-and-forget

            Mode = ActivityMode.Active;
            return Task.CompletedTask;
        }


        public override Task DeactivateAsync(CancellationToken ct)
        {
            if (Mode != ActivityMode.Active)
                return Task.CompletedTask;

            Mode = ActivityMode.Deactivating;

            _ = LerpLayerWeight(animator, layerIndex, exitWeight, duration, ct); // run asynchronously

            Mode = ActivityMode.Inactive;
            return Task.CompletedTask;
        }


        private static async Task LerpLayerWeight(Animator anim, int layer, float targetWeight, float duration, CancellationToken ct)
        {
            float start = anim.GetLayerWeight(layer);
            float elapsed = 0f;

            while (elapsed < duration && !ct.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                anim.SetLayerWeight(layer, Mathf.Lerp(start, targetWeight, t));
                await Task.Yield();
            }

            anim.SetLayerWeight(layer, targetWeight);
        }
    }
}
