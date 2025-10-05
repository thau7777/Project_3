using UnityEngine;
using HSM;
using System;
public class Move : State
{
    readonly PlayerContext ctx;
    Action getMoveDirByInput;
    public Move(StateMachine m, State parent, PlayerContext ctx, Action getMoveDirByInput = null) : base(m, parent)
    {
        this.ctx = ctx;
        this.getMoveDirByInput = getMoveDirByInput;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.green,  // runs while Grounded is activating
        });
    }
    protected override void OnEnter()
    {
        ctx.targetMoveSpeed = ctx.baseMoveSpeed;
        var currentAnim = ctx.anim.GetCurrentAnimatorStateInfo(0);
        if(!currentAnim.IsName("Movement"))
            ctx.anim.CrossFade(ctx.movementStateHash, ctx.nextAnimCrossFadeTime);
    }
    protected override void OnUpdate(float deltaTime)
    {
        UpdateMoveDir();
        // rotate toward movement
        ctx.rotateDir = ctx.moveDir;
    }
    private void UpdateMoveDir()
    {
        getMoveDirByInput?.Invoke();
    }
    protected override State GetTransition()
    {
        if(ctx.moveInput == Vector2.zero)
            return ((Grounded)Parent).Idle;
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