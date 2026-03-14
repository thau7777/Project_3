using UnityEngine;
using HSM;
public class Despawn : State
{
    readonly PlayerTopdownContext ctx;
    public Despawn(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }
    protected override void OnEnter()
    {
        ctx.Animator.CrossFade(ctx.VictoryStateHash, 0.1f);
        ctx.CurrentMoveSpeed = 0f;
        ctx.TargetMoveSpeed = 0f;
    }
}
