using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : State
{
    readonly PlayerContext ctx;
    float ogMoveSpeedSmoothTime;
    float ogMoveSpeed;
    Action getMoveDirByInput;
    public Attack(StateMachine m, State parent, PlayerContext ctx, Action getMoveDirInput = null) : base(m, parent)
    {
        this.ctx = ctx;
        this.getMoveDirByInput = getMoveDirInput;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.cyan, 
        }); 
        if (ctx.IsRangeClass) // attack by upper body
            Add(new LayerWeightActivity(ctx.anim, ctx.upperBodyLayerIndex, 1f, 0f, 0.1f));
    }
    protected override void OnEnter()
    {
        ogMoveSpeedSmoothTime = ctx.moveSpeedSmoothTime;
        ctx.moveSpeedSmoothTime = 0.05f; // quick to 0
        if (ctx.IsRangeClass)
        {
            ctx.anim.CrossFade(ctx.nextAttackAnim, ctx.nextAnimCrossFadeTime, ctx.upperBodyLayerIndex, 0);
            ctx.anim.CrossFade(ctx.strafeStateHash, ctx.nextAnimCrossFadeTime);
            ctx.targetMoveSpeed = ctx.strafeMoveSpeed;
            return;
        }
        ctx.anim.CrossFade(ctx.nextAttackAnim, ctx.nextAnimCrossFadeTime, 0, 0);
    }
    protected override void OnUpdate(float deltaTime)
    {
        if (ctx.IsRangeClass)
        {
            ctx.anim.SetLayerWeight(ctx.upperBodyLayerIndex, 1f);
            UpdateMoveDir();
        }

    }
    private void UpdateMoveDir()
    {
        getMoveDirByInput?.Invoke();
    }

    protected override void OnExit()
    {
        ctx.moveSpeedSmoothTime = ogMoveSpeedSmoothTime;
        if (ctx.IsRangeClass)
        {
            ctx.anim.CrossFade("Empty State", ctx.nextAnimCrossFadeTime, ctx.upperBodyLayerIndex);
            return;
        }
    }
    protected override State GetTransition()
    {
        if(ctx.isAttacking == false)
        {
            ctx.nextAnimCrossFadeTime = 0.1f;
            if (ctx.moveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }
        return null;
    }
}
