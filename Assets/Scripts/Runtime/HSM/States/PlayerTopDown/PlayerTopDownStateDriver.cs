using HSM;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[Serializable]
public struct ClassWeaponData
{
    public CharacterClass Class;
    public List<GameObject> weaponOfThisClass;
}


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerTopDownStateDriver : MonoBehaviour
{
    #region References
    [SerializeField, Required]
    [TabGroup("References")]
    private InputReader _inputReader;

    [SerializeField, Required]
    [TabGroup("References")]
    private LocomotionSet _locomotionSet; 

    [SerializeField, Required]
    [TabGroup("References")]
    private CharacterController _controller;

    [SerializeField, Required]
    [TabGroup("References")]
    private Animator _animator;
     
    [SerializeField, Required]
    [TabGroup("References")]
    private Renderer _renderer;

    [SerializeField, Required]
    [TabGroup("References")]
    private SkillExecutor _executer;

    [SerializeField]
    private List<ClassWeaponData> classWeaponDataList;


    [SerializeField]
    List<Transform> _attackVfxSpawnPoints;
    #endregion

    #region Variables
    [SerializeField, TabGroup("Movement Settings")] private float baseMoveSpeed = 5f;   // default
    [SerializeField, TabGroup("Movement Settings")] private float strafeMoveSpeed = 2f;   // default
    [SerializeField, TabGroup("Movement Settings")] private float smoothTime = 0.2f;    // higher = slower accel/dec

    [SerializeField]
    private PlayerContext _context;
    StateMachine machine;
    State root;

    Vector3 _rotateDirOnAttack;
    bool _isNextAttackQueued = false;
    bool _isStillInAttackAnim => _animator.GetCurrentAnimatorStateInfo(_context.IsRangeClass ? 1 : 0).IsTag("Attack");
    bool _isUseSkillByUpperBody = false;
    bool _isInSpecialMoveAnim
        => _animator.GetCurrentAnimatorStateInfo(_isUseSkillByUpperBody ? 1 : 0).IsTag("SpecialMove");

    #endregion

    #region Initialization
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _executer = GetComponent<SkillExecutor>();
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
        _inputReader.playerTopDownActions.onLeftClick += OnLeftClick;
        _inputReader.playerTopDownActions.onRightClick += OnRightClick;
    }
    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onMove -= OnMove;
        _inputReader.playerTopDownActions.onLeftClick -= OnLeftClick;
        _inputReader.playerTopDownActions.onRightClick -= OnRightClick;
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
    private void OnRightClick(bool value)
    {
        if (_isInSpecialMoveAnim || _context.IsInSpecialMove) return;

        if (value)
        {
            // use first skill so we put in 0
            Action onCastInstantly = null;
            onCastInstantly += SaveDirToAttack;
            _isUseSkillByUpperBody = _executer.UseSkill(0, _locomotionSet.characterClass, _context, onCastInstantly);
            _isNextAttackQueued = false;
        }
        else if(_context.IsStrafing) // unleash the right mouse 
        {
            _executer.CastSkill(_context);
        }
        

    }
    private void OnMove(Vector2 vector)
    {
        _context.MoveInput = vector;
    }
    public void OnLeftClick()
    {
        SaveDirToAttack();
        if (_isNextAttackQueued || _context.IsInSpecialMove) return;
        // lay cai dau tien va neu la queue thi lay cai thu 2 va luu o dau do
        if (!_context.IsAttacking)
        {
            _context.IsAttacking = true;
            _animator.Play(_locomotionSet.FirstComboAttack.animName, _context.IsRangeClass ? 1 : 0,0);
        }
        else
        {
            _isNextAttackQueued = true;
            _locomotionSet.QueueNextComboAttack();
        }
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
                _rotateDirOnAttack = lookDir;
            }
        }
    }
    #endregion

    #region Loop
    private void Update()
    {
        machine.Tick(Time.deltaTime);
    }

    #endregion

    #region Animation Events
    public void OnAttackAnimStart()
    {
        _context.RotateDir = _rotateDirOnAttack; // face the saved dir
    }
    public void ApplyDash()
    {
        _context.MoveSpeedSmoothTime = 0.05f; // quick accel
        bool isDashForward;
        float dashForce;
        if (_context.IsInSpecialMove)
        {
            isDashForward = _executer.StoredSkillData.Value.isDashForward;
            dashForce = _executer.StoredSkillData.Value.dashForce;
        }
        else
        {
            isDashForward = _locomotionSet.CurrentAttackData.isDashForward;
            dashForce = _locomotionSet.CurrentAttackData.dashForce;
        }
            
        _context.MoveDir = _rotateDirOnAttack.normalized * (isDashForward ? 1 : -1);
        _context.TargetMoveSpeed = dashForce;
    }
    public void StopMoving()
    {
        _context.TargetMoveSpeed = 0; // stop moving when attacking
        _context.MoveSpeedSmoothTime = 0.1f;
    }
    
    public void OnAttackTrigger()
    {
        SpawnVFX(_locomotionSet.CurrentAttackData.flyweightSettings, _locomotionSet.CurrentAttackData.spawnLocation);
    }
    private void SpawnVFX(FlyweightSettings flyweightSettings, VFXSpawnLocation location)
    {
        foreach(var spawnPoint in _attackVfxSpawnPoints)
        {
            if(location.ToString() == spawnPoint.name)
            {
                Flyweight slashVFX = FlyweightFactory.Spawn(flyweightSettings);
                slashVFX.transform.position = spawnPoint.position;
                slashVFX.transform.rotation = transform.rotation;
                break;
            }
        }
    }
    public void OnSkillDone()
    {
        _context.IsInSpecialMove = false;
        if (_context.IsStrafing) _context.IsStrafing = false;
    }
    public void OnAttackDone()
    {
        if (_context.IsInSpecialMove) return;
        if (!_isNextAttackQueued)
        {
            _context.IsAttacking = false;
            return;
        }
        // execute cai queued combo
        _isNextAttackQueued = false;
        _animator.Play(_locomotionSet.QueuedAttackData.animName, _context.IsRangeClass ? _context.UpperBodyLayerIndex : 0,0);
    }
    public void OnAttackAnimExit()
    {
        if (_context.IsAttacking)
            return;
        _locomotionSet.ResetAttackAnimCycle();
    }
    #endregion
}

[Serializable]
public class PlayerContext
{
    [field: SerializeField] public float CurrentMoveSpeed { get; set; }   // smoothed speed
    [field: SerializeField] public float TargetMoveSpeed { get; set; }    // desired speed

    [field: SerializeField] public Vector2 MoveInput { get; set; }        // latest input
    [field: SerializeField] public Vector3 MoveDir { get; set; }
    [field: SerializeField] public Vector3 RotateDir { get; set; }
    [field: SerializeField] public float RotateSpeed { get; set; }
    [field: SerializeField] public bool IsStrafing { get; set; }          // player aiming

    [field: SerializeField] public bool IsAttacking { get; set; }
    [field: SerializeField] public bool IsInSpecialMove { get; set; }
    [field: SerializeField] public LocomotionSet LocomotionSet { get; set; }
    [field: SerializeField] public Vector2 MousePosOnClick { get; set; }

    [field: SerializeField] public float BaseMoveSpeed { get; set; }
    [field: SerializeField] public float StrafeMoveSpeed { get; set; }
    [field: SerializeField] public float MoveSpeedSmoothTime { get; set; }

    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public CharacterController CharacterController { get; set; }
    [field: SerializeField] public Renderer Renderer { get; set; }
    [field: SerializeField] public Transform MainCameraTransform { get; set; }
    [field: SerializeField] public Transform RootTransform { get; set; }

    public CharacterClass CharacterClass => LocomotionSet.characterClass;
    public bool IsRangeClass => CharacterClass == CharacterClass.Mage || CharacterClass == CharacterClass.Bow;

    [field: SerializeField] public bool NeedHoldStill { get; set; }

    public int StrafeStateHash => Animator.StringToHash("Strafe");
    public int MovementStateHash => Animator.StringToHash("Movement");
    public int MoveSpeedHash => Animator.StringToHash("MoveSpeed");
    public int InputXHash => Animator.StringToHash("MoveDirX");
    public int InputYHash => Animator.StringToHash("MoveDirY");
    public int UpperBodyLayerIndex => 1;

    [field: SerializeField] public float NextAnimCrossFadeTime { get; set; }

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
        BaseMoveSpeed = baseMoveSpeed;
        StrafeMoveSpeed = strafeMoveSpeed;
        MoveSpeedSmoothTime = smoothTime;
        Animator = anim;
        CharacterController = characterController;
        Renderer = renderer;
        MainCameraTransform = mainCameraTransform;
        RootTransform = rootTransform;
        LocomotionSet = locomotionSet;

        IsStrafing = false;
        IsAttacking = false;
        RotateDir = Vector3.zero;
        RotateSpeed = 20f;
    }
}



