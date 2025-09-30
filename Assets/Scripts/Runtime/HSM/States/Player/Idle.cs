using UnityEngine;
using HSM;
public class Idle : State
{
    readonly PlayerContext ctx;

    public Idle(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.yellow,  // runs while Grounded is activating
        });
    }
    protected override void OnEnter()
    {
        ctx.targetMoveSpeed = 0;
    }
    protected override State GetTransition()
    {
        if (ctx.moveInput != Vector2.zero)
            return ((Grounded)Parent).Move;
        else if (ctx.isAiming)
            return ((Grounded)Parent).Strafe;
        return null;
    }

}