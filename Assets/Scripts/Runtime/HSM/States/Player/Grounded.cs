using UnityEngine;
using HSM;
public class Grounded : State
{
    readonly PlayerContext ctx;
    public readonly Idle Idle;
    public readonly Move Move;
    public readonly Strafe Strafe;
    public Grounded(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Idle = new Idle(m, this, ctx);
        Move = new Move(m, this, ctx);
        Strafe = new Strafe(m, this, ctx);
        
    }

    protected override void OnUpdate(float deltaTime)
    {
        HandleMovement(deltaTime);
        ctx.anim.SetFloat("MoveSpeed", ctx.currentMoveSpeed);
    }

    private void HandleMovement(float deltaTime)
    {
        //camera reference
        Transform cam = ctx.mainCameraTransform;

        // flatten forward/right onto XZ
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        // input relative to camera
        Vector3 moveDir = camForward * ctx.moveInput.y + camRight * ctx.moveInput.x;
        ctx.moveDir = moveDir;
        // move
        ctx.characterController.Move(moveDir * ctx.currentMoveSpeed * Time.deltaTime);

    }
    protected override State GetInitialState() => Idle;

    protected override State GetTransition()
    {
        return null;
    }
}