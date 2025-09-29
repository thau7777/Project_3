using HSM;
public class PlayerRoot : State
{
    public readonly Grounded Grounded;
    readonly MyPlayerContext ctx;

    public PlayerRoot(StateMachine m, MyPlayerContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        Grounded = new Grounded(m, this, ctx);
    }

    protected override State GetInitialState() => Grounded;
    protected override State GetTransition() => null;
}