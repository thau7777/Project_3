using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class SmoothCameraLook : MonoBehaviour
    {
        [Header("References")]
        public Transform target;

        [Header("Settings")]
        public float lookSpeed = 0.1f;
        public float smoothSpeed = 5f;
        public float maxXAngle = 15f;
        public float maxYAngle = 25f;

        private Vector2 rotation;
        private Vector2 targetRotation;

        private void Start()
        {
            rotation = Vector2.zero;
            targetRotation = rotation;
        }

        private void Update()
        {
            Vector2 lookInput = Mouse.current.delta.ReadValue() * lookSpeed;

            targetRotation.x -= lookInput.y;
            targetRotation.y += lookInput.x;
            targetRotation.x = Mathf.Clamp(targetRotation.x, -maxXAngle, maxXAngle);
            targetRotation.y = Mathf.Clamp(targetRotation.y, -maxYAngle, maxYAngle);

            rotation = Vector2.Lerp(rotation, targetRotation, Time.deltaTime * smoothSpeed);

            if (target != null)
            {
                target.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            }
        }
    }
}