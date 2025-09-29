using HSM;
public class PlayerRoot : State
{
    public readonly Grounded Grounded;
    readonly PlayerContext ctx;

    public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null)
    {
        this.ctx = ctx;
        Grounded = new Grounded(m, this, ctx);
    }

    protected override void OnEnter()
    {

    }

    protected override State GetInitialState() => Grounded;
    protected override State GetTransition() => null;
}