using UnityEngine;
using HSM;
public class Grounded : State
{
    readonly MyPlayerContext ctx;
    public readonly Idle Idle;
    public readonly Move Move;
    public readonly Strafe Strafe;
    public Grounded(StateMachine m, State parent, MyPlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Idle = new Idle(m, this, ctx);
        Move = new Move(m, this, ctx);
        Strafe = new Strafe(m, this, ctx);
        
    }
    protected override void OnEnter()
    {

    }

    protected override void OnUpdate(float deltaTime)
    {
        
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

        // move
        ctx.characterController.Move(moveDir * ctx.currentMoveSpeed * Time.deltaTime);

        // rotate toward movement
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            ctx.rootTransform.rotation = Quaternion.Slerp(ctx.rootTransform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }
    protected override State GetInitialState() => Idle;

    protected override State GetTransition()
    {
        return null;
        //if (ctx.jumpPressed)
        //{
        //    ctx.jumpPressed = false;
        //    var rb = ctx.rb;

        //    if (rb != null)
        //    {
        //        var v = rb.linearVelocity;
        //        v.y = ctx.jumpSpeed;
        //        rb.linearVelocity = v;
        //    }
        //    return ((PlayerRoot)Parent).Strafe;
        //}
        //return ctx.grounded ? null : ((PlayerRoot)Parent).Strafe;
    }
}