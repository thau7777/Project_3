using UnityEngine;
using HSM;
public class Grounded : State
{
    readonly PlayerTopdownContext ctx;
    public readonly Spawn Spawn;
    public readonly Idle Idle;
    public readonly Move Move;
    public readonly Strafe Strafe;
    public readonly Attack Attack;
    public readonly SpecialMove SpecialMove;
    public readonly Hurt Hurt;
    public readonly Die Die;
    public readonly Despawn Despawn;

    public Grounded(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Spawn = new Spawn(m, this, ctx);
        Idle = new Idle(m, this, ctx);
        Move = new Move(m, this, ctx);
        Strafe = new Strafe(m, this, ctx);
        Attack = new Attack(m, this, ctx);
        SpecialMove = new SpecialMove(m, this, ctx);
        Hurt = new Hurt(m, this, ctx);
        Die = new Die(m, this, ctx);
        Despawn = new Despawn(m, this, ctx);
    }
    protected override void OnUpdate(float deltaTime)
    {
        GetMoveDirByInput();
    }
    public void GetMoveDirByInput()
    {
        //camera reference
        Transform cam = ctx.MainCameraTransform;

        // flatten forward/right onto XZ
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        // input relative to camera
        Vector3 moveDir = camForward * ctx.MoveInput.y + camRight * ctx.MoveInput.x;
        
        ctx.DesiredMoveDir = moveDir;
    }
    protected override State GetInitialState() => Spawn;

    protected override State GetTransition()
    {
        return null;
    }
}