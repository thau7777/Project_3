using HSM;
using UnityEngine;

public class MinionTopDownHurt : State
{
    readonly MinionTopDownContext ctx;
    float _targetSpeed = 0;
    float _knockBackDuration = 0.2f;
    float _elapsedTime = 0;
    public MinionTopDownHurt(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }

    protected override void OnEnter()
    {
        _elapsedTime = 0f;
        ctx.MoveDir = ctx.KnockbackDirection;
        ctx.CurrentSpeed = ctx.KnockbackForce;
        ctx.Animator.Play(ctx.HurtHash, 0, 0);
    }
    protected override void OnUpdate(float deltaTime)
    {
        if (ctx.IsMoreHurt)
        {
            ctx.IsMoreHurt = false;
            OnEnter();
        }
        if (_elapsedTime >= _knockBackDuration)
        {
            ctx.CurrentSpeed = _targetSpeed;
            return;
        }

        _elapsedTime += deltaTime;

        float t = Mathf.Clamp01(_elapsedTime / _knockBackDuration);
        ctx.CurrentSpeed = Mathf.Lerp(ctx.KnockbackForce, _targetSpeed, t);

    }

    protected override State GetTransition()
    {
        if (ctx.IsDead)
            return ((MinionTopDownRoot)Parent).Dead;
        if(!ctx.IsHurting)
            return ((MinionTopDownRoot)Parent).Idle;
        return null;
    }
}
