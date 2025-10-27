using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : State
{
    readonly PlayerTopdownContext ctx;
    Action getMoveDirByInput;
    public Attack(StateMachine m, State parent, PlayerTopdownContext ctx, Action getMoveDirInput = null) : base(m, parent)
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
            ctx.TargetMoveSpeed = ctx.StrafeMoveSpeed;
            ctx.Animator.Play(ctx.StrafeStateHash, 0, 0);
        }
        ctx.Animator.Play(ctx.FirstAttackAnimName, ctx.IsRangeClass ? 1 : 0, 0);
    }
    protected override void OnUpdate(float deltaTime)
    {
        if (ctx.IsRangeClass)
        {
            ctx.Animator.SetLayerWeight(ctx.UpperBodyLayerIndex, 1f);
            UpdateMoveDir();
            ctx.MoveDir = ctx.DesiredMoveDir;
        }

    }
    private void UpdateMoveDir()
    {
        getMoveDirByInput?.Invoke();
    }
    protected override void OnExit()
    {
        if (ctx.IsRangeClass)
            ctx.Animator.Play("Empty State", ctx.UpperBodyLayerIndex);
    }
    protected override State GetTransition()
    {
        if (ctx.IsAiming)
        {
            ctx.IsAttacking = false;

            return ((Grounded)Parent).Strafe;
        }
        if (ctx.IsInSpecialMove)
        {
            ctx.IsAttacking = false;

            return ((Grounded)Parent).SpecialMove;
        }
        if (!ctx.IsAttacking)
        {
            if (ctx.MoveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }
            return null;
    }
}
