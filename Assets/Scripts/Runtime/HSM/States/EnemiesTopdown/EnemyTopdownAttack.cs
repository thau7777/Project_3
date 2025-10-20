using UnityEngine;
using HSM;
public class EnemyTopdownAttack : State
{
    readonly EnemyTopdownContext ctx;
    Vector3 _targetLastPosition;
    public EnemyTopdownAttack(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
 
    protected override void OnEnter()
    {
        _targetLastPosition = ctx.TargetTransform.position;
        if (GetTransition() != null) return;
        ctx.Animator.CrossFade(ctx.AttackHash, 0.1f);
    }protected override void OnUpdate(float deltaTime)
    {
        UpdateRotation(deltaTime);
    }
    private void UpdateRotation(float deltaTime)
    {
        var toPlayer = (_targetLastPosition - ctx.RootTransform.position).normalized;
        if (toPlayer == Vector3.zero)
            return;
        Quaternion targetRot = Quaternion.LookRotation(toPlayer);
        ctx.RootTransform.rotation = Quaternion.Slerp(ctx.RootTransform.rotation, targetRot, deltaTime * ctx.RotateSpeed);
    }
    protected override State GetTransition()
    {
        if (ctx.IsDoneAttacking)
        {
            ctx.IsDoneAttacking = false;    
            return ((EnemyTopdownRoot)Parent).Idle;
        }
        return null;
    }
}
