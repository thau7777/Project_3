using UnityEngine;
using HSM;
public class EnemyTopdownMove : State
{
    readonly EnemyTopdownContext ctx;
    Vector3 _targetLastPosition;
    Vector3 _targetPosition => ctx.EnemyType == EnemyTopdownType.Slime ? _targetLastPosition : ctx.TargetTransform.position;
    public EnemyTopdownMove(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        if (ctx.EnemyType == EnemyTopdownType.Slime)
            _targetLastPosition = ctx.TargetTransform.position;
        ctx.CurrentSpeed = ctx.BaseMoveSpeed;
        //if (GetTransition() != null) return;
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
        if (ctx.IsHurting)
        {
            return ((EnemyTopdownRoot)Parent).Hurt;
        }
        if (ctx.EnemyType == EnemyTopdownType.Slime)
        {
            if (ctx.IsDoneMoving) // for enemy like slime;
            {
                ctx.IsDoneMoving = false;
                if (ctx.IsTargetInAttackRange())
                {
                    return ((EnemyTopdownRoot)Parent).Attack;
                }
                return ((EnemyTopdownRoot)Parent).Idle;
            }
        }
        else
        {
            if (ctx.IsTargetInAttackRange())
            {
                return ((EnemyTopdownRoot)Parent).Attack;
            }
        }
        
        return null;
    }
}
