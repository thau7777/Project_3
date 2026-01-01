using UnityEngine;
using HSM;
public class EnemyTopdownMove : State
{
    readonly EnemyTopdownContext ctx;
    Vector3 _targetLastPosition;
    Vector3 _targetPosition => ctx.EnemyType == EnemyTopdownKind.Slime ? _targetLastPosition : ctx.CurrentTargetTransform.position;
    public EnemyTopdownMove(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        if (ctx.EnemyType == EnemyTopdownKind.Slime)
            _targetLastPosition = ctx.CurrentTargetTransform.position;
        ctx.CurrentSpeed = ctx.BaseMoveSpeed;
        ctx.Animator.CrossFade(ctx.MoveHash, 0.1f);
    }
    protected override void OnUpdate(float deltaTime)
    {
        UpdateMoveDir();
        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, _targetPosition);
    }
    private void UpdateMoveDir()
    {
        ctx.MoveDir = (_targetPosition - ctx.RootTransform.position).normalized;
    }
    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Dead;
        }
        if(ctx.IsStunned)
        {
            return ((EnemyTopdownRoot)Parent).Stunned;
        }
        if (ctx.IsHurting)
        {
            return ((EnemyTopdownRoot)Parent).Hurt;
        }
        if (ctx.EnemyType == EnemyTopdownKind.Slime)
        {
            if (ctx.IsDoneMoving) // for enemy like slime;
            {
                ctx.IsDoneMoving = false;
                if (ctx.IsTargetInAttackRange())
                {
                    if (!ctx.IsBoss)
                        return ((EnemyTopdownRoot)Parent).Attack;

                    if (ctx.CheckAndPickRandomAttack())
                        return ((EnemyTopdownRoot)Parent).SpecialMove;

                    return ((EnemyTopdownRoot)Parent).Idle;
                }
                return ((EnemyTopdownRoot)Parent).Idle;
            }
        }
        else
        {
            if (ctx.IsTargetInAttackRange())
            {
                if (!ctx.IsBoss)
                    return ((EnemyTopdownRoot)Parent).Attack;

                if (ctx.CheckAndPickRandomAttack())
                    return ((EnemyTopdownRoot)Parent).SpecialMove;

                return null;
            }
        }
        
        return null;
    }
}
