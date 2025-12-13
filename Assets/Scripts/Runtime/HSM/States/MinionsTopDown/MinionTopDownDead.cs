using HSM;
using UnityEngine;

public class MinionTopDownDead : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownDead(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }

    protected override void OnEnter()
    {
        ctx.CurrentSpeed = 0;
        ctx.Animator.CrossFade(ctx.DieHash, 0.1f);
    }

    protected override State GetTransition()
    {
        if (!ctx.IsDead)
        {
            return ((MinionTopDownRoot)Parent).Idle;
        }
        return null;
    }
}
