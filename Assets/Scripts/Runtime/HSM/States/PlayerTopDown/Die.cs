using UnityEngine;
using HSM;

public class Die : State
{
    readonly PlayerTopdownContext ctx;
    public Die(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }
    protected override void OnEnter()
    {
        ctx.Animator.CrossFade(ctx.DieStateHash, 0.1f);
        ctx.CurrentMoveSpeed = 0f;
        ctx.TargetMoveSpeed = 0f;
    }
}
