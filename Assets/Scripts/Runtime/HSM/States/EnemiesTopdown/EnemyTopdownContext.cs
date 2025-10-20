using UnityEngine;
using HSM;
public enum EnemyTopdownType
{
    Slime,
    Normal
}
[System.Serializable]
public class EnemyTopdownContext    
{
    public Animator Animator { get; private set; }
    public CharacterController CharacterController { get; private set; }

    public Transform RootTransform { get; private set; }
    public Transform TargetTransform { get; private set; }
    public float MoveSpeed { get; private set; }
    public float RotateSpeed { get; private set; }
    public float AttackRange { get; private set; }

    // State Properties
    public bool IsDoneMoving { get; set; }
    public bool IsDoneAttacking { get; set; }

    // Cached Animator Hashes
    public int IdleHash => Animator.StringToHash("Idle");
    public int MoveHash => Animator.StringToHash("Move");
    public int AttackHash => Animator.StringToHash("Attack");
    public int HurtHash => Animator.StringToHash("Hurt");
    public int StunnedHash => Animator.StringToHash("Stunned");
    public int DeadHash => Animator.StringToHash("Dead");

    public EnemyTopdownType EnemyType { get; set; }
    public bool IsTargetInAttackRange()
    {
        var distanceToTarget = Vector3.Distance(TargetTransform.position, RootTransform.position);
        return distanceToTarget <= AttackRange;
    }

    //public State GetTransition()
    //{
    //    switch(EnemyType)
    //    {
    //        case EnemyTopdownType.Slime:
    //            // Slime specific transition logic
    //            break;
    //        case EnemyTopdownType.Normal:
    //            // Normal enemy specific transition logic
    //            break;
    //    }
    //}
    //public 

    private EnemyTopdownContext() { }

    // -----------------------------
    // Builder
    // -----------------------------
    public class Builder
    {
        private readonly EnemyTopdownContext ctx = new EnemyTopdownContext();

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

        public Builder SetRootTransform(Transform root)
        {
            ctx.RootTransform = root;
            return this;
        }

        public Builder SetTargetTransform(Transform target)
        {
            ctx.TargetTransform = target;
            return this;
        }

        public Builder SetMoveSpeed(float distance)
        {
            ctx.MoveSpeed = distance;
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

        public Builder SetEnemyType(EnemyTopdownType type)
        {
            ctx.EnemyType = type;
            return this;
        }
        public EnemyTopdownContext Build()
        {
            // Optional: validate required fields
            if (ctx.Animator == null)
                Debug.LogWarning("SmallEnemyTopdownContext built without Animator.");
            if (ctx.RootTransform == null)
                Debug.LogWarning("SmallEnemyTopdownContext built without RootTransform.");

            return ctx;
        }
    }
}
