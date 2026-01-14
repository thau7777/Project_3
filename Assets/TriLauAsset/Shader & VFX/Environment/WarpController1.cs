using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace MyRule
{
    public class ReverseLensProcess : MonoBehaviour
    {
        public static ReverseLensProcess Instance;
        [SerializeField] private VisualEffect effect;

        [Header("Cinemachine")]
        [SerializeField] private CinemachineCamera vcam;
        [SerializeField] private float minLens = 40f;
        [SerializeField] private float maxLens = 60f;

        [Header("Cylinders")]
        [SerializeField] private MeshRenderer cylinder1;
        [SerializeField] private MeshRenderer cylinder2;

        [Header("Timing")]
        [SerializeField] private float growTime = 6f;
        [SerializeField] private float powerDownTime = 2f;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetupStartState();
        }

        public void Active()
        {
            StartCoroutine(ProcessRoutine());
        }

        // ================= START STATE =================
        void SetupStartState()
        {
            vcam.Lens.FieldOfView = minLens;

            effect.SetFloat("WarpAmount", 1f);
            cylinder1.material.SetFloat("_Active", 0f);
            cylinder2.material.SetFloat("_Active", 0f);
            cylinder2.material.SetFloat("_Power", 1f);
        }

        // ================= MAIN PROCESS =================
        IEnumerator ProcessRoutine()
        {
            float t = 0f;

            while (t < growTime)
            {
                float progress = t / growTime;

                vcam.Lens.FieldOfView = Mathf.Lerp(minLens, maxLens, progress);

                cylinder1.material.SetFloat("_Active", progress);

                t += Time.deltaTime;
                yield return null;
            }

            vcam.Lens.FieldOfView = maxLens;
            cylinder1.material.SetFloat("_Active", 1f);

            t = 0f;

            while (t < powerDownTime)
            {
                float progress = t / powerDownTime;

                cylinder2.material.SetFloat("_Active", progress);
                cylinder2.material.SetFloat("_Power", Mathf.Lerp(1f, 0f, progress));

                t += Time.deltaTime;
                yield return null;
            }

            cylinder2.material.SetFloat("_Active", 1f);
            cylinder2.material.SetFloat("_Power", 0f);
        }
    }
}
