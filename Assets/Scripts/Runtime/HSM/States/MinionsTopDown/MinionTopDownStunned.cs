using HSM;

public class MinionTopDownStunned : State
{
    readonly MinionTopDownContext ctx;
    private float _stunDuration;
    private float _elapsedTime;
    public MinionTopDownStunned(StateMachine machine, State parent, MinionTopDownContext context) : base(machine, parent)
    {
        ctx = context;
    }

    protected override void OnEnter()
    {
        ctx.CurrentSpeed = 0;
        _elapsedTime = 0;
        ctx.Animator.CrossFade(ctx.StunnedHash, 0.1f);
    }

    protected override void OnUpdate(float deltaTime)
    {
        if(_elapsedTime < _stunDuration)
            _elapsedTime += deltaTime;
    }

    protected override State GetTransition()
    {
        if (_elapsedTime >= _stunDuration)
        {
            return ((MinionTopDownRoot)Parent).Idle;
        }
        return null;
    }
}
