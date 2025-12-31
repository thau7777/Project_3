using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private InputReader _inputReader;

    private CinemachineInputAxisController _cinemachineInputAxisController;
    private bool _canRotate = false;

    private void Awake()
    {
        // Get the component reference
        if (cinemachineCamera == null)
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
        }

        _cinemachineInputAxisController = cinemachineCamera.GetComponent<CinemachineInputAxisController>();

        if (_cinemachineInputAxisController == null)
        {
            Debug.LogError("CinemachineInputAxisController not found on CinemachineCamera!");
        }
    }

    private void OnEnable()
    {
        _inputReader.playerTopDownActions.onRightClick += OnRightClick;
    }

    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onRightClick -= OnRightClick;
    }

    private void OnRightClick(bool value)
    {
        _canRotate = value;

        if (_cinemachineInputAxisController != null && _cinemachineInputAxisController.Controllers.Count > 0)
        {
            _cinemachineInputAxisController.Controllers[0].Enabled = _canRotate; // Look Orbit X only
        }

        // Lock cursor when rotating
        if (_canRotate)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}