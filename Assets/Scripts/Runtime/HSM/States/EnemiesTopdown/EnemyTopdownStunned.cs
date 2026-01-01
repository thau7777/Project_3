using UnityEngine;
using HSM;
public class EnemyTopdownStunned : State
{
    readonly EnemyTopdownContext ctx;
    private float _elapsedTime;
    public EnemyTopdownStunned(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        _elapsedTime = 0f;
        ctx.CurrentSpeed = 0;
        ctx.Animator.Play(ctx.StunnedHash);
    }

    protected override void OnUpdate(float deltaTime)
    {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= ctx.StunDuration)
        {
            ctx.IsStunned = false;
        }

    }
    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Dead;
        }
        if(!ctx.IsStunned)
        {
            return ((EnemyTopdownRoot)Parent).Idle;
        }
        return null;
    }
}