using HSM;
using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MyPlayerStateDriver : MonoBehaviour
{
    #region References
    [SerializeField, Required]
    private InputReader _inputReader;

    [SerializeField, Required]
    private CharacterController _controller;

    [SerializeField, Required]
    private Animator _animator;

    [SerializeField, Required]
    private Renderer _renderer;
    #endregion

    #region Variables
    [Header("Movement Settings")]
    [SerializeField] private float baseMoveSpeed = 5f;   // default
    [SerializeField] private float accelerate = 5f; // how fast speed interpolates

    [Header("Animation Settings")]
    [SerializeField] private int attackLayerIndex = 1;   // which layer is your Attack layer
    [SerializeField] private float layerBlendSpeed = 5f; // how fast it blends
    private float targetAttackLayerWeight = 0f;          // 0 = off, 1 = active
    private float currentAttackLayerWeight = 0f;

    [SerializeField]
    private MyPlayerContext _context;
    #endregion
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _context = new MyPlayerContext(
            baseMoveSpeed, 
            accelerate, 
            _animator, 
            _controller, 
            _renderer,
            Camera.main.transform,
            transform);
    }
    private void OnEnable()
    {
        _inputReader.playerTopDownActions.onMove += OnMove;
        _inputReader.playerTopDownActions.onAttack += OnAttack;
    }
    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onMove -= OnMove;
        _inputReader.playerTopDownActions.onAttack -= OnAttack;
    }
    private void OnMove(Vector2 vector)
    {
        _context.moveInput = vector;
    }
    private void OnAttack()
    {
        Debug.Log("Attack input received");
    }


    //private void HandleMove()
    //{
    //    if (moveInput.sqrMagnitude < 0.01f)
    //        return;

    //    // camera reference
    //    Transform cam = Camera.main.transform;

    //    // flatten forward/right onto XZ
    //    Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
    //    Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

    //    // input relative to camera
    //    Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

    //    // move
    //    _controller.Move(moveDir * currentMoveSpeed * Time.deltaTime);

    //    // rotate toward movement
    //    if (moveDir.sqrMagnitude > 0.01f)
    //    {
    //        Quaternion targetRot = Quaternion.LookRotation(moveDir);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
    //    }
    //}
}

[Serializable]
public class MyPlayerContext
{
    public float currentMoveSpeed;   // smoothed speed
    public float targetMoveSpeed;    // desired speed

    public Vector2 moveInput;   // store the latest input          // store the latest input

    public float baseMoveSpeed = 5f;
    public float accel = 4;
    public Animator anim;
    public CharacterController characterController;
    public Renderer renderer;
    public Transform mainCameraTransform;
    public Transform rootTransform;

    public MyPlayerContext(
        float baseMoveSpeed, 
        float accel, 
        Animator anim, 
        CharacterController characterController, 
        Renderer renderer,
        Transform mainCameraTransform,
        Transform rootTransform)
    {
        this.baseMoveSpeed = baseMoveSpeed;
        this.accel = accel;
        this.anim = anim;
        this.characterController = characterController;
        this.renderer = renderer;
        this.mainCameraTransform = mainCameraTransform;
        this.rootTransform = rootTransform;
    }
}
