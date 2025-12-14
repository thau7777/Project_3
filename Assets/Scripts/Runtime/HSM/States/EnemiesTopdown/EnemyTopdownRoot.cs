using HSM;
using UnityEngine;

public class EnemyTopdownRoot : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownIdle Idle;
    public EnemyTopdownMove Move;
    public EnemyTopdownAttack Attack;
    public EnemyTopdownHurt Hurt;
    public EnemyTopdownDead Dead;
    public EnemyTopdownStunned Stunned;
    public EnemyTopdownSpecialMove SpecialMove;


    private float verticalVelocity; // stored between frames
    public EnemyTopdownRoot(StateMachine machine, EnemyTopdownContext context) : base(machine, null)
    {
        ctx = context;
        Idle = new EnemyTopdownIdle(machine, this, ctx);
        Move = new EnemyTopdownMove(machine, this, ctx);
        Attack = new EnemyTopdownAttack(machine, this, ctx);
        Hurt = new EnemyTopdownHurt(machine, this, ctx);
        Dead = new EnemyTopdownDead(machine, this, ctx);
        Stunned = new EnemyTopdownStunned(machine, this, ctx);
        if(ctx.IsBoss)
        SpecialMove = new EnemyTopdownSpecialMove(machine, this, ctx);
    }
    public void UpdateRotation(float deltaTime, Vector3 targetPosition)
    {
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
        if (ctx.IsBoss)
            UpdateSpecialMoveCoolDown(deltaTime);
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
    private void UpdateSpecialMoveCoolDown(float deltaTime)
    {
        if (ctx.IsBoss && !ctx.IsInSpecialMove)
            ctx.BossAttackCoolDownTimer += deltaTime;
    }
    protected override State GetInitialState() => Idle;
    protected override State GetTransition() => null;
}
