using HSM;
using UnityEngine;

public class MinionTopDownIdle : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownIdle(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }

    protected override void OnEnter()
    {
        ctx.CurrentSpeed = 0;
        ctx.Animator.CrossFade(ctx.IdleHash, 0.1f);
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
        if (ctx.NeedToMove())
            return ((MinionTopDownRoot)Parent).Move;

        return null;
    }
}
