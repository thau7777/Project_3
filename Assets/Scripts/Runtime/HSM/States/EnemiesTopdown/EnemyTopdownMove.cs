using UnityEngine;
using HSM;
public class EnemyTopdownMove : State
{
    readonly EnemyTopdownContext ctx;
    Vector3 _targetLastPosition;
    public EnemyTopdownMove(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        _targetLastPosition = ctx.TargetTransform.position;
        if (GetTransition() != null) return;
        ctx.Animator.CrossFade(ctx.MoveHash, 0.1f);
    }
    protected override void OnUpdate(float deltaTime)
    {
        UpdateMovement(deltaTime);
    }
    private void UpdateMovement(float deltaTime)
    {
        Vector3 directionToTarget = (_targetLastPosition - ctx.RootTransform.position).normalized;
        ctx.CharacterController.Move(ctx.MoveSpeed * deltaTime * directionToTarget);
    }
    protected override State GetTransition()
    {
        if (ctx.IsDoneMoving) // for enemy like slime;
        {
            ctx.IsDoneMoving = false;
            if(ctx.IsTargetInAttackRange())
                return ((EnemyTopdownRoot)Parent).Attack;

            return ((EnemyTopdownRoot)Parent).Idle;
        }
        return null;
    }
}
