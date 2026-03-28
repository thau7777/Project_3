using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static OneLine.Examples.ComplexExample;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;


namespace Turnbase
{
    public class CameraAction : MonoBehaviour
    {
        public static CameraAction instance { get; private set; }

        [Header("Cài đặt Camera")]
        public CinemachineCamera cam; 
        public float smoothSpeed = 5f;

        [Header("Chế độ Target + Offset")]
        public Transform targetPoint;
        public Vector3 offsetPosition;
        public Vector3 offsetRotation;


        [Header("Cấu hình Lens")]
        public float normalFOV = 60f;
        public float zoomFOV = 80f;
        public float fovSmoothSpeed = 30f;

        [Header("Điểm Neo Cố Định")]
        [SerializeField] private Transform CameraTargetAll;
        [SerializeField] private Transform TargetAllPlayer;
        [SerializeField] private Transform TargetAllEnemy;

        [SerializeField] private Transform DeadCameraTarget;



        private Transform currentAnchor;

        private bool shouldTeleport = false;

        public Volume globalVolume;
        private Vignette vignette;


        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            if (cam == null) cam = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            if (globalVolume.profile.TryGet<Vignette>(out var v))
            {
                vignette = v;
            }
        }

        private void LookAtAnchorTransform(Transform anchor, bool teleportImmediately = false)
        {
            targetPoint = null;

            currentAnchor = anchor;
            shouldTeleport = teleportImmediately;

            if (anchor == null)
            {
                Debug.LogWarning("Anchor mục tiêu bị Null. Camera sẽ dừng cập nhật.");
            }
        }

        private void SetTargetAndOffset(Character character, Vector3 posOffset, Vector3 rotOffset, bool teleport = false)
        {
            currentAnchor = null;
            shouldTeleport = teleport;

            if (character != null)
            {
                targetPoint = character.transform.Find("CameraTarget");

            }
            else
            {
                targetPoint = null;
            }

            offsetPosition = posOffset;
            offsetRotation = rotOffset;
        }

        public void TargetAllTeam()
        {
            LookAtAnchorTransform(TargetAllPlayer, true);
        }

        public void TargetAllEnemies(bool teleportImmediately = false)
        {
            LookAtAnchorTransform(TargetAllEnemy, teleportImmediately);
        }

        public void TargetDeadCamera(bool teleportImmediately = false)
        {
            LookAtAnchorTransform(DeadCameraTarget, teleportImmediately = true);

            Camera.main.cullingMask = LayerMask.GetMask("Default", "Player", "WarpDrive");

            StopAllCoroutines(); 
            StartCoroutine(FadeVignetteRoutine(2.0f));
        }

        private IEnumerator FadeVignetteRoutine(float duration)
        {
            if (vignette == null) yield break;

            float elapsed = 0;
            float startIntensity = vignette.intensity.value;
            float startSmoothness = vignette.smoothness.value;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float percent = elapsed / duration;

                vignette.intensity.value = Mathf.Lerp(startIntensity, 0.6f, percent);
                vignette.smoothness.value = Mathf.Lerp(startSmoothness, 0.5f, percent);

                yield return null;
            }

            vignette.intensity.value = 0.6f;
            vignette.smoothness.value = 0.5f;
        }


        public void ResetCamera()
        {
            currentAnchor = null;
            shouldTeleport = false;

            if (targetPoint != null)
            {
                offsetPosition = Vector3.zero;
                offsetRotation = Vector3.zero;
            }
            else
            {
                LookAtAnchorTransform(TargetAllPlayer, false);
            }
        }

        public void LookCameraAtTarget(Character character)
        {
            SetTargetAndOffset(character, Vector3.zero, Vector3.zero,true);
        }


        public void NormalAttack(Character attacker, bool teleportImmediately = false)
        {
            Vector3 pos = new Vector3(-0.5f, 0f, -0.77f);
            Vector3 rot = new Vector3(0f, 0f, 0f);

            SetTargetAndOffset(attacker, pos, rot, teleportImmediately);
        }

        public void ReadySkill(Character character)
        {
            Vector3 pos = new Vector3(-0.3f, 0.05f, -0.9f);
            Vector3 rot = new Vector3(8f, 20f, 5f);
            SetTargetAndOffset(character, pos, rot);
        }

        public void ReadyUseItem(Character character)
        {
            Vector3 pos = new Vector3(-0.1f, 0f, -0.77f);
            Vector3 rot = new Vector3(0f, 20f, 0f);
            SetTargetAndOffset(character, pos, rot);
        }

        public void NormalCamera(Character character)
        {
            Vector3 pos = new Vector3(0f, 0f, 0f);
            Vector3 rot = new Vector3(0f, 0f, 0f);
            SetTargetAndOffset(character, pos, rot);
        }


        public void ParryCamera(Character character)
        {
            Vector3 pos = new Vector3(-1f, -1f, 1f);
            Vector3 rot = new Vector3(0f, -12f, 0f);
            SetTargetAndOffset(character, pos, rot);
        }

        public void PerfectParryCamera(Character character)
        {
            Vector3 pos = new Vector3(-1.5f, 0.5f, -1.5f);
            Vector3 rot = new Vector3(10f, 30f, 0f);
            SetTargetAndOffset(character, pos, rot);
        }


        private void LateUpdate()
        {
            Vector3 desiredPos;
            Quaternion desiredRot;

            if (currentAnchor != null)
            {
                desiredPos = currentAnchor.position;
                desiredRot = currentAnchor.rotation;
            }
            else if (targetPoint != null)
            {
                desiredPos = targetPoint.position + offsetPosition;
                desiredRot = targetPoint.rotation * Quaternion.Euler(offsetRotation);
            }
            else return;

            float distance = Vector3.Distance(transform.position, desiredPos);
            float targetFOV = (distance > 0.1f && !shouldTeleport) ? zoomFOV : normalFOV;

            if (shouldTeleport)
            {
                transform.position = desiredPos;
                transform.rotation = desiredRot;
                cam.Lens.FieldOfView = normalFOV;
                shouldTeleport = false;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * smoothSpeed);

                cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
            }
        }
    }
}
