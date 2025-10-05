using HSM;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct ClassWeaponData
{
    public CharacterClass Class;
    public List<GameObject> weaponOfThisClass;
}
[Serializable]
public struct  SlashVFXSpawnPoint
{
    public FlyweightType flyweightType;
    public Transform spawnPoint;
}
public enum MageAimType 
{ 
    Straight,
    Above,
    Below
}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerTopDownStateDriver : MonoBehaviour
{
    #region References
    [SerializeField, Required]
    [FoldoutGroup("References")]
    private InputReader _inputReader;

    [SerializeField, Required]
    [FoldoutGroup("References")]
    private LocomotionSet _locomotionSet; 

    [SerializeField, Required]
    [FoldoutGroup("References")]
    private CharacterController _controller;

    [SerializeField, Required]
    [FoldoutGroup("References")]
    private Animator _animator;
     
    [SerializeField, Required]
    [FoldoutGroup("References")]
    private Renderer _renderer;

    [SerializeField]
    private List<ClassWeaponData> classWeaponDataList = new List<ClassWeaponData>();

    [SerializeField]
    List<FlyweightSettings> _flyweightSettings = new List<FlyweightSettings>();

    [SerializeField]
    List<SlashVFXSpawnPoint> slashVFXSpawnPoints = new List<SlashVFXSpawnPoint>();
    #endregion

    #region Variables
    [SerializeField, FoldoutGroup("Movement Settings")] private float baseMoveSpeed = 5f;   // default
    [SerializeField, FoldoutGroup("Movement Settings")] private float strafeMoveSpeed = 2f;   // default
    [SerializeField, FoldoutGroup("Movement Settings")] private float smoothTime = 0.2f;    // higher = slower accel/dec

    [SerializeField]
    private PlayerContext _context;
    StateMachine machine;
    State root;

    Vector3 rotateDirOnAttack;
    bool _isNextAttackQueued = false;
    bool _isStillInAttackAnim => _animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") ||
        _animator.GetCurrentAnimatorStateInfo(_context.upperBodyLayerIndex).IsTag("Attack");

    #endregion

    #region Initialization
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _animator.runtimeAnimatorController = _locomotionSet.animationController;
        _context = new PlayerContext(
            baseMoveSpeed,
            strafeMoveSpeed,
            smoothTime, 
            _animator, 
            _controller, 
            _renderer,
            Camera.main.transform,
            transform,
            _locomotionSet);

        root = new PlayerRoot(null, _context);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();

        InitializeClassWeapon(_locomotionSet.characterClass);
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
    private void InitializeClassWeapon(CharacterClass characterClass)
    {
        foreach (var classWeaponData in classWeaponDataList)
        {
            bool isActiveClass = classWeaponData.Class == characterClass;
            foreach (var weapon in classWeaponData.weaponOfThisClass)
            {
                if(weapon != null)
                    weapon.SetActive(isActiveClass);
            }
        }
    }
    #endregion

    #region Input Handlers
    private void OnAim(bool value)
    {
        if (_context.isAttacking || _isStillInAttackAnim) return;

        if (_context.IsRangeClass)
        {
            _context.isStrafing = value;
        }
        else 
        {
            _context.isInSpecialAction = value;
        }

    }

    private void OnMove(Vector2 vector)
    {
        _context.moveInput = vector;
    }
    public void OnAttack()
    {
        if (_context.isCastingSpell || _animator.GetCurrentAnimatorStateInfo(_context.upperBodyLayerIndex).IsTag("CastSpell"))
        {
            Debug.Log("Attack ignored: already casting spell");
            return;
        }
            
        SaveDirToAttack();
        if (_context.isStrafing)
        {
            string spellToCast = "Cast"+_context.aimType.ToString();
            _context.nextAttackAnim = spellToCast;
            _context.isAttacking = true;
            return;
        }
        if (_isNextAttackQueued || _locomotionSet.IsNextAttackNull ||
            _isStillInAttackAnim && !_context.isAttacking) return;

        var nextAnim = _locomotionSet.GetNextAttackAnim();
        _context.attackDashForce = nextAnim.dashForce;
        _context.nextAttackAnim = nextAnim.animName;

        if (!_context.isAttacking)
            _context.isAttacking = true;
        else
            _isNextAttackQueued = true;
    }

    private void SaveDirToAttack()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            Vector3 hitPoint = ray.GetPoint(hitDist);

            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f; // keep it flat on ground

            if (lookDir.sqrMagnitude > 0.001f)
            {
                rotateDirOnAttack = lookDir;
            }
        }
    }
    #endregion

    #region Loop
    private void Update()
    {
        machine.Tick(Time.deltaTime);
        SetInputValueSmoothly();
    }

    private void SetInputValueSmoothly()
    {
        // lerp animator input params
        float currentX = _animator.GetFloat(_context.inputXHash);
        float currentY = _animator.GetFloat(_context.inputYHash);

        float smoothedX = Mathf.Lerp(currentX, _context.moveInput.x, Time.deltaTime * 10f);
        float smoothedY = Mathf.Lerp(currentY, _context.moveInput.y, Time.deltaTime * 10f);

        _animator.SetFloat(_context.inputXHash, smoothedX);
        _animator.SetFloat(_context.inputYHash, smoothedY);
    }
    #endregion

    #region Animation Events
    public void OnAttackAnimStart()
    {
        _context.rotateDir = rotateDirOnAttack; // face the saved dir
        if (_context.IsRangeClass)
            return;
        _context.targetMoveSpeed = _context.attackDashForce;
        _context.moveDir = _context.rotateDir.normalized;
    }
    public void OnAttackTrigger(FlyweightType type)
    {
        SpawnSlashVFX(type, transform.position);

        if (_context.IsRangeClass) return;

        _context.targetMoveSpeed = 0; // stop moving when attacking
    }
    private void SpawnSlashVFX(FlyweightType type, Vector3 pos)
    {
        foreach(var spawnPoint in slashVFXSpawnPoints)
        {
            if(spawnPoint.flyweightType == type)
            {
                Flyweight slashVFX = FlyweightFactory.Spawn(GetSlashVfxPrefab(type));
                slashVFX.transform.position = spawnPoint.spawnPoint.position;
                slashVFX.transform.rotation = transform.rotation;
                break;
            }
        }
    }
    private FlyweightSettings GetSlashVfxPrefab(FlyweightType type)
    {
        foreach(var data in _flyweightSettings)
        {
            if(data.type == type)
            {
                Debug.Log("Found slash VFX prefab for type: " + type);
                return data;
            }
        }
        return null;
    }
    public void ExecuteNextAttack()
    {
        if (!_isNextAttackQueued)
        {
            _context.isAttacking = false;
            if(_context.isCastingSpell)
                _context.isCastingSpell = false;
            return;
        }
        _isNextAttackQueued = false;
        if(_context.characterClass == CharacterClass.Mage)
            _animator.CrossFadeInFixedTime(_context.nextAttackAnim, _context.nextAnimCrossFadeTime, _context.upperBodyLayerIndex, 0f);
        else
            _animator.CrossFadeInFixedTime(_context.nextAttackAnim, _context.nextAnimCrossFadeTime, 0, 0f);

    }
    public void OnAttackAnimEnd()
    {
        if (_context.isAttacking)
            return;
        _context.nextAttackAnim = null;
        _locomotionSet.ResetAttackAnimCycle();
    }
    #endregion
}

[Serializable]
public class PlayerContext
{
    public float currentMoveSpeed;   // smoothed speed
    public float targetMoveSpeed;    // desired speed

    public Vector2 moveInput;   // store the latest input
    public Vector3 moveDir;
    public Vector3 rotateDir;
    public float rotateSpeed;
    public bool isStrafing;     // store whether the player is aiming
    public bool isInSpecialAction;

    public bool isAttacking;
    public bool isCastingSpell;
    public LocomotionSet locomotionSet;
    public string nextAttackAnim;
    public Vector2 mousePosOnClick;
    public float attackDashForce;

    public float baseMoveSpeed;
    public float strafeMoveSpeed;
    public float moveSpeedSmoothTime;
    public Animator anim;
    public CharacterController characterController;
    public Renderer renderer;
    public Transform mainCameraTransform;
    public Transform rootTransform;
    public CharacterClass characterClass => locomotionSet.characterClass;
    public bool IsRangeClass => characterClass == CharacterClass.Mage || characterClass == CharacterClass.Bow;

    public readonly int strafeStateHash = Animator.StringToHash("Strafe");
    public readonly int movementStateHash = Animator.StringToHash("Movement");

    public readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    public readonly int inputXHash = Animator.StringToHash("InputX");
    public readonly int inputYHash = Animator.StringToHash("InputY");
    public readonly int upperBodyLayerIndex;
    public MageAimType aimType = MageAimType.Below;
    public float nextAnimCrossFadeTime;
    public PlayerContext(
        float baseMoveSpeed, 
        float strafeMoveSpeed,
        float smoothTime, 
        Animator anim, 
        CharacterController characterController, 
        Renderer renderer,
        Transform mainCameraTransform,
        Transform rootTransform,
        LocomotionSet locomotionSet)
    {
        this.baseMoveSpeed = baseMoveSpeed;
        this.strafeMoveSpeed = strafeMoveSpeed;
        this.moveSpeedSmoothTime = smoothTime;
        this.anim = anim;
        this.characterController = characterController;
        this.renderer = renderer;
        this.mainCameraTransform = mainCameraTransform;
        this.rootTransform = rootTransform;
        upperBodyLayerIndex = anim.GetLayerIndex("UpperBody");

        isStrafing = false;
        isAttacking = false;
        rotateDir = Vector3.zero;
        rotateSpeed = 20f;
        this.locomotionSet = locomotionSet;
    }
}


