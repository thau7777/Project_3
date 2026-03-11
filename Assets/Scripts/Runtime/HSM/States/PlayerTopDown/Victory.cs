using UnityEngine;
using HSM;
public class Victory : State
{
    readonly PlayerTopdownContext ctx;
    public Victory(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }
    protected override void OnEnter()
    {
        ctx.CurrentMoveSpeed = 0f;
        ctx.TargetMoveSpeed = 0f;
    }
}
