using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;


namespace MyRule
{
    public class RTSCameraController : Singleton<RTSCameraController>
    {
        [Header("References")]
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private CameraData cameraData;
        [SerializeField] private Transform target;
        [SerializeField] private InputProvider input;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private AnimationCurve moveSpeedZoomCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);
        public AnimationCurve MoveSpeedZoomCurve => moveSpeedZoomCurve;

        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;
        
        [Space(10)]
        [SerializeField] private float edgeScrollingMargin = 15f;
        Vector3 velocity = Vector3.zero;
        Vector2 edgeScrollInput;

        [Header("Rotation")]
        [SerializeField] private float orbitSensitivity = 0.5f;
        [SerializeField] private float orbitSmoothing = 5f;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float zoomSmoothing = 5f;
        private float currentZoomSpeed = 0f;

        public float ZoomLevel
        {
            get
            {
                InputAxis axis = orbitalFollow.RadialAxis;

                return Mathf.InverseLerp(axis.Range.x, axis.Range.y, axis.Value);
            }
        }

        [Header("Bounds")]
        public float minX, maxX, minZ, maxZ;

        private CinemachineOrbitalFollow orbitalFollow;

        protected override void Awake()
        {
            base.Awake();

            orbitalFollow = virtualCamera.GetComponent<CinemachineOrbitalFollow>();
        }

        private void LateUpdate()
        {
            float deltaTime = Time.unscaledDeltaTime;

            HandleEdgeScrolling();

            HandleMove(deltaTime);
            
            HandleOrbit(deltaTime);

            HandleZoom(deltaTime);
        }

        #region EdgeScrolling
        private void HandleEdgeScrolling()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            edgeScrollInput = Vector2.zero;

            if (mousePos.x <= edgeScrollingMargin)
            {
                edgeScrollInput.x = -1f;
            }
            else if (mousePos.x >= Screen.width - edgeScrollingMargin)
            {
                edgeScrollInput.x = 1f;
            }

            if (mousePos.y <= edgeScrollingMargin)
            {
                edgeScrollInput.y = -1f;
            }   
            else if (mousePos.y >= Screen.height - edgeScrollingMargin)
            {
                edgeScrollInput.y = 1f;
            }
        }
        #endregion

        #region Zoom
        private void HandleZoom(float deltaTime)
        {
            InputAxis axis = orbitalFollow.RadialAxis;

            float targetZoomSpeed = 0f;

            if (Mathf.Abs(input.ScrollInput.y) >= 0.001f)
            {
                targetZoomSpeed = zoomSpeed * input.ScrollInput.y;
            }

            currentZoomSpeed = Mathf.Lerp(currentZoomSpeed, targetZoomSpeed, zoomSmoothing * deltaTime);

            axis.Value -= currentZoomSpeed;
            axis.Value = Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);

            orbitalFollow.RadialAxis = axis;
        }
        #endregion

        #region Rotation
        private void HandleOrbit(float deltaTime)
        {
            Vector3 orbitInput = input.LookInput * (input.MiddleClickInput ? 1f : 0f);

            orbitInput *= orbitSensitivity;

            InputAxis horizontalAxis = orbitalFollow.HorizontalAxis;
            InputAxis verticalAxis = orbitalFollow.VerticalAxis;

            //horizontalAxis.Value += orbitInput.x;
            //verticalAxis.Value -= orbitInput.y;

            horizontalAxis.Value = Mathf.Lerp(horizontalAxis.Value, horizontalAxis.Value + orbitInput.x, orbitSmoothing * deltaTime);
            verticalAxis.Value = Mathf.Lerp(verticalAxis.Value, verticalAxis.Value + orbitInput.y, orbitSmoothing * deltaTime);

            //horizontalAxis.Value = Mathf.Clamp(horizontalAxis.Value,horizontalAxis.Range.x, horizontalAxis.Range.y);
            verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, verticalAxis.Range.x, verticalAxis.Range.y);

            orbitalFollow.HorizontalAxis = horizontalAxis;
            orbitalFollow.VerticalAxis = verticalAxis;
        }
        #endregion

        #region Move

        private void HandleMove(float deltaTime)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = Camera.main.transform.right;
            right.y = 0;
            right.Normalize();

            Vector3 inputVector = new Vector3(input.MoveInput.x + edgeScrollInput.x, 0, input.MoveInput.y + edgeScrollInput.y);
            inputVector.Normalize();

            float zoomMult = moveSpeedZoomCurve.Evaluate(ZoomLevel);

            Vector3 targetVelocity = inputVector * moveSpeed * zoomMult;

            if (inputVector.magnitude > 0.001f)
            {
                velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration *  deltaTime);
            }
            else
            {
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * deltaTime);
            }

            Vector3 motion = targetVelocity * deltaTime;

            target.position += forward * motion.z + right * motion.x;

            target.position = new Vector3(
            Mathf.Clamp(target.position.x, minX, maxX),
            target.position.y,
            Mathf.Clamp(target.position.z, minZ, maxZ)
            );
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Vector3 center = new Vector3(
                (minX + maxX) / 2f,
                0,
                (minZ + maxZ) / 2f
            );

            Vector3 size = new Vector3(
                maxX - minX,
                0.1f,
                maxZ - minZ
            );

            Gizmos.DrawWireCube(center, size);
        }
        #endregion
    }
}