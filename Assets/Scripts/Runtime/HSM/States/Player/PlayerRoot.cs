using HSM;
using UnityEngine;
public class PlayerRoot : State
{
    public readonly Grounded Grounded;
    readonly PlayerContext ctx;

    public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        Grounded = new Grounded(m, this, ctx);
    }

    protected override void OnUpdate(float deltaTime)
    {
        ModifyCurrentSpeed(deltaTime);
    }
    private void ModifyCurrentSpeed(float deltaTime)
    {
        ctx.currentMoveSpeed = Mathf.SmoothDamp(
            ctx.currentMoveSpeed,
            ctx.targetMoveSpeed,
            ref ctx.speedVelocity,
            ctx.smoothTime
        );
    }
    protected override State GetInitialState() => Grounded;
    protected override State GetTransition() => null;
}