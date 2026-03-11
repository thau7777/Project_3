using UnityEngine;
using HSM;
public class Hurt : State
{
    readonly PlayerTopdownContext ctx;
    public Hurt(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }
    protected override void OnEnter()
    {
        ctx.Animator.Play(ctx.HurtStateHash, 0, 0);
        ctx.MoveDir = ctx.KnockBackDirection;
        ctx.CurrentMoveSpeed = ctx.KnockbackForce;
        ctx.TargetMoveSpeed = 0;
    }
    protected override State GetTransition()
    {
        if (ctx.IsVictory)
            return ((Grounded)Parent).Victory;
        if (ctx.IsDead)
        {
            return ((Grounded)Parent).Die;
        }
        if (!ctx.IsHurting)
        {
            if(ctx.MoveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            return ((Grounded)Parent).Idle;
        }
        return null;
    }
}
