using UnityEngine;

[System.Serializable]
public class MinionTopDownContext
{
    public MinionsManager.MinionKind Kind {  get; private set; }
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public Transform RootTransform { get; private set; }

    [field: SerializeField]
    public Transform EnemyTransform { get; private set; }
    public Transform OwnerTransform { get; private set; }
    public Vector3 MoveTargetPosition
    {
        get
        {
            if(EnemyTransform)
                return EnemyTransform.position;
            return OwnerTransform.position;
        }
    }
    public float BaseMoveSpeed { get; private set; }

    [field: SerializeField]
    public float CurrentSpeed { get; set; }

    public float RotateSpeed { get; private set; }

    [field: SerializeField]
    public Vector3 MoveDir { get; set; }

    // Attack 
    public Vector3 KnockbackDirection { get; set; }
    public float KnockbackForce { get; set; }
    public float AttackRange { get; private set; }

    // State Properties
    [field: SerializeField] public bool IsDoneAttacking { get; set; }
    [field: SerializeField] public bool IsHurting { get; set; }
    [field: SerializeField] public bool IsMoreHurt { get; set; }
    [field: SerializeField] public bool IsStunned { get; set; }
    [field: SerializeField] public bool IsDead { get; set; }

    // Animator Hashes
    public int IdleHash => Animator.StringToHash("Idle");
    public int MoveHash => Animator.StringToHash("Move");
    public int AttackHash1 => Animator.StringToHash("Attack1");
    public int AttackHash2 => Animator.StringToHash("Attack2");
    public int HurtHash => Animator.StringToHash("Hurt");
    public int StunnedHash => Animator.StringToHash("Stunned");
    public int DieHash => Animator.StringToHash("Die");

    private MinionTopDownContext() { }

    public void SetEnemyTransform(SummonerTargetEvent value)
    {
        EnemyTransform = value.target;
    }
    public bool IsEnemyInAttackRange()
    {
        if (!EnemyTransform) return false;
        var distanceToTarget = Vector3.Distance(EnemyTransform.position, RootTransform.position);
        return distanceToTarget <= AttackRange;
    }
    public bool NeedToMove()
    {
        if (!EnemyTransform)
        {
            var distanceToOwner = Vector3.Distance(OwnerTransform.position, RootTransform.position);
            if (distanceToOwner > 2f)
                return true;
            return false;
        }
        else
            return true;
    }
    // ----------------------------------------------------
    //                   Builder Pattern
    // ----------------------------------------------------
    public class Builder
    {
        private readonly MinionTopDownContext ctx = new MinionTopDownContext();

        public Builder SetKind(MinionsManager.MinionKind kind)
        {
            ctx.Kind = kind;
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
        public Builder SetOwner(Transform owner)
        {
            ctx.OwnerTransform = owner;
            return this;
        }
        public Builder SetRootTransform(Transform root)
        {
            ctx.RootTransform = root;
            return this;
        }


        public Builder SetMoveSpeed(float speed)
        {
            ctx.BaseMoveSpeed = speed;
            return this;
        }

        public Builder SetRotateSpeed(float speed)
        {
            ctx.RotateSpeed = speed;
            return this;
        }

        public Builder SetAttackRange(float range)
        {
            ctx.AttackRange = range;
            return this;
        }

        public MinionTopDownContext Build()
        {
            // Optional validation
            if (ctx.Animator == null)
                Debug.LogWarning("MinionTopDownContext built without Animator.");

            if (ctx.RootTransform == null)
                Debug.LogWarning("MinionTopDownContext built without RootTransform.");

            ctx.EnemyTransform = null;
            return ctx;
        }
    }
}
