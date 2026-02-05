using UnityEngine;
using HSM;
public class EnemyTopdownSpawn : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownSpawn(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        ctx.IsSpawning = true;

        ctx.CurrentSpeed = 0; 
        ctx.Animator.CrossFade(ctx.IdleHash, 0, 0);
    }
    protected override State GetTransition()
    {
        if(!ctx.IsSpawning)
            return ((EnemyTopdownRoot)Parent).Idle;
        return null;
    }
}
