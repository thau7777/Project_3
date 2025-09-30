using HSM;
using UnityEngine;

public class Attack : State
{
    readonly PlayerContext ctx;
    public Attack(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
    }
    protected override void OnEnter()
    {
    }
    protected override void OnUpdate(float deltaTime)
    {
    }
    protected override State GetTransition()
    {

        return null;
    }
}
