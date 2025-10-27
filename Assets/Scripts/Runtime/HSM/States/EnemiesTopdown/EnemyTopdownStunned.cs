using UnityEngine;
using HSM;
public class EnemyTopdownStunned : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownStunned(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Dead;
        }
        return null;
    }
}