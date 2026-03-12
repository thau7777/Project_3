using UnityEngine;
using HSM;
using System;
public class Move : State
{
    readonly PlayerTopdownContext ctx;
    public Move(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        
        Add(new ColorPhaseActivity(ctx.Renderer)
        {
            enterColor = Color.green,  // runs while Grounded is activating
        });
    }
    protected override void OnEnter()
    {
        ctx.TargetMoveSpeed = ctx.BaseMoveSpeed;
        bool isInMovementAnim = ctx.Animator.GetCurrentAnimatorStateInfo(0).IsName("Movement");
        if (!isInMovementAnim)
        {
            ctx.Animator.CrossFade(ctx.MovementStateHash, 0.1f); // main layer / lower body
        }
    }
    protected override void OnUpdate(float deltaTime)
    {
        ctx.MoveDir = ctx.DesiredMoveDir;
        // rotate toward movement
        ctx.RotateDir = ctx.MoveDir;
    }
    protected override State GetTransition()
    {
        if (ctx.IsVictory)
            return ((Grounded)Parent).Victory;
        if (ctx.IsDead)
        {
            return ((Grounded)Parent).Die;
        }
        if (ctx.IsHurting)
        {
            return ((Grounded)Parent).Hurt;
        }
        if (ctx.IsAiming)
        {
            return ((Grounded)Parent).Strafe;
        }
        if (ctx.IsInSpecialMove)
        {
            return ((Grounded)Parent).SpecialMove;
        }
        if (ctx.IsAttacking)
        {
            return ((Grounded)Parent).Attack;
        }
        if (ctx.MoveInput == Vector2.zero)
            return ((Grounded)Parent).Idle;
        return null;
    }

}