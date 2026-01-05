using HSM;
using UnityEngine;

public class EnemyTopdownRoot : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownSpawn Spawn;
    public EnemyTopdownIdle Idle;
    public EnemyTopdownMove Move;
    public EnemyTopdownCharge Charge;
    public EnemyTopdownAttack Attack;
    public EnemyTopdownHurt Hurt;
    public EnemyTopdownDead Dead;
    public EnemyTopdownStunned Stunned;


    private float verticalVelocity; // stored between frames
    public EnemyTopdownRoot(StateMachine machine, EnemyTopdownContext context) : base(machine, null)
    {
        ctx = context;
        Spawn = new EnemyTopdownSpawn(machine, this, ctx);
        Idle = new EnemyTopdownIdle(machine, this, ctx);
        Move = new EnemyTopdownMove(machine, this, ctx);
        Charge = new EnemyTopdownCharge(machine, this, ctx);
        Attack = new EnemyTopdownAttack(machine, this, ctx);
        Hurt = new EnemyTopdownHurt(machine, this, ctx);
        Dead = new EnemyTopdownDead(machine, this, ctx);
        Stunned = new EnemyTopdownStunned(machine, this, ctx);
    }
    public void UpdateRotation(float deltaTime, Vector3 targetPosition)
    {
        if(ctx.ForceStopFacingTarget)
            return;
        var toPlayer = (targetPosition - ctx.RootTransform.position);

        // Flatten the direction to only rotate on Y-axis
        toPlayer.y = 0;
        toPlayer = toPlayer.normalized;

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
        if (!ctx.CharacterController.enabled) return;
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
    protected override State GetInitialState() => Spawn;
    protected override State GetTransition() => null;
}
