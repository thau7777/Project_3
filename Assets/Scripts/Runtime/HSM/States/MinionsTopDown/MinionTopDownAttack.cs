using HSM;
using UnityEngine;

public class MinionTopDownAttack : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownAttack(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override void OnEnter()
    {
        ctx.CurrentSpeed = 0;
        ctx.Animator.Play(ctx.AttackHash1);
    }

    protected override void OnUpdate(float deltaTime)
    {
        ((MinionTopDownRoot)Parent).UpdateRotation(deltaTime, ctx.MoveTargetPosition);
    }
    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((MinionTopDownRoot)Parent).Dead;
        }
        if (ctx.IsHurting)
        {
            return ((MinionTopDownRoot)Parent).Hurt;
        }
        if (!ctx.IsEnemyInAttackRange())
        {
            return ((MinionTopDownRoot)Parent).Move;
        }
        

        return null;
    }
}
