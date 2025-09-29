using UnityEngine;
using HSM;
public class Grounded : State
{
    readonly PlayerContext ctx;
    public readonly Idle Idle;
    public readonly Move Move;
    public readonly Strafe Strafe;
    public Grounded(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Idle = new Idle(m, this, ctx);
        Move = new Move(m, this, ctx);
        Strafe = new Strafe(m, this, ctx);
        
    }

    protected override State GetInitialState() => Idle;

    protected override State GetTransition()
    {
        return null;
        //if (ctx.jumpPressed)
        //{
        //    ctx.jumpPressed = false;
        //    var rb = ctx.rb;

        //    if (rb != null)
        //    {
        //        var v = rb.linearVelocity;
        //        v.y = ctx.jumpSpeed;
        //        rb.linearVelocity = v;
        //    }
        //    return ((PlayerRoot)Parent).Strafe;
        //}
        //return ctx.grounded ? null : ((PlayerRoot)Parent).Strafe;
    }
}