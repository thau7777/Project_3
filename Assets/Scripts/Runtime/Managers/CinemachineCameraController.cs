using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    [TabGroup("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [TabGroup("References")]
    [SerializeField] private InputReader _inputReader;
    [TabGroup("References")]
    [SerializeField] private Animator _animator;

    private CinemachineInputAxisController _cinemachineInputAxisController;
    private bool _canRotate = false;


    private int DieHash = Animator.StringToHash("PlayerDie");

    private EventBinding<TopDownPlayerDeadEvent> _playerDeadEventBinding;

    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        _playerDeadEventBinding = new(TriggerDeathAnimation);
        EventBus<TopDownPlayerDeadEvent>.Register(_playerDeadEventBinding);
    }

    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onRightClick -= OnRightClick;
        EventBus<TopDownPlayerDeadEvent>.Deregister(_playerDeadEventBinding);
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

    public void TriggerDeathAnimation()
    {
        _animator.Play(DieHash);
    }
}