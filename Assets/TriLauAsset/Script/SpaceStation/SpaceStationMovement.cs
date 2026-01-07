using UnityEngine;
using UnityEngine.Windows;

namespace MyRule
{
    public class SpaceStationMovement : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float smoothSpeed;
        private Vector3 velocity;

        private bool canMove = false;
        private Vector2 movement;
        private Vector2 moveDir;

        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundStickForce = -2f;
        [SerializeField] private float acceleration = 1.5f;
        [SerializeField] private float deceleration = 1.5f;

        private void OnEnable()
        {
            inputReader.spaceStationActions.onMove += OnMove;
        }

        private void OnDisable()
        {
            inputReader.spaceStationActions.onMove -= OnMove;
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            ApplyGravity();

            float targetX = Mathf.Abs(moveDir.x) > 0.1f ? 1f * moveDir.x : 0f;
            float targetY = Mathf.Abs(moveDir.y) > 0.1f ? 1f * moveDir.y : 0f;

            movement.x = Mathf.Lerp(movement.x, targetX, Time.fixedDeltaTime * smoothSpeed);
            movement.y = Mathf.Lerp(movement.y, targetY, Time.fixedDeltaTime * smoothSpeed);

            animator.SetFloat("X", movement.x);
            animator.SetFloat("Y", movement.y);
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded)
            {
                if (velocity.y < 0)
                    velocity.y = groundStickForce;
            }
            else
            {
                velocity.y += gravity * Time.fixedDeltaTime;
            }

            controller.Move(velocity * Time.fixedDeltaTime);
        }

        private void OnMove(Vector2 value)
        {
            moveDir = value;
        }
    }
}