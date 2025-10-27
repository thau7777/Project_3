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
        ctx.CharacterController.isTrigger = true;
        ctx.Animator.CrossFade(ctx.DeadHash, 0, 0);
    }
    protected override State GetTransition()
    {
        return null;
    }
}