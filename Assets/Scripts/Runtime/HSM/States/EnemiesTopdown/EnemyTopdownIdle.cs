using UnityEngine;
using HSM;
public class EnemyTopdownIdle : State
{
    readonly EnemyTopdownContext ctx;
    float _moveTimer = 0;
    public EnemyTopdownIdle(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        _moveTimer = 0;
        ctx.CurrentSpeed = 0;
        //if (GetTransition() != null) return;
        ctx.Animator.CrossFade(ctx.IdleHash, 0.1f);
    }
    protected override void OnUpdate(float deltaTime)
    {
        _moveTimer += deltaTime;

        Vector3 direction = ctx.CurrentTargetTransform.position - ctx.RootTransform.position;
        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, direction);
    }
    
    protected override State GetTransition()
    {
        if(ctx.IsDead)
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
        if (ctx.EnemyType == EnemyTopdownMovementType.Range)
        {

            // In optimal range - can attack
            if (ctx.DistanceToTarget >= ctx.MinRangeDistance &&
                ctx.DistanceToTarget <= ctx.MaxRangeDistance)
            {

                if (ctx.CheckAndPickRandomAttack())
                {

                    if (ctx.CurrentEnemyAttackData.needCharge)
                        return ((EnemyTopdownRoot)Parent).Charge;
                    return ((EnemyTopdownRoot)Parent).Attack;

                }
                else
                    return null;
            }
            else if(ctx.DistanceToTarget < ctx.MinRangeDistance)
                return ((EnemyTopdownRoot)Parent).Move;
            else if(ctx.DistanceToTarget > ctx.MaxRangeDistance)
                return ((EnemyTopdownRoot)Parent).Move;
            // Continue moving to get into optimal range
        }
        else if (ctx.IsTargetInMaxAttackRange()) // Melee or Slime and target is in attack range
        {
            if (ctx.CheckAndPickRandomAttack())
            {
                if (ctx.CurrentEnemyAttackData.needCharge)
                    return ((EnemyTopdownRoot)Parent).Charge;
                return ((EnemyTopdownRoot)Parent).Attack;
            }
            if (ctx.DistanceToTarget < 1.5f)
                return null;
            return ((EnemyTopdownRoot)Parent).Move;
        }
        else
        {
            if (ctx.EnemyType == EnemyTopdownMovementType.Slime)
            {
                if (_moveTimer >= ctx.MovePauseDuration)
                    return ((EnemyTopdownRoot)Parent).Move;
            }
            else
                return ((EnemyTopdownRoot)Parent).Move;


        }

        return null;
    }

}
