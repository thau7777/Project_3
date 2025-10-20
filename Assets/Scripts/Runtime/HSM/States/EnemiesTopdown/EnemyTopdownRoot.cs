using HSM;
using UnityEngine;

public class EnemyTopdownRoot : State
{
    readonly EnemyTopdownContext ctx;
    public EnemyTopdownIdle Idle;
    public EnemyTopdownMove Move;
    public EnemyTopdownAttack Attack;
    public EnemyTopdownHurt Hurt;
    public EnemyTopdownDead Dead;
    public EnemyTopdownStunned Stunned;
    public EnemyTopdownRoot(StateMachine machine, EnemyTopdownContext context) : base(machine, null)
    {
        ctx = context;
        Idle = new EnemyTopdownIdle(machine, this, ctx);
        Move = new EnemyTopdownMove(machine, this, ctx);
        Attack = new EnemyTopdownAttack(machine, this, ctx);
        Hurt = new EnemyTopdownHurt(machine, this, ctx);
        Dead = new EnemyTopdownDead(machine, this, ctx);
        Stunned = new EnemyTopdownStunned(machine, this, ctx);
    }
    protected override State GetInitialState() => Idle;
    protected override State GetTransition() => null;
}
