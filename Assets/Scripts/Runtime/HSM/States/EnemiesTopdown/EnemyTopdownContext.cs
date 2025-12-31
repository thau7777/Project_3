using System.Collections.Generic;
using UnityEngine;
public enum EnemyTopdownKind
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
    [field: SerializeField] public Transform CurrentTargetTransform { get; private set; }
    public float BaseMoveSpeed { get; private set; }
    public float MovePauseDuration { get; private set; }
    [field: SerializeField]
    public float CurrentSpeed { get; set; }
    public float RotateSpeed { get; private set; }
    [field: SerializeField]
    public Vector3 MoveDir { get; set; }

    public Vector3 KnockbackDirection { get; set; }
    public float KnockbackForce { get; set; }
    public float AttackRange { get; private set; }
    public bool IsBoss { get; private set; }
    public float StunDuration { get; set; }

    public Transform PlayerTransform { get; private set; }

    // State Properties
    [field: SerializeField] public bool IsDoneMoving { get; set; }
    [field: SerializeField] public bool IsDoneAttacking { get; set; }
    [field: SerializeField] public bool IsInSpecialMove { get; set; }
    [field: SerializeField] public bool IsHurting { get; set; }
    [field: SerializeField] public bool IsMoreHurt { get; set; }
    [field: SerializeField] public bool IsStunned { get; set; }
    [field: SerializeField] public bool IsDead { get; set; }

    // Cached Animator Hashes
    public int IdleHash => Animator.StringToHash("Idle");
    public int MoveHash => Animator.StringToHash("Move");
    public int AttackHash => Animator.StringToHash("Attack");
    public int HurtHash => Animator.StringToHash("Hurt");
    public int StunnedHash => Animator.StringToHash("Stunned");
    public int DeadHash => Animator.StringToHash("Dead");

    public EnemyTopdownKind EnemyType { get; set; }

    public List<EnemySpecialMoveData> EnemySpecialMoveList { get; private set; }
    public EnemySpecialMoveData EnemySpecialMoveData { get; private set; }
    public float BossAttackCoolDownTimer { get; set; }
    public float BossAttackCoolDown { get; private set; } = 2;
    public bool IsTargetInAttackRange()
    {
        var distanceToTarget = Vector3.Distance(CurrentTargetTransform.position, RootTransform.position);
        return distanceToTarget <= AttackRange;
    }

    public void SetCurrentTarget(Transform target)
    {
        CurrentTargetTransform = target;
    }
    public bool CheckAndPickRandomAttack()
    {
        if (BossAttackCoolDownTimer < BossAttackCoolDown) return false;

        var index = Random.Range(0, EnemySpecialMoveList.Count);
        PlaySpecialMoveData(EnemySpecialMoveList[index]);
        return true;
    }
    public void PlaySpecialMoveData(EnemySpecialMoveData data)
    {
        EnemySpecialMoveData = data;
        IsInSpecialMove = true;
    }

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

        public Builder SetPlayerTransform(Transform target)
        {
            ctx.PlayerTransform = target;
            ctx.SetCurrentTarget(target);
            return this;
        }

        public Builder SetMoveSpeed(float distance)
        {
            ctx.BaseMoveSpeed = distance;
            return this;
        }

        public Builder SetMovePauseDuration(float duration)
        {
            ctx.MovePauseDuration = duration;
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

        public Builder SetEnemyType(EnemyTopdownKind type)
        {
            ctx.EnemyType = type;
            return this;
        }
        public Builder SetIsBoss(bool value)
        {
            ctx.IsBoss = value;
            return this;
        }
        public Builder SetSpecialMoveList(List<EnemySpecialMoveData> moves)
        {
            ctx.EnemySpecialMoveList = moves;
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
