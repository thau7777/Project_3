using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerStateDriver : MonoBehaviour
{
    #region References
    [SerializeField, Required]
    [FoldoutGroup("References")]
    private InputReader _inputReader;

    [SerializeField, Required]
    [FoldoutGroup("References")]
    private CharacterController _controller;

    [SerializeField, Required]
    [FoldoutGroup("References")]
    private Animator _animator;
     
    [SerializeField, Required]
    [FoldoutGroup("References")]
    private Renderer _renderer;
    #endregion

    #region Variables
    [SerializeField, TabGroup("Movement Settings")] private float baseMoveSpeed = 5f;   // default
    [SerializeField, TabGroup("Movement Settings")] private float strafeMoveSpeed = 2f;   // default

    [SerializeField, TabGroup("Movement Settings")] private float smoothTime = 0.2f;    // higher = slower accel/decel

    [SerializeField,TabGroup("Animation Settings")] private int attackLayerIndex = 1;   // which layer is your Attack layer
    [SerializeField, TabGroup("Animation Settings")] private float layerBlendSpeed = 5f; // how fast it blends

    private readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int attackTriggerHash = Animator.StringToHash("Attack");
    private readonly int isAimingHash = Animator.StringToHash("isAiming");
    private readonly int inputXHash = Animator.StringToHash("InputX");
    private readonly int inputYHash = Animator.StringToHash("InputY");

    [SerializeField]
    private PlayerContext _context;
    StateMachine machine;
    State root;

    #endregion
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _context = new PlayerContext(
            baseMoveSpeed,
            strafeMoveSpeed,
            smoothTime, 
            _animator, 
            _controller, 
            _renderer,
            Camera.main.transform,
            transform);

        root = new PlayerRoot(null, _context);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();
    }
    private void OnEnable()
    {
        _inputReader.playerTopDownActions.onMove += OnMove;
        _inputReader.playerTopDownActions.onAttack += OnAttack;
        _inputReader.playerTopDownActions.onAim += OnAim;
    }
    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onMove -= OnMove;
        _inputReader.playerTopDownActions.onAttack -= OnAttack;
        _inputReader.playerTopDownActions.onAim -= OnAim;
    }

    private void OnAim(bool isAim)
    {
        _context.isAiming = isAim;
        _animator.SetBool(isAimingHash, isAim);
    }

    private void OnMove(Vector2 vector)
    {
        _context.moveInput = vector;
    }
    private void OnAttack()
    {
        Debug.Log("Attack input received");
    }
    private void Update()
    {
        machine.Tick(Time.deltaTime);
        _context.mouseScreenPos = Mouse.current.position.ReadValue();
        SetInputValueSmoothly();
    }

    private void SetInputValueSmoothly()
    {
        // lerp animator input params
        float currentX = _animator.GetFloat(inputXHash);
        float currentY = _animator.GetFloat(inputYHash);

        float smoothedX = Mathf.Lerp(currentX, _context.moveInput.x, Time.deltaTime * 10f);
        float smoothedY = Mathf.Lerp(currentY, _context.moveInput.y, Time.deltaTime * 10f);

        _animator.SetFloat(inputXHash, smoothedX);
        _animator.SetFloat(inputYHash, smoothedY);
    }
}

[Serializable]
public class PlayerContext
{
    public float currentMoveSpeed;   // smoothed speed
    public float targetMoveSpeed;    // desired speed

    public Vector2 moveInput;   // store the latest input
    public Vector3 moveDir;
    public bool isAiming;     // store whether the player is aiming
    public Vector2 mouseScreenPos;

    public float baseMoveSpeed;
    public float strafeMoveSpeed;
    public float smoothTime;
    public float speedVelocity;
    public Animator anim;
    public CharacterController characterController;
    public Renderer renderer;
    public Transform mainCameraTransform;
    public Transform rootTransform;

    public PlayerContext(
        float baseMoveSpeed, 
        float strafeMoveSpeed,
        float smoothTime, 
        Animator anim, 
        CharacterController characterController, 
        Renderer renderer,
        Transform mainCameraTransform,
        Transform rootTransform)
    {
        this.baseMoveSpeed = baseMoveSpeed;
        this.strafeMoveSpeed = strafeMoveSpeed;
        this.smoothTime = smoothTime;
        this.anim = anim;
        this.characterController = characterController;
        this.renderer = renderer;
        this.mainCameraTransform = mainCameraTransform;
        this.rootTransform = rootTransform;

        speedVelocity = 0;
        isAiming = false;
        moveDir = Vector3.zero;
    }
}
