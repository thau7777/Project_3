using UnityEngine;
using HSM;
public class EnemyTopdownDead : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownDead(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override State GetTransition()
    {
        return null;
    }
}