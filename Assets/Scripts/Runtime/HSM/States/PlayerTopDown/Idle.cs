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
        var currentAnim = ctx.anim.GetCurrentAnimatorStateInfo(0);
        if (!currentAnim.IsName("Movement"))
            ctx.anim.CrossFade(ctx.movementStateHash, ctx.nextAnimCrossFadeTime);
    }
    protected override State GetTransition()
    {
        if (ctx.moveInput != Vector2.zero)
            return ((Grounded)Parent).Move;
        else if (ctx.isStrafing)
        {
            ctx.nextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Strafe;
        }
        else if (ctx.isAttacking)
        {
            ctx.nextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Attack;
        }
        return null;
    }

}