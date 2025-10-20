using UnityEngine;
using HSM;
public class EnemyTopdownHurt : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownHurt(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }
    protected override State GetTransition()
    {
        return null;
    }
}