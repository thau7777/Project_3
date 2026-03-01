using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;


namespace MyRule
{
    public class RTSCameraController : MonoBehaviour
    {
        #region Refs
        [Header("Refs")]
        [SerializeField]
        [Tooltip("The Cinemachine Virtual Camera to be controlled by the controller.")]
        public CinemachineCamera VirtualCamera;

        [SerializeField]
        [Tooltip("The target for the camera to follow.")]
        public Transform CameraTarget;

        [Space]
        [Header("Settings")]
        public CameraControllerSettings[] Settings;

        [Tooltip("How far high the camera is")]
        public float CameraDistance;
        public CameraControllerSettings currentSettings;

        [Space]
        [Header("Boundaries")]
        [SerializeField]
        public bool enableBoundaries = true;

        [SerializeField, Range(-10000, 0)] public float BoundaryMinX = -500f;

        [SerializeField, Range(0, 10000)] public float BoundaryMaxX = 500f;

        [SerializeField, Range(-10000, 0)] public float BoundaryMinZ = -500f;

        [SerializeField, Range(0, 10000)] public float BoundaryMaxZ = 500f;
        #endregion

        #region Private Fields
        private IInputProvider _inputProvider;
        private bool _isRotating;
        private CinemachinePositionComposer _framingTransposer;
        private GameObject _virtualCameraGameObject;
        #endregion

        private void Start()
        {
            currentSettings = Settings[0];
            _inputProvider = GetComponent<IInputProvider>();
            _framingTransposer = VirtualCamera.GetComponent<CinemachinePositionComposer>();
            _virtualCameraGameObject = VirtualCamera.gameObject;
        }

        private void Update()
        {
            HandleBoundaries();
        }

        private void HandleMove()
        {
            Vector2 moveInput = _inputProvider.MovementInput();
            if (moveInput.sqrMagnitude > 0f && (!_isRotating || _inputProvider.CanAlwaysRotate))
            {
                Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y);
                MoveTargetRelativeToCamera(moveVector, currentSettings.CameraScreenSideSpeed);
            }
        }

        private void MoveTargetRelativeToCamera(Vector3 direction, float speed)
        {
            float minZoom = 1;
            if (currentSettings.IsRestricted)
            {
                minZoom = currentSettings.ZoomLevelData[0].ZoomAmount;
            }
            else
            {
                minZoom = currentSettings.CameraZoomMin;
            }

            float relativeZoomCameraMoveSpeed = _framingTransposer.CameraDistance / minZoom;
            Vector3 camForward = _virtualCameraGameObject.transform.forward;
            Vector3 camRight = _virtualCameraGameObject.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 relativeDir = (2 * direction.z * camForward) + (camRight * direction.x);

            CameraTarget.Translate(relativeDir * (relativeZoomCameraMoveSpeed * speed * Time.deltaTime));
        }

        private void HandleBoundaries()
        {
            if (CameraTarget.position.x > BoundaryMaxX)
                CameraTarget.position = new Vector3(BoundaryMaxX, CameraTarget.position.y, CameraTarget.position.z);
            if (CameraTarget.position.x < BoundaryMinX)
                CameraTarget.position = new Vector3(BoundaryMinX, CameraTarget.position.y, CameraTarget.position.z);
            if (CameraTarget.position.z > BoundaryMaxZ)
                CameraTarget.position = new Vector3(CameraTarget.position.x, CameraTarget.position.y, BoundaryMaxZ);
            if (CameraTarget.position.z < BoundaryMinZ)
                CameraTarget.position = new Vector3(CameraTarget.position.x, CameraTarget.position.y, BoundaryMinZ);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (enableBoundaries)
            {
                Handles.color = Color.green;
                Handles.DrawLine(new Vector3(BoundaryMinX, 0, BoundaryMinZ), new Vector3(BoundaryMaxX, 0, BoundaryMinZ));
                Handles.DrawLine(new Vector3(BoundaryMaxX, 0, BoundaryMinZ), new Vector3(BoundaryMaxX, 0, BoundaryMaxZ));
                Handles.DrawLine(new Vector3(BoundaryMinX, 0, BoundaryMinZ), new Vector3(BoundaryMinX, 0, BoundaryMaxZ));
                Handles.DrawLine(new Vector3(BoundaryMinX, 0, BoundaryMaxZ), new Vector3(BoundaryMaxX, 0, BoundaryMaxZ));
                Handles.Label(new Vector3(BoundaryMinX, 0, 0), $"Min X: {BoundaryMinX}");
                Handles.Label(new Vector3(BoundaryMaxX, 0, 0), $"Max X: {BoundaryMaxX}");
                Handles.Label(new Vector3(0, 0, BoundaryMinZ), $"Min Z: {BoundaryMinZ}");
                Handles.Label(new Vector3(0, 0, BoundaryMaxZ), $"Max Z: {BoundaryMaxZ}");
            }
        }

#endif
    }
}