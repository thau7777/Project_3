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
        _targetLastPosition = ctx.CurrentTargetTransform.position;
        ctx.CurrentSpeed = 0;
        //if (GetTransition() != null) return;
        ctx.Animator.CrossFade(ctx.AttackHash, 0,0);
    }protected override void OnUpdate(float deltaTime)
    {
        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime,_targetLastPosition);
    }
    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Dead;
        }
        if (ctx.IsStunned)
        {
            return ((EnemyTopdownRoot)Parent).Stunned;
        }
        if (ctx.IsHurting)
        {
            return ((EnemyTopdownRoot)Parent).Hurt;
        }
        if (ctx.IsDoneAttacking)
        {
            ctx.IsDoneAttacking = false;
            return ((EnemyTopdownRoot)Parent).Idle;
        }
        return null;
    }
}
