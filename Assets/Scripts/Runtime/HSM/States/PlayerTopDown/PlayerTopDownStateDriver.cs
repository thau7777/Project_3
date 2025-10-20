using HSM;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

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
    private SkillExecutor _executor;

    [SerializeField]
    private List<ClassWeaponData> classWeaponDataList;


    [SerializeField]
    List<Transform> _attackVfxSpawnPoints;
    #endregion

    #region Variables
    [SerializeField, TabGroup("Movement Settings")] private float _baseMoveSpeed = 5f;   // default
    [SerializeField, TabGroup("Movement Settings")] private float _strafeMoveSpeed = 2f;   // default
    [SerializeField, TabGroup("Movement Settings")] private float _smoothTime = 0.2f;    // higher = slower accel/dec
    [SerializeField, TabGroup("Movement Settings")] private float _rotateSpeed = 20f;

    [SerializeField]
    private PlayerTopdownContext _context;
    StateMachine machine;
    State root;

    Vector3 _rotateDirOnAttack;
    
    bool _isInSpecialMoveAnim
        => _animator.GetCurrentAnimatorStateInfo(_context.IsUseSkillByUpperBody ? 1 : 0).IsTag("SpecialMove");

    #endregion

    #region Initialization
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _executor = GetComponent<SkillExecutor>();
        _animator.runtimeAnimatorController = _locomotionSet.animationController;
        _context = new PlayerTopdownContextBuilder()
        .SetBaseMoveSpeed(_baseMoveSpeed)
        .SetStrafeMoveSpeed(_strafeMoveSpeed)
        .SetMoveSpeedSmoothTime(_smoothTime)
        .SetRotateSpeed(_rotateSpeed)
        .SetAnimator(_animator)
        .SetCharacterController(_controller)
        .SetRenderer(_renderer)
        .SetMainCameraTransform(Camera.main.transform)
        .SetRootTransform(transform)
        .SetLocomotionSet(_locomotionSet)
        .Build();


        root = new PlayerTopdownRoot(null, _context);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();

        InitializeClassWeapon(_locomotionSet.characterClass);
    }
    private void OnEnable()
    {
        _inputReader.playerTopDownActions.onMove += OnMove;
        _inputReader.playerTopDownActions.onLeftClick += OnLeftClick;
        _inputReader.playerTopDownActions.onRightClick += OnRightClick;
        _inputReader.playerTopDownActions.onSpaceBar += OnSpaceBar;
        _inputReader.playerTopDownActions.onButtonQ += OnButtonQ;
    }
    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onMove -= OnMove;
        _inputReader.playerTopDownActions.onLeftClick -= OnLeftClick;
        _inputReader.playerTopDownActions.onRightClick -= OnRightClick;
        _inputReader.playerTopDownActions.onSpaceBar -= OnSpaceBar;
        _inputReader.playerTopDownActions.onButtonQ -= OnButtonQ;
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
        UseSKill(0, value, SaveDirToAttack);
    }
    private void OnSpaceBar(bool value)
    {
        UseSKill(1, value, SaveDirToAttack);
    }
    private void OnButtonQ(bool value)
    {
        UseSKill(2, value, SaveDirToAttack);
    }
    private void UseSKill(int skillIndex, bool isPressed, Action onCastInstantly = null)
    {

        if (_isInSpecialMoveAnim || _context.IsInSpecialMove || 
            _context.CastingSkill != -1 && _context.CastingSkill != skillIndex) return;

        if (isPressed)
        {
            // use first skill so we put in 0
            _executor.UseSkill(skillIndex, _locomotionSet.characterClass, _context, onCastInstantly);
        }
        else if (_context.IsStrafing) // unleash the right mouse 
        {
            _executor.CastSkill(_context);
        }

    }
    private void OnMove(Vector2 vector)
    {
        _context.MoveInput = vector;
    }
    public void OnLeftClick()
    {
        SaveDirToAttack();
        if (_context.IsNextAttackQueued || _context.IsInSpecialMove) return;
        // lay cai dau tien va neu la queue thi lay cai thu 2 va luu o dau do
        if (_context.IsStrafing) // unleash the right mouse 
        {
            _executor.CastSkill(_context);
            return;
        }
        if (!_context.IsAttacking)
        {
            _context.IsAttacking = true;
            _animator.Play(_locomotionSet.FirstComboAttack.animName, _context.IsRangeClass ? 1 : 0,0);
        }
        else
        {
            _context.IsNextAttackQueued = true;
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
        if (_context.IsDashing)
        {
            _context.CurrentMoveSpeed = 0; // reset current speed
            _context.RotateDir = _context.DesiredMoveDir == Vector3.zero ? transform.forward : _context.DesiredMoveDir;
            return;
        }
        _context.RotateDir = _rotateDirOnAttack; // face the saved dir
    }
    public void ApplyDash()
    {
        _context.CurrentMoveSpeed = 0; // reset current speed
        _context.MoveSpeedSmoothTime = 0.05f; // quick accel
        bool isDashForward;
        float dashForce;
        if (_context.IsInSpecialMove)
        {
            isDashForward = _executor.StoredSkillData.Value.isDashForward;
            dashForce = _executor.StoredSkillData.Value.dashForce;
        }
        else
        {
            isDashForward = _locomotionSet.CurrentAttackData.isDashForward;
            dashForce = _locomotionSet.CurrentAttackData.dashForce;
        }
            
        _context.TargetMoveSpeed = dashForce;
        
        _context.MoveDir = _context.IsDashing ? (_context.DesiredMoveDir == Vector3.zero ? transform.forward : _context.DesiredMoveDir) : _rotateDirOnAttack.normalized * (isDashForward ? 1 : -1);
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
                slashVFX.Initialize(spawnPoint.position, transform.rotation);
                if (slashVFX is StraightProjectile)
                {
                    var straightProjectile = slashVFX as StraightProjectile;
                    straightProjectile.InitializeMovement(transform.forward, 10);
                }
                break;
            }
        }
    }
    public void OnSkillDone()
    {
        _context.IsInSpecialMove = false;
        _context.IsStrafing = false;
        _context.CastingSkill = -1;
        _context.IsDashing = false;
        
    }
    public void OnAttackDone()
    {
        if (!_context.IsNextAttackQueued)
        {
            _context.IsAttacking = false;
            return;
        }
        // execute cai queued combo
        _context.IsNextAttackQueued = false;
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



