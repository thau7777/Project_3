using HSM;
using UnityEngine;

public class MinionTopDownDead : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownDead(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }
}
