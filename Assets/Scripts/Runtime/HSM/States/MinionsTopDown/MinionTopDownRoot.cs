using HSM;
using UnityEngine;

public class MinionTopDownRoot : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownIdle Idle;
    public MinionTopDownMove Move;
    public MinionTopDownAttack Attack;
    public MinionTopDownHurt Hurt;
    public MinionTopDownDead Dead;
    public MinionTopDownStunned Stunned; 
    
    private float verticalVelocity; // stored between frames
    public MinionTopDownRoot(StateMachine machine, MinionTopDownContext context) : base(machine, null)
    {
        ctx = context;
        Idle = new MinionTopDownIdle(machine, this, ctx);
        Move = new MinionTopDownMove(machine, this, ctx);
        Attack = new MinionTopDownAttack(machine, this, ctx);
        Hurt = new MinionTopDownHurt(machine, this, ctx);
        Dead = new MinionTopDownDead(machine, this, ctx);
        Stunned = new MinionTopDownStunned(machine, this, ctx);
    }

    public void UpdateRotation(float deltaTime, Vector3 targetPostion)
    {
        var toPlayer = (targetPostion - ctx.RootTransform.position).normalized;
        if (toPlayer == Vector3.zero)
            return;
        Quaternion targetRot = Quaternion.LookRotation(toPlayer);
        ctx.RootTransform.rotation = Quaternion.Slerp(ctx.RootTransform.rotation, targetRot, deltaTime * ctx.RotateSpeed);
    }
    protected override void OnUpdate(float deltaTime)
    {
        UpdateMovement(deltaTime);
    }
    private void UpdateMovement(float deltaTime)
    {
        if (ctx.CurrentSpeed <= 0 && ctx.CharacterController.isGrounded)
            verticalVelocity = -1f; // keep grounded so controller detects ground
        else
            verticalVelocity += Physics.gravity.y * deltaTime; // apply gravity over time

        // combine horizontal + vertical movement
        Vector3 move = ctx.MoveDir * ctx.CurrentSpeed;
        move.y = verticalVelocity;

        // final move call
        ctx.CharacterController.Move(move * deltaTime);

        // reset vertical velocity if grounded
        if (ctx.CharacterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -1f; // small negative to keep grounded
    }
    protected override State GetInitialState() => Idle;
    protected override State GetTransition() => null;
}
