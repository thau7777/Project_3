using UnityEngine;
using HSM;
public class EnemyTopdownCharge : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownCharge(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        if (ctx.CurrentEnemyAttackData.lockMovementState != EnemyAttackData.LockMovementState.ChargeOnly &&
            ctx.CurrentEnemyAttackData.lockMovementState != EnemyAttackData.LockMovementState.Full)
            ctx.CurrentSpeed = ctx.CurrentEnemyAttackData.movementSpeed;
        else
            ctx.CurrentSpeed = 0;

        if (ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.ChargeOnly ||
            ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.Full)
            ctx.RotateSpeed = ctx.CurrentEnemyAttackData.rotateSpeed;
        else
            ctx.RotateSpeed = ctx.BaseRotateSpeed;

        ctx.IsCharging = true;
        ctx.Animator.CrossFade(ctx.CurrentEnemyAttackData.chargeAnimationName, 0,0);
    }
    protected override void OnUpdate(float deltaTime)
    {
        if(ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.ChargeOnly ||
            ctx.CurrentEnemyAttackData.faceTargetState == EnemyAttackData.FaceTargetState.Full)
        {
            Vector3 direction = ctx.CurrentTargetTransform.position - ctx.RootTransform.position;
            ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, direction);
        }

        if (ctx.CurrentEnemyAttackData.lockMovementState != EnemyAttackData.LockMovementState.ChargeOnly &&
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
        if (!ctx.IsCharging)
        {
            return ((EnemyTopdownRoot)Parent).Attack;
        }
        return null;
    }
}
