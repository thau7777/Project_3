using UnityEngine;
using HSM;
public class BossTransitionPhase : State
{
    readonly EnemyTopdownContext ctx;
    public BossTransitionPhase(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        ctx.CurrentSpeed = 0;
        ctx.Animator.CrossFade(ctx.ChangePhaseHash, 0, 0);
    }
    protected override void OnExit()
    {
        ctx.IsChangedPhase = true;
    }
    protected override State GetTransition()
    {
        if (!ctx.IsChangingPhase)
            return ((EnemyTopdownRoot)Parent).Idle;
        return null;
    }
}
