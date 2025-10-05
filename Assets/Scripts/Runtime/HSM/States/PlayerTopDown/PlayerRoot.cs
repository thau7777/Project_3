using HSM;
using UnityEngine;
public class PlayerRoot : State
{
    public readonly Grounded Grounded;
    readonly PlayerContext ctx;

    float _speedVelocity;
    public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        Grounded = new Grounded(m, this, ctx);
    }

    protected override void OnUpdate(float deltaTime)
    {
        ModifyCurrentSpeed();
        HandleMovement(deltaTime);
        HandleRotation(deltaTime);
        ctx.anim.SetFloat(ctx.moveSpeedHash, ctx.currentMoveSpeed);
    }
    private void ModifyCurrentSpeed()
    {
        ctx.currentMoveSpeed = Mathf.SmoothDamp(
            ctx.currentMoveSpeed,
            ctx.targetMoveSpeed,
            ref _speedVelocity,
            ctx.moveSpeedSmoothTime
        );
    }
    private void HandleMovement(float deltaTime) 
        => ctx.characterController.Move(ctx.moveDir * ctx.currentMoveSpeed * deltaTime);
    private void HandleRotation(float deltaTime)
    {
        if (ctx.rotateDir == Vector3.zero)
            return; // skip rotation if no direction
        Quaternion targetRot = Quaternion.LookRotation(ctx.rotateDir);
        ctx.rootTransform.rotation = Quaternion.Slerp(ctx.rootTransform.rotation, targetRot, deltaTime * ctx.rotateSpeed);
    }

    protected override State GetInitialState() => Grounded;
    protected override State GetTransition() => null;
}