using System;
using UnityEngine;

[Serializable]
public class PlayerTopdownContext
{
    // Core properties
    public float CurrentMoveSpeed { get; set; }   // smoothed speed
    [field: SerializeField] public float TargetMoveSpeed { get; set; }    // desired speed
    public Vector3 KnockBackDirection { get; set; }
    public float KnockbackForce { get; set; }
    [field: SerializeField] public Vector2 MoveInput { get; set; }
    public Vector3 MoveDir { get; set; }
    public Vector3 DesiredMoveDir { get; set; }
    public Vector3 RotateDir { get; set; }
    public float RotateSpeed { get; private set; }

    // States

    [field: SerializeField] public bool IsAiming { get; set; }
    [field: SerializeField] public bool IsDashing { get; set; }
    [field: SerializeField] public bool IsAttacking { get; set; }
    [field: SerializeField] public bool IsInSpecialMove { get; set; }
    [field: SerializeField] public bool IsHurting { get; set; }
    [field: SerializeField] public bool IsDead { get; set; }
    [field: SerializeField] public bool IsNextAttackQueued { get; set; }
    public string FirstAttackAnimName { get; set; }
    public string SkillAnimName { get; set; }
    public string AimAnimName { get; set; }
    public bool IsUseSkillByUpperBody { get; set; }
    public bool CanRotateWhileUsingSkill{ get; set; }
    public bool NeedHoldStillWhileExecuteWhenAiming { get; set; }
    [field: SerializeField] public int CastingSkill { get; set; } = -1;

    // Movement config
    public float BaseMoveSpeed { get; set; }
    public float StrafeMoveSpeed { get; private set; }
    public float MoveSpeedSmoothTime { get; set; }

    // References
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public Renderer Renderer { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    public Transform RootTransform { get; private set; }
    public LocomotionSet LocomotionSet { get; private set; }

    // Misc
    public Vector2 MousePosOnClick { get; set; }

    // Derived properties
    public CharacterClass CharacterClass => LocomotionSet.characterClass;
    public bool IsRangeClass => CharacterClass == CharacterClass.Mage || CharacterClass == CharacterClass.Summoner;

    // Cached Animator Hashes
    public int StrafeStateHash => Animator.StringToHash("Strafe");
    public int MovementStateHash => Animator.StringToHash("Movement");
    public int HurtStateHash => Animator.StringToHash("Hurt");
    public int DieStateHash => Animator.StringToHash("Die");
    public int MoveSpeedHash => Animator.StringToHash("MoveSpeed");
    public int InputXHash => Animator.StringToHash("MoveDirX");
    public int InputYHash => Animator.StringToHash("MoveDirY");
    public int UpperBodyLayerIndex => 1;

    private PlayerTopdownContext() { }

    // -------------------------------------
    // Builder
    // -------------------------------------
    public class Builder
    {
        private readonly PlayerTopdownContext ctx = new PlayerTopdownContext();

        public Builder SetBaseMoveSpeed(float value)
        {
            ctx.BaseMoveSpeed = value;
            return this;
        }

        public Builder SetStrafeMoveSpeed(float value)
        {
            ctx.StrafeMoveSpeed = value;
            return this;
        }

        public Builder SetMoveSpeedSmoothTime(float value)
        {
            ctx.MoveSpeedSmoothTime = value;
            return this;
        }

        public Builder SetRotateSpeed(float speed)
        {
            ctx.RotateSpeed = speed;
            return this;
        }

        public Builder SetAnimator(Animator animator)
        {
            ctx.Animator = animator;
            return this;
        }

        public Builder SetCharacterController(CharacterController controller)
        {
            ctx.CharacterController = controller;
            return this;
        }

        public Builder SetRenderer(Renderer renderer)
        {
            ctx.Renderer = renderer;
            return this;
        }

        public Builder SetMainCameraTransform(Transform camera)
        {
            ctx.MainCameraTransform = camera;
            return this;
        }

        public Builder SetRootTransform(Transform root)
        {
            ctx.RootTransform = root;
            return this;
        }

        public Builder SetLocomotionSet(LocomotionSet locomotionSet)
        {
            ctx.LocomotionSet = locomotionSet;
            return this;
        }
        public PlayerTopdownContext Build()
        {
            if (ctx.Animator == null)
                Debug.LogWarning("PlayerTopdownContext built without Animator.");
            if (ctx.RootTransform == null)
                Debug.LogWarning("PlayerTopdownContext built without RootTransform.");


            return ctx;
        }
    }
}
