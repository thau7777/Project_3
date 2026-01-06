using UnityEngine;
using UnityEngine.InputSystem;

namespace MyRule
{
    public class SpaceStationRotator : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private Transform camTarget;

        [Header("Clamp X Rotation")]
        [SerializeField] private float minX = -30f;
        [SerializeField] private float maxX = 30f;

        private float currentXRotation = 0f;

        private void OnEnable()
        {
            inputReader.spaceStationActions.onLook += OnLook;
        }

        private void OnDisable()
        {
            inputReader.spaceStationActions.onLook -= OnLook;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnLook(Vector2 value)
        {
            transform.rotation *= Quaternion.Euler(0, value.x * rotationSpeed, 0);

            currentXRotation -= value.y * rotationSpeed;
            currentXRotation = Mathf.Clamp(currentXRotation, minX, maxX);

            camTarget.localRotation = Quaternion.Euler(currentXRotation, 0, 0);
        }
    }
}
