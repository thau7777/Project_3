using UnityEngine;
using HSM;
public class EnemyTopdownIdle : State
{
    readonly EnemyTopdownContext ctx;
    float _moveDelayTime = 1;
    float _moveTimer = 0;
    public EnemyTopdownIdle(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        _moveTimer = 0;
        if (GetTransition() != null) return;
        ctx.Animator.CrossFade(ctx.IdleHash, 0.1f);
    }
    protected override void OnUpdate(float deltaTime)
    {
        _moveTimer += deltaTime;
        UpdateRotation(deltaTime);
    }
    private void UpdateRotation(float deltaTime)
    {
        var toPlayer = (ctx.TargetTransform.position - ctx.RootTransform.position).normalized;
        if (toPlayer == Vector3.zero)
            return;
        Quaternion targetRot = Quaternion.LookRotation(toPlayer);
        ctx.RootTransform.rotation = Quaternion.Slerp(ctx.RootTransform.rotation, targetRot, deltaTime * ctx.RotateSpeed);
    }
    
    protected override State GetTransition()
    {
        if (ctx.IsTargetInAttackRange())
        {
            return ((EnemyTopdownRoot)Parent).Attack;
        }
        if (_moveTimer >= _moveDelayTime && !ctx.IsTargetInAttackRange())
        {
            return ((EnemyTopdownRoot)Parent).Move;
        }
        return null;
    }

}
