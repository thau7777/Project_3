using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HSM {
    public class ColorPhaseActivity : Activity {
        readonly Renderer renderer;
        readonly Material mat; // cached instance to avoid repeated .material allocations

        public Color enterColor = Color.red;
        public Color exitColor  = Color.yellow;

        public ColorPhaseActivity(Renderer r){
            renderer = r;
            if (renderer != null){
                mat = renderer.material; // clone once per activity/state
            }
        }

        public override Task ActivateAsync(CancellationToken ct)
        {
            if (this.Mode != ActivityMode.Inactive || mat == null)
                return Task.CompletedTask;

            this.Mode = ActivityMode.Activating;
            mat.color = enterColor;
            this.Mode = ActivityMode.Active;

            return Task.CompletedTask;
        }

        public override Task DeactivateAsync(CancellationToken ct)
        {
            if (this.Mode != ActivityMode.Active || mat == null)
                return Task.CompletedTask;

            this.Mode = ActivityMode.Deactivating;
            mat.color = exitColor;
            this.Mode = ActivityMode.Inactive;

            return Task.CompletedTask;
        }

    }
}