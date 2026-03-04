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
            ctx.RotateSpeed = 0;

        ctx.IsAttacking = true;
        ctx.Animator.CrossFade(ctx.CurrentEnemyAttackData.executeAnimationName, 0,0);
    }protected override void OnUpdate(float deltaTime)
    {
        Vector3 direction = ctx.CurrentTargetTransform.position - ctx.RootTransform.position;
        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, direction);


        ctx.MoveDir = (ctx.CurrentTargetTransform.position - ctx.RootTransform.position).normalized;
    }
    protected override void OnExit()
    {
        ctx.CurrentSpeed = ctx.BaseMoveSpeed;
        ctx.RotateSpeed = ctx.BaseRotateSpeed;
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
                    ctx.DistanceToTarget <= ctx.MaxRangeDistance)
                {
                    if (ctx.CheckAndPickRandomAttack())
                    {
                        if (ctx.CurrentEnemyAttackData.needCharge)
                            return ((EnemyTopdownRoot)Parent).Charge;
                        return ((EnemyTopdownRoot)Parent).Attack;
                    }else
                        return ((EnemyTopdownRoot)Parent).Idle;

                }
                else// Stay in move state to maintain optimal distance
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
