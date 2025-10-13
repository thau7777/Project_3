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
        Add(new ColorPhaseActivity(ctx.Renderer)
        {
            enterColor = Color.green,  // runs while Grounded is activating
        });
    }
    protected override void OnEnter()
    {
        ctx.TargetMoveSpeed = ctx.BaseMoveSpeed;
        var currentAnim = ctx.Animator.GetCurrentAnimatorStateInfo(0);
        if (!currentAnim.IsName("Movement") && !ctx.IsInSpecialMove && !ctx.IsStrafing && !ctx.IsAttacking)
            ctx.Animator.CrossFade(ctx.MovementStateHash, ctx.NextAnimCrossFadeTime);
    }
    protected override void OnUpdate(float deltaTime)
    {
        UpdateMoveDir();
        // rotate toward movement
        ctx.RotateDir = ctx.MoveDir;
    }
    private void UpdateMoveDir()
    {
        getMoveDirByInput?.Invoke();
    }
    protected override State GetTransition()
    {
        if(ctx.MoveInput == Vector2.zero)
            return ((Grounded)Parent).Idle;
        else if (ctx.IsStrafing)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Strafe;
        }
        else if (ctx.IsAttacking)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Attack;
        }else if (ctx.IsInSpecialMove)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).SpecialMove;
        }
            return null;
    }

}