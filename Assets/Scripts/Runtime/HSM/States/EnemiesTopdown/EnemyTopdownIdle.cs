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
        
        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, ctx.CurrentTargetTransform.position);
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
        if (ctx.IsTargetInAttackRange())
        {
            if(!ctx.IsBoss)
                return ((EnemyTopdownRoot)Parent).Attack;

            if(ctx.CheckAndPickRandomAttack())
                return ((EnemyTopdownRoot)Parent).SpecialMove;

            return null;
        }
        else
        {
            if(ctx.EnemyType != EnemyTopdownKind.Slime)
            {
                return ((EnemyTopdownRoot)Parent).Move;
            }
            else if(_moveTimer >= ctx.MovePauseDuration)
            {
                return ((EnemyTopdownRoot)Parent).Move;
            }
                
        }

        return null;
    }

}
