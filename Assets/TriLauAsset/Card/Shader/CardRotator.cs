using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class CardRotator : MonoBehaviour
    {
        [SerializeField] private Card card;

        [Header("Rotation")]
        [SerializeField] private float maxRotationX = 30f;
        [SerializeField] private float maxRotationY = 30f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float snapBackDuration = 0.15f;

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
            if (!canRotate || !card.IsShowing)
                return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector3 mousePos = mouse.position.ReadValue();
            mousePos.z = 1f;

            if (!isRotating && canRotate && mouse.rightButton.wasPressedThisFrame)
            {
                isRotating = true;
                StopSnapBack();
                rotationStartPos = mainCam.ScreenToWorldPoint(mousePos);
            }

            if (isRotating && mouse.rightButton.wasReleasedThisFrame)
            {
                isRotating = false;
                canRotate = false;
                StopSnapBack();
                snapBackRoutine = StartCoroutine(SnapBack());
            }

            if (isRotating && card.IsShowing)
            {
                var currentPos = mainCam.ScreenToWorldPoint(mousePos);
                var offset = currentPos - rotationStartPos;

                float xRot = offset.x * -rotationSpeed;
                float yRot = offset.y * rotationSpeed;

                xRot = -Mathf.Clamp(xRot, -maxRotationX, maxRotationX);
                yRot = Mathf.Clamp(yRot, -maxRotationY, maxRotationY);

                transform.rotation = Quaternion.Euler(yRot, xRot, 0);
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
            Quaternion start = transform.rotation;
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
