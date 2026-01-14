using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

namespace MyRule
{
    public class WarpController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VisualEffect effect;
        [SerializeField] private CinemachineCamera cineCam;
        [SerializeField] private MeshRenderer cylinder1;
        [SerializeField] private MeshRenderer cylinder2;

        [Header("Lens Settings")]
        [SerializeField] private float minLens = 60f;
        [SerializeField] private float maxLens = 160f;

        [Header("Timing")]
        [SerializeField] private float holdDuration = 5f;
        [SerializeField] private float lensStartBeforeHoldEnd = 1f;
        [SerializeField] private float warpReduceDuration = 3f;
        [SerializeField] private float lensReduceDuration = 6f;

        private float amount;

        private float holdTimer;
        private float warpTimer;
        private float lensTimer;

        private bool warpReducing;
        private bool lensReducing;

        private void Start()
        {
            amount = 1f;

            holdTimer = holdDuration;
            warpTimer = warpReduceDuration;
            lensTimer = lensReduceDuration;

            warpReducing = false;
            lensReducing = false;

            effect.Play();
            effect.SetFloat("WarpAmount", amount);

            cineCam.Lens.FieldOfView = maxLens;

            // Cylinder init
            cylinder1.material.SetFloat("_Active", 1f);
            cylinder2.material.SetFloat("_Active", 1f);
            cylinder2.material.SetFloat("_Power", 0f);
        }

        private void Update()
        {
            HandleHoldPhase();
            HandleWarpAmountAndCylinders();
            HandleLens();
        }

        // ===================== PHASE 1 =====================
        private void HandleHoldPhase()
        {
            if (holdTimer <= 0f) return;

            holdTimer -= Time.deltaTime;

            float powerT = Mathf.Clamp01(1f - (holdTimer / holdDuration));
            cylinder2.material.SetFloat("_Power", powerT);

            float lensStartTime = holdDuration - lensStartBeforeHoldEnd;

            if (!lensReducing && holdTimer <= lensStartTime)
            {
                lensReducing = true;

                lensTimer = lensReduceDuration;
            }

            if (holdTimer <= 0f)
            {
                warpReducing = true;
            }
        }


        // ===================== PHASE 2 =====================
        private void HandleWarpAmountAndCylinders()
        {
            if (!warpReducing) return;

            warpTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(warpTimer / warpReduceDuration);

            // Warp effect
            amount = t;
            effect.SetFloat("WarpAmount", amount);

            cylinder1.material.SetFloat("_Active", t);
            cylinder2.material.SetFloat("_Active", t);

            if (warpTimer <= 0f)
            {
                warpReducing = false;

                effect.SetFloat("WarpAmount", 0f);
                effect.Stop();

                cylinder1.material.SetFloat("_Active", 0f);
                cylinder2.material.SetFloat("_Active", 0f);
            }
        }

        // ===================== PHASE 3 =====================
        private void HandleLens()
        {
            if (!lensReducing) return;

            lensTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(lensTimer / lensReduceDuration);

            cineCam.Lens.FieldOfView = Mathf.Lerp(minLens, maxLens, t);

            if (lensTimer <= 0f)
            {
                lensReducing = false;
                cineCam.Lens.FieldOfView = minLens;
            }
        }
    }
}
