using UnityEngine;
using HSM;
public class Idle : State
{
    readonly MyPlayerContext ctx;

    public Idle(StateMachine m, State parent, MyPlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.yellow,  // runs while Grounded is activating
        });
    }

    protected override State GetTransition()
    {
        return null;
        //return Mathf.Abs(ctx.move.x) > 0.01f ? ((Grounded)Parent).Move : null;
    }

    protected override void OnEnter()
    {
        //ctx.velocity.x = 0f;
    }
}