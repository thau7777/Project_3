using UnityEngine;
using HSM;
public class EnemyTopdownAttack : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownAttack(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
 
    protected override void OnEnter()
    {
        if (ctx.CurrentEnemyAttackData.lockMovementState != EnemyAttackData.LockMovementState.ExecutingOnly &&
            ctx.CurrentEnemyAttackData.lockMovementState != EnemyAttackData.LockMovementState.Full)
            ctx.CurrentSpeed = ctx.CurrentEnemyAttackData.movementSpeed;
        else
            ctx.CurrentSpeed = 0;

        if (ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.ExecutingOnly ||
            ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.Full)
            ctx.RotateSpeed = ctx.CurrentEnemyAttackData.rotateSpeed;
        else
            ctx.RotateSpeed = ctx.BaseRotateSpeed;

        ctx.IsAttacking = true;
        ctx.Animator.CrossFade(ctx.CurrentEnemyAttackData.executeAnimationName, 0,0);
    }protected override void OnUpdate(float deltaTime)
    {
        if(ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.ExecutingOnly ||
            ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.Full)
            ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, ctx.CurrentTargetTransform.position);

        if (ctx.CurrentEnemyAttackData.lockMovementState != EnemyAttackData.LockMovementState.ExecutingOnly &&
            ctx.CurrentEnemyAttackData.lockMovementState != EnemyAttackData.LockMovementState.Full)
            ctx.MoveDir = (ctx.CurrentTargetTransform.position - ctx.RootTransform.position).normalized;
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
        if (!ctx.IsAttacking)
        {
            if (ctx.EnemyType == EnemyTopdownMovementType.Range)
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
                if(ctx.DistanceToTarget < 1.5f)
                    return ((EnemyTopdownRoot)Parent).Idle;
                return ((EnemyTopdownRoot)Parent).Move;
                // Continue moving to get into optimal range
            }
            else
            {
                if (ctx.DistanceToTarget < 1.5f)
                    return ((EnemyTopdownRoot)Parent).Idle;

                return ((EnemyTopdownRoot)Parent).Move;

            }
        }
        return null;
    }
}
