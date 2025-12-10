using HSM;
using UnityEngine;

public class MinionTopDownStunned : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownStunned(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }
}
