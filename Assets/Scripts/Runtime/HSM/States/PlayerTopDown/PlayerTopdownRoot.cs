using HSM;
using UnityEngine;
public class PlayerTopdownRoot : State
{
    public readonly Grounded Grounded;
    readonly PlayerTopdownContext ctx;
    
    float _speedVelocity;
    public PlayerTopdownRoot(StateMachine m, PlayerTopdownContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        Grounded = new Grounded(m, this, ctx);
    }

    protected override void OnUpdate(float deltaTime)
    {
        ModifyCurrentSpeed();
        HandleMovement(deltaTime);
        HandleRotation(deltaTime);
        SetInputValueSmoothly();
        ctx.Animator.SetFloat(ctx.MoveSpeedHash, ctx.CurrentMoveSpeed);
    }
    private void ModifyCurrentSpeed()
    {
        ctx.CurrentMoveSpeed = Mathf.SmoothDamp(
            ctx.CurrentMoveSpeed,
            ctx.TargetMoveSpeed,
            ref _speedVelocity,
            ctx.MoveSpeedSmoothTime
        );
    }
    private void SetInputValueSmoothly()
    {
        // lerp animator input params
        float currentX = ctx.Animator.GetFloat(ctx.InputXHash);
        float currentY = ctx.Animator.GetFloat(ctx.InputYHash);

        float smoothedX = Mathf.Lerp(currentX, ctx.MoveDir.x, Time.deltaTime * 10f);
        float smoothedY = Mathf.Lerp(currentY, ctx.MoveDir.z, Time.deltaTime * 10f);

        ctx.Animator.SetFloat(ctx.InputXHash, smoothedX);
        ctx.Animator.SetFloat(ctx.InputYHash, smoothedY);
    }
    private void HandleMovement(float deltaTime) 
        => ctx.CharacterController.Move(ctx.MoveDir * ctx.CurrentMoveSpeed * deltaTime);
    private void HandleRotation(float deltaTime)
    {
        if (ctx.RotateDir == Vector3.zero)
            return; // skip rotation if no direction
        Quaternion targetRot = Quaternion.LookRotation(ctx.RotateDir);
        ctx.RootTransform.rotation = Quaternion.Slerp(ctx.RootTransform.rotation, targetRot, deltaTime * ctx.RotateSpeed);
    }

    protected override State GetInitialState() => Grounded;
    protected override State GetTransition() => null;
}