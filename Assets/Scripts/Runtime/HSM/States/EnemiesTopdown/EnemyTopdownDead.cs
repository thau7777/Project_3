using UnityEngine;
using HSM;
public class EnemyTopdownDead : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownDead(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        ctx.CurrentSpeed = 0; 
        ctx.Animator.CrossFade(ctx.DeadHash, 0, 0);
        MinionsManager.Instance?.RemoveTargetedEnemy(ctx.RootTransform.gameObject);
    }
    protected override State GetTransition()
    {
        if(!ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Idle;
        }
        return null;
    }
}