using UnityEngine;
using HSM;
using System;
public class SpecialMove : State
{
    readonly PlayerTopdownContext ctx;
    public SpecialMove(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.Renderer)
        {
            enterColor = Color.black,
        });
    }
    protected override State GetTransition()
    {
        if (!ctx.IsInSpecialMove)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            if (ctx.MoveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }
            return null;
    }
}
