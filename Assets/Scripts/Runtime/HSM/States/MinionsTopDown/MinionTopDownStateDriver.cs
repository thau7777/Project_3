using HSM;
using UnityEngine;

public class MinionTopDownStateDriver : MonoBehaviour
{
    #region References
    private Animator _animator;
    private CharacterController _characterController;

    private MinionData _minionData;
    #endregion

    #region Variables

    [SerializeField]
    private MinionTopDownContext _context;
    private MinionTopDownRoot _root;
    private StateMachine _machine;

    private EventBinding<SummonerTargetEvent> _summonerTargetEventBinding;
    #endregion
    
    public void InitializeMinion(MinionData minionData)
    {
        _minionData = minionData;

        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        GetComponent<Damageable>().Initialize(_minionData.MaxHealth);

        _context = new MinionTopDownContext.Builder()
        .SetKind(minionData.Kind)
        .SetAnimator(_animator)
        .SetCharacterController(_characterController)
        .SetOwner(GameObject.FindWithTag("Player").transform)
        .SetRootTransform(transform)
        .SetMoveSpeed(1)
        .SetRotateSpeed(20f)
        .SetAttackRange(1.2f)
        .Build();

        _root = new MinionTopDownRoot(null, _context);
        _machine = new StateMachineBuilder(_root).Build();


        _summonerTargetEventBinding = new EventBinding<SummonerTargetEvent>(_context.SetEnemyTransform);
        EventBus<SummonerTargetEvent>.Register(_summonerTargetEventBinding);
    }
    private void OnDisable()
    {
        EventBus<SummonerTargetEvent>.Deregister(_summonerTargetEventBinding);
    }
   
    private void Update()
    {
        _machine.Tick(Time.deltaTime);
    }

    public void OnEndOfAnimation()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Attack"))
        {
            _context.IsDoneAttacking = true;
        }
        else if (stateInfo.IsTag("Hurt"))
        {
            _context.IsHurting = false;
        }
        else if (stateInfo.IsTag("Die"))
        {

        }
    }
    public void OnAttackTrigger(FlyweightSettings vfxSettings)
    {
        var vfx = FlyweightFactory.Spawn(vfxSettings);
        vfx.FlyweightInitialize(transform.position, transform.rotation);
        //vfx.GetComponent<HitBoxHandler>().Sender = transform.gameObject;
        //vfx.GetComponent<DamageDealer>().Damage = _minionData.BaseAttackDamage;
    } 
    public void OnTakeDamage(GameObject sender, int currentHealth, Vector3 knockBackDirection, float knockBackForce)
    {
        if (_context.IsDead) return;
        if (currentHealth <= 0)
        {
            _context.IsDead = true;
            return;
        }
        if (!_context.IsHurting)
            _context.IsHurting = true;
        else _context.IsMoreHurt = true;

        _context.KnockbackDirection = knockBackDirection;
        _context.KnockbackForce = knockBackForce;
    }
    public void OnHealed()
    {

    }
    public void ResetStateContext()
    {
        _context.IsDead = false;
        _context.IsHurting = false;
        _context.IsMoreHurt = false;
        _context.IsStunned = false;
        _context.IsDoneAttacking = false;
    }
}
