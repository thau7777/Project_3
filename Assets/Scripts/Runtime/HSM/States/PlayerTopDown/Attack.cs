using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : State
{
    readonly PlayerContext ctx;
    Action getMoveDirByInput;
    public Attack(StateMachine m, State parent, PlayerContext ctx, Action getMoveDirInput = null) : base(m, parent)
    {
        this.ctx = ctx;
        this.getMoveDirByInput = getMoveDirInput;
        Add(new ColorPhaseActivity(ctx.Renderer)
        {
            enterColor = Color.cyan, 
        }); 
        if (ctx.IsRangeClass) // attack by upper body
            Add(new LayerWeightActivity(ctx.Animator, ctx.UpperBodyLayerIndex, 1f, 0f, 0.1f));
    }
    protected override void OnEnter()
    {
        if (ctx.IsRangeClass)
        {
            ctx.Animator.CrossFade(ctx.StrafeStateHash, ctx.NextAnimCrossFadeTime);
            ctx.TargetMoveSpeed = ctx.StrafeMoveSpeed;
            return;
        }
    }
    protected override void OnUpdate(float deltaTime)
    {
        if (ctx.IsRangeClass)
        {
            ctx.Animator.SetLayerWeight(ctx.UpperBodyLayerIndex, 1f);
            UpdateMoveDir();
        }

    }
    private void UpdateMoveDir()
    {
        getMoveDirByInput?.Invoke();
    }

    protected override void OnExit()
    {
        if (ctx.IsRangeClass && !ctx.IsStrafing)
        {
            ctx.Animator.CrossFade("Empty State", ctx.NextAnimCrossFadeTime, ctx.UpperBodyLayerIndex);
            return;
        }
    }
    protected override State GetTransition()
    {
        if(!ctx.IsAttacking)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            if (ctx.MoveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }else if (ctx.IsStrafing)
        {
            ctx.IsAttacking = false;
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Strafe;
        }else if (ctx.IsInSpecialMove)
        {
            ctx.IsAttacking = false;
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).SpecialMove;
        }
            return null;
    }
}
