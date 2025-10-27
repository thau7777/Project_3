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
        ctx.CurrentSpeed = 0;
        //if (GetTransition() != null) return;
        //ctx.Animator.CrossFade(ctx.IdleHash, 0.1f);
    }
    protected override void OnUpdate(float deltaTime)
    {
        _moveTimer += deltaTime;
        
        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, ctx.TargetTransform.position);
    }
    
    protected override State GetTransition()
    {
        if (ctx.IsHurting)
        {
            ctx.Animator.CrossFade(ctx.HurtHash, 0.1f);
            return ((EnemyTopdownRoot)Parent).Hurt;
        }
        if (ctx.IsTargetInAttackRange())
        {
            ctx.Animator.CrossFade(ctx.AttackHash, 0.1f);
            return ((EnemyTopdownRoot)Parent).Attack;
        }
        else
        {
            if(ctx.EnemyType != EnemyTopdownType.Slime)
            {
                ctx.Animator.CrossFade(ctx.MoveHash, 0.1f);
                return ((EnemyTopdownRoot)Parent).Move;
            }
            else if(_moveTimer >= _moveDelayTime)
            {
                ctx.Animator.CrossFade(ctx.MoveHash, 0.1f);
                return ((EnemyTopdownRoot)Parent).Move;
            }
                
        }

        return null;
    }

}
