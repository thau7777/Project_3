using UnityEngine;
using HSM;
public class Move : State
{
    readonly MyPlayerContext ctx;

    public Move(StateMachine m, State parent, MyPlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.green,  // runs while Grounded is activating
        });
    }

    protected override State GetTransition()
    {
        return null;
        //if (!ctx.grounded) return ((PlayerRoot)Parent).Strafe;

        //return Mathf.Abs(ctx.move.x) <= 0.01f ? ((Grounded)Parent).Idle : null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        //var target = ctx.move.x * ctx.moveSpeed;
        //ctx.velocity.x = Mathf.MoveTowards(ctx.velocity.x, target, ctx.accel * deltaTime);
    }
}