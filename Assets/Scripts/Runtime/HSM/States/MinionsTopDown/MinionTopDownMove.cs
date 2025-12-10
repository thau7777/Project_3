using HSM;
using UnityEngine;

public class MinionTopDownMove : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownMove(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        ctx.CurrentSpeed = ctx.BaseMoveSpeed;

        ctx.Animator.CrossFade(ctx.MoveHash, 0.1f);
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.MoveDir = ctx.MoveTargetPosition - ctx.RootTransform.position;
        ((MinionTopDownRoot)Parent).UpdateRotation(deltaTime, ctx.MoveTargetPosition);
    }

    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((MinionTopDownRoot)Parent).Dead;
        }
        if (ctx.IsHurting)
        {
            return ((MinionTopDownRoot)Parent).Hurt;
        }
        if (ctx.IsEnemyInAttackRange())
        {
            return ((MinionTopDownRoot)Parent).Attack;
        }
        if (!ctx.NeedToMove())
        {
            return ((MinionTopDownRoot)Parent).Idle;

        }

        return null;
    }
}
