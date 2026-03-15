using UnityEngine;
using HSM;
public class Idle : State
{
    readonly PlayerTopdownContext ctx;
    public Idle(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.Renderer)
        {
            enterColor = Color.yellow,  // runs while Grounded is activating
        });
    }
    protected override void OnEnter()
    {
        ctx.TargetMoveSpeed = 0;
        bool isInMovementAnim = ctx.Animator.GetCurrentAnimatorStateInfo(0).IsName("Movement");
        if (!isInMovementAnim)
        {
            ctx.Animator.CrossFade(ctx.MovementStateHash, 0.1f); // main layer / lower body
        }
    }
    protected override State GetTransition()
    {
        if (ctx.IsDespawning)
            return ((Grounded)Parent).Despawn;
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
        if (ctx.MoveInput != Vector2.zero)
            return ((Grounded)Parent).Move;
        return null;
    }

}