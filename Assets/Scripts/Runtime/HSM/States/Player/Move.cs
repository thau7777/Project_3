using UnityEngine;
using HSM;
public class Move : State
{
    readonly PlayerContext ctx;

    public Move(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.green,  // runs while Grounded is activating
        });
    }
    protected override void OnEnter()
    {
        ctx.targetMoveSpeed = ctx.baseMoveSpeed;
    }
    protected override void OnUpdate(float deltaTime)
    {
        // rotate toward movement
        if (ctx.moveInput != Vector2.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(ctx.moveDir);
            ctx.rootTransform.rotation = Quaternion.Slerp(ctx.rootTransform.rotation, targetRot, Time.deltaTime * 20f);
        }
    }
    protected override State GetTransition()
    {
        if(ctx.moveInput == Vector2.zero)
            return ((Grounded)Parent).Idle;
        else if(ctx.isAiming)
            return ((Grounded)Parent).Strafe;
        return null;
    }

}