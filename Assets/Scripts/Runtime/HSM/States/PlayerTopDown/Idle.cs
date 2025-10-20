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
        if(!ctx.IsDashing)
            ctx.TargetMoveSpeed = 0; 
        var currentAnim = ctx.Animator.GetCurrentAnimatorStateInfo(0);
        if (!currentAnim.IsName("Movement") && !ctx.IsInSpecialMove && !ctx.IsStrafing && !ctx.IsAttacking)
            ctx.Animator.CrossFade(ctx.MovementStateHash, ctx.NextAnimCrossFadeTime);
    }
    protected override State GetTransition()
    {
        if (ctx.MoveInput != Vector2.zero)
            return ((Grounded)Parent).Move;
        else if (ctx.IsStrafing)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Strafe;
        }
        else if (ctx.IsAttacking)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Attack;
        }
        else if (ctx.IsInSpecialMove)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).SpecialMove;
        }
        return null;
    }

}