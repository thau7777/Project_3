using UnityEngine;
using HSM;
public class Spawn : State
{
    readonly PlayerTopdownContext ctx;
    private float _spawnDuration = 3f;
    private float _elapsedTime;
    public Spawn(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }
    protected override void OnEnter()
    {
        ctx.Animator.CrossFade(ctx.SpawnStateHash, 0.1f);

        ctx.CurrentMoveSpeed = 0f;
        ctx.TargetMoveSpeed = 0f;
        _elapsedTime = 0f;
    }
    protected override void OnUpdate(float deltaTime)
    {
        _elapsedTime += deltaTime;
    }
    protected override void OnExit()
    {
        ctx.IsSpawning = false;
    }
    protected override State GetTransition()
    {
        if(_elapsedTime >= 3)
            return ((Grounded)Parent).Idle;
        return null;
    }
}
