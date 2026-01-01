using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    [RequireComponent(typeof(Animator))]
    public class MazeMovement : MonoBehaviour
    {
        [Header("Target")]
        public Transform targetPoint;
        public ShapeInfo shapeInfo;

        [Header("Speed")]
        [Tooltip("Tốc độ tăng MoveSpeed (0 -> 1)")]
        public float acceleration = 1.5f;

        [Tooltip("Tốc độ giảm MoveSpeed (1 -> 0)")]
        public float deceleration = 2.5f;

        [Header("Rotation")]
        public float rotateSpeed = 360f;
        public float rotationThreshold = 3f;

        [Header("Stop Distance")]
        public float stopDistance = 0.4f;

        public float gravity = -9.81f;
        public float groundStickForce = -2f;

        private CharacterController controller;
        private Vector3 velocity;

        private Animator animator;
        private float moveSpeed;
        private bool hasArrived;

        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");

        private EventBinding<FirstShapeEvent> firstShapeEventBinding;
        private EventBinding<MazeMoveEvent> mazeMoveEventBinding;

        private void OnEnable()
        {
            firstShapeEventBinding = new EventBinding<FirstShapeEvent>(OnFirstShapeEvent);
            EventBus<FirstShapeEvent>.Register(firstShapeEventBinding);

            mazeMoveEventBinding = new EventBinding<MazeMoveEvent>(OnMove);
            EventBus<MazeMoveEvent>.Register(mazeMoveEventBinding);
        }

        private void OnDisable()
        {
            EventBus<FirstShapeEvent>.Deregister(firstShapeEventBinding);
            EventBus<MazeMoveEvent>.Deregister(mazeMoveEventBinding);
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;
        }

        private void Update()
        {
            ApplyGravity();

            if (targetPoint == null || hasArrived)
            {
                Decelerate();
                return;
            }

            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(targetPoint.position.x, 0, targetPoint.position.z)
            );

            if (distance <= stopDistance)
            {
                hasArrived = true;
                Decelerate();
                return;
            }

            RotateTowardsTarget();

            bool facingTarget = IsFacingTarget();

            if (facingTarget)
                Accelerate();
            else
                Decelerate();
        }

        private void OnFirstShapeEvent(FirstShapeEvent evt)
        {
            shapeInfo = evt.shape;
        }

        private async void OnMove(MazeMoveEvent evt)
        {
            int steps = evt.steps;

            while (steps > 0 && shapeInfo != null)
            {
                foreach (var shape in shapeInfo.pointsTarget)
                {
                    if (shape != null)
                    {
                        SetTarget(shape.transform);
                        shapeInfo = shape;

                        await UniTask.Delay(2400);

                        EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent(1));
                        steps--;
                        break;
                    }
                }
            }

            if (steps == 0 && shapeInfo != null)
            {
                await UniTask.Delay(1000);
                EventBus<MazeGameplayEvent>.Raise(new MazeGameplayEvent(shapeInfo.shapeSO.shapeType));
            }
        }

        private void RotateTowardsTarget()
        {
            Vector3 dir = targetPoint.position - transform.position;
            dir.y = 0;

            if (dir.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }

        private bool IsFacingTarget()
        {
            Vector3 dir = targetPoint.position - transform.position;
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);

            return angle <= rotationThreshold;
        }

        private void Accelerate()
        {
            moveSpeed = Mathf.MoveTowards(
                moveSpeed,
                1f,
                acceleration * Time.deltaTime
            );

            animator.SetFloat(MoveSpeedHash, moveSpeed);
        }

        private void Decelerate()
        {
            moveSpeed = Mathf.MoveTowards(
                moveSpeed,
                0f,
                deceleration * Time.deltaTime
            );

            animator.SetFloat(MoveSpeedHash, moveSpeed);
        }

        public void SetTarget(Transform newTarget)
        {
            targetPoint = newTarget;
            hasArrived = false;
        }

        public void Stop()
        {
            targetPoint = null;
            hasArrived = true;
        }

        void ApplyGravity()
        {
            if (controller.isGrounded)
            {
                if (velocity.y < 0)
                    velocity.y = groundStickForce;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            controller.Move(velocity * Time.deltaTime);
        }
    }
}