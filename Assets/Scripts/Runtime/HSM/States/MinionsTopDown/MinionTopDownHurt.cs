using HSM;
using UnityEngine;

public class MinionTopDownHurt : State
{
    readonly MinionTopDownContext ctx;
    public MinionTopDownHurt(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }
}
