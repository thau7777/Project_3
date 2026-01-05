using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class CardRotator : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private float maxRotationX = 30f;
        [SerializeField] private float maxRotationY = 30f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float snapBackDuration = 0.15f;

        [SerializeField] private Animator animator;

        private bool canRotate = false;   // hover
        private bool isRotating = false;  // drag lock

        private Vector3 rotationStartPos;
        private Coroutine snapBackRoutine;

        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void Update()
        {
            if (!canRotate)
                return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector3 mousePos = mouse.position.ReadValue();
            mousePos.z = 1f;

            if (!isRotating && canRotate && mouse.rightButton.wasPressedThisFrame)
            {
                isRotating = true;
                animator.enabled = false;
                StopSnapBack();
                rotationStartPos = mainCam.ScreenToWorldPoint(mousePos);
            }

            if (isRotating && mouse.rightButton.wasReleasedThisFrame)
            {
                isRotating = false;
                canRotate = false;
                animator.enabled = true;
                StopSnapBack();
                snapBackRoutine = StartCoroutine(SnapBack());
            }

            if (isRotating)
            {
                var currentPos = mainCam.ScreenToWorldPoint(mousePos);
                var offset = currentPos - rotationStartPos;

                float xRot = offset.x * -rotationSpeed;
                float yRot = offset.y * rotationSpeed;

                xRot = -Mathf.Clamp(xRot, -maxRotationX, maxRotationX);
                yRot = Mathf.Clamp(yRot, -maxRotationY, maxRotationY);

                transform.localRotation = Quaternion.Euler(yRot, xRot, 0);
            }
        }

        private void StopSnapBack()
        {
            if (snapBackRoutine != null)
            {
                StopCoroutine(snapBackRoutine);
                snapBackRoutine = null;
            }
        }

        private IEnumerator SnapBack()
        {
            Quaternion start = transform.localRotation;
            Quaternion end = Quaternion.identity;

            for (float t = 0; t < snapBackDuration; t += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Slerp(start, end, t / snapBackDuration);
                yield return null;
            }

            transform.localRotation = end;
        }

        private void OnMouseEnter()
        {
            canRotate = true;
        }

        private void OnMouseOver()
        {
            canRotate = true;
        }

        private void OnMouseExit()
        {
            if (!isRotating)
                canRotate = false;
        }
    }
}
