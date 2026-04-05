using UnityEngine;
using HSM;
public class EnemyTopdownHurt : State
{
    readonly EnemyTopdownContext ctx;
    float _targetSpeed = 0;
    float _knockBackDuration = 0.2f;
    float _elapsedTime = 0;
    public EnemyTopdownHurt(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
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
    protected override void OnExit()
    {
        ctx.IsHurting = false;
        ctx.IsMoreHurt = false;
    }
    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Dead;
        }
        if (ctx.IsChangingPhase && !ctx.IsChangedPhase)
        {
            return ((EnemyTopdownRoot)Parent).BossTransition;
        }
        if (ctx.IsStunned)
        {
            ctx.IsHurting = false;
            return ((EnemyTopdownRoot)Parent).Stunned;
        }
        if (!ctx.IsHurting)
        {
            return ((EnemyTopdownRoot)Parent).Idle;
        }
        return null;
    }
}