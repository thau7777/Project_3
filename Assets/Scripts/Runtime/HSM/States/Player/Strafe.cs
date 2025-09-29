using HSM;
using UnityEngine;

public class Strafe : State
{
    readonly MyPlayerContext ctx;

    public Strafe(StateMachine m, State parent, MyPlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.red, // runs while Airborne is activating
        });
    }

    protected override State GetTransition() => null;

    protected override void OnEnter()
    {
        // TODO: Update Animator through ctx.anim
    }
}