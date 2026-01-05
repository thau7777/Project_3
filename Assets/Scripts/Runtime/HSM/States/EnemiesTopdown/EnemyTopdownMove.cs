using UnityEngine;
using HSM;

public class EnemyTopdownMove : State
{
    readonly EnemyTopdownContext ctx;
    Vector3 _targetLastPosition;

    Vector3 _targetPosition
    {
        get
        {
            if (ctx.EnemyType == EnemyTopdownMovementType.Slime)
                return _targetLastPosition;

            if (ctx.EnemyType == EnemyTopdownMovementType.Range)
            {
                // For Range enemies, calculate position based on keeping distance
                return GetRangeEnemyTargetPosition();
            }

            return ctx.CurrentTargetTransform.position;
        }
    }

    public EnemyTopdownMove(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }

    protected override void OnEnter()
    {
        ctx.IsMoving = true;
        if (ctx.EnemyType == EnemyTopdownMovementType.Slime)
            _targetLastPosition = ctx.CurrentTargetTransform.position;

        ctx.CurrentSpeed = ctx.BaseMoveSpeed;
        ctx.Animator.CrossFade(ctx.MoveHash, 0.1f);
    }
    protected override void OnExit()
    {
        ctx.IsMoving = false;
    }

    protected override void OnUpdate(float deltaTime)
    {
        UpdateMoveDir();
        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, _targetPosition);
    }

    private void UpdateMoveDir()
    {
        ctx.MoveDir = (_targetPosition - ctx.RootTransform.position).normalized;
    }

    private Vector3 GetRangeEnemyTargetPosition()
    {
        Vector3 targetPos = ctx.CurrentTargetTransform.position;
        Vector3 currentPos = ctx.RootTransform.position;
        float distanceToTarget = Vector3.Distance(currentPos, targetPos);

        // Too close - move away from target
        if (distanceToTarget < ctx.MinRangeDistance)
        {
            // Calculate position opposite to target
            Vector3 directionAwayFromTarget = (currentPos - targetPos).normalized;
            return currentPos + directionAwayFromTarget * ctx.MinRangeDistance;
        }
        // Too far - move toward target
        return targetPos;
        //else if (distanceToTarget >= ctx.MinRangeDistance)
        //{
        //    return targetPos;
        //}
        //// In optimal range - stay in place (return current position)
        //else
        //{
        //    return currentPos;
        //}
    }

    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Dead;
        }

        if (ctx.IsStunned)
        {
            return ((EnemyTopdownRoot)Parent).Stunned;
        }

        if (ctx.IsHurting)
        {
            return ((EnemyTopdownRoot)Parent).Hurt;
        }

        // Slime enemy logic
        if (ctx.EnemyType == EnemyTopdownMovementType.Slime)
        {
            if (!ctx.IsMoving)
            {
                if (ctx.IsTargetInMaxAttackRange() && ctx.CheckAndPickRandomAttack())
                {
                    if (ctx.CurrentEnemyAttackData.needCharge)
                        return ((EnemyTopdownRoot)Parent).Charge;
                    return ((EnemyTopdownRoot)Parent).Attack;
                }
                return ((EnemyTopdownRoot)Parent).Idle;
            }
        }
        // Range enemy logic
        else if (ctx.EnemyType == EnemyTopdownMovementType.Range)
        {

            // In optimal range - can attack
            if (ctx.DistanceToTarget >= ctx.MinRangeDistance && 
                ctx.DistanceToTarget <= ctx.MaxRangeDistance && 
                ctx.CheckAndPickRandomAttack())
            {
                if (ctx.CurrentEnemyAttackData.needCharge)
                    return ((EnemyTopdownRoot)Parent).Charge;
                return ((EnemyTopdownRoot)Parent).Attack;

                // Stay in move state to maintain optimal distance
            }
            // Continue moving to get into optimal range
        }
        // Normal enemy logic
        else
        {
            if (ctx.IsTargetInMaxAttackRange() && ctx.CheckAndPickRandomAttack())
            {
                if (ctx.CurrentEnemyAttackData.needCharge)
                    return ((EnemyTopdownRoot)Parent).Charge;
                return ((EnemyTopdownRoot)Parent).Attack;
            }
            if (ctx.DistanceToTarget < 1.5f)
                return ((EnemyTopdownRoot)Parent).Idle;
        }

        return null;
    }
}
