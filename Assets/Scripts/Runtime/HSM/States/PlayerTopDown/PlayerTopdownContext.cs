using HSM;
using System;
using UnityEngine;

[Serializable]
public class PlayerTopdownContext
{
    [field: SerializeField] public float CurrentMoveSpeed { get; set; }   // smoothed speed
    [field: SerializeField] public float TargetMoveSpeed { get; set; }    // desired speed

    [field: SerializeField] public Vector2 MoveInput { get; set; }        // latest input
    [field: SerializeField] public Vector3 MoveDir { get; set; }
    [field: SerializeField] public Vector3 DesiredMoveDir { get; set; }
    [field: SerializeField] public Vector3 RotateDir { get; set; }
    [field: SerializeField] public float RotateSpeed { get; set; }
    [field: SerializeField] public bool IsStrafing { get; set; }          // player aiming
    [field: SerializeField] public bool IsDashing { get; set; }
    [field: SerializeField] public int CastingSkill { get; set; }


    [field: SerializeField] public bool IsAttacking { get; set; }
    [field: SerializeField] public bool IsInSpecialMove { get; set; }
    [field: SerializeField] public bool IsNextAttackQueued { get; set; }
    [field: SerializeField] public bool IsUseSkillByUpperBody { get; set; }
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

    public PlayerTopdownContext(
        float baseMoveSpeed,
        float strafeMoveSpeed,
        float smoothTime,
        float rotateSpeed,
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
    }
}

public class PlayerTopdownContextBuilder
{
    private float _baseMoveSpeed;
    private float _strafeMoveSpeed;
    private float _moveSpeedSmoothTime;
    private Animator _animator;
    private CharacterController _characterController;
    private Renderer _renderer;
    private Transform _mainCameraTransform;
    private Transform _rootTransform;
    private LocomotionSet _locomotionSet;
    private float _rotateSpeed;

    // Optional parameters with defaults
    private bool _isStrafing = false;
    private bool _isAttacking = false;
    private bool _isInSpecialMove = false;
    private int _castingSkill = -1;
    private bool _needHoldStill = false;
    private Vector3 _rotateDir = Vector3.zero;
    private float _nextAnimCrossFadeTime = 0.1f;

    public PlayerTopdownContextBuilder SetBaseMoveSpeed(float value)
    {
        _baseMoveSpeed = value;
        return this;
    }

    public PlayerTopdownContextBuilder SetStrafeMoveSpeed(float value)
    {
        _strafeMoveSpeed = value;
        return this;
    }

    public PlayerTopdownContextBuilder SetMoveSpeedSmoothTime(float value)
    {
        _moveSpeedSmoothTime = value;
        return this;
    }

    public PlayerTopdownContextBuilder SetAnimator(Animator animator)
    {
        _animator = animator;
        return this;
    }

    public PlayerTopdownContextBuilder SetCharacterController(CharacterController controller)
    {
        _characterController = controller;
        return this;
    }

    public PlayerTopdownContextBuilder SetRenderer(Renderer renderer)
    {
        _renderer = renderer;
        return this;
    }

    public PlayerTopdownContextBuilder SetMainCameraTransform(Transform camera)
    {
        _mainCameraTransform = camera;
        return this;
    }

    public PlayerTopdownContextBuilder SetRootTransform(Transform root)
    {
        _rootTransform = root;
        return this;
    }

    public PlayerTopdownContextBuilder SetLocomotionSet(LocomotionSet locomotionSet)
    {
        _locomotionSet = locomotionSet;
        return this;
    }

    public PlayerTopdownContextBuilder SetRotateSpeed(float speed)
    {
        _rotateSpeed = speed;
        return this;
    }

    public PlayerTopdownContext Build()
    {
        var ctx = new PlayerTopdownContext(
            _baseMoveSpeed,
            _strafeMoveSpeed,
            _moveSpeedSmoothTime,
            _rotateSpeed,
            _animator,
            _characterController,
            _renderer,
            _mainCameraTransform,
            _rootTransform,
            _locomotionSet
        );

        ctx.IsStrafing = _isStrafing;
        ctx.IsAttacking = _isAttacking;
        ctx.IsInSpecialMove = _isInSpecialMove;
        ctx.NeedHoldStill = _needHoldStill;
        ctx.RotateDir = _rotateDir;
        ctx.MoveDir = _rootTransform.forward; // default forward
        ctx.DesiredMoveDir = _rootTransform.forward;
        ctx.RotateSpeed = _rotateSpeed;
        ctx.NextAnimCrossFadeTime = _nextAnimCrossFadeTime;
        ctx.CastingSkill = _castingSkill;
        return ctx;
    }
}




