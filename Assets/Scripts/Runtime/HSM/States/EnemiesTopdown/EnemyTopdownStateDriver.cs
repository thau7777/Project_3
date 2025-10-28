using UnityEngine;
using HSM;
public class EnemyTopdownStateDriver : Flyweight
{
    #region References
    Animator _animator;
    CharacterController _characterController;
    #endregion

    #region Variables
    [SerializeField]
    EnemyTopdownType _enemyType = EnemyTopdownType.Normal;

    [SerializeField, FoldoutGroup("Movements")]
    float _moveSpeed = 2f;

    [SerializeField, FoldoutGroup("Movements")]
    float _rotateSpeed = 10f;

    [SerializeField]
    float _attackRange = 1f;

    [SerializeField]
    LayerMask _layerIgnoreOnDeath;

    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    #endregion

    [SerializeField]
    EnemyTopdownContext _context;
    StateMachine _machine;
    State _root;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();

        _context = new EnemyTopdownContext.Builder()
            .SetAnimator(_animator)
            .SetCharacterController(_characterController)
            .SetTargetTransform(GameObject.FindWithTag("Player").transform)
            .SetRootTransform(transform)
            .SetMoveSpeed(_moveSpeed)
            .SetRotateSpeed(_rotateSpeed)
            .SetAttackRange(_attackRange)
            .SetEnemyType(_enemyType)
            .Build();
        _root = new EnemyTopdownRoot(null, _context);
        _machine = new StateMachineBuilder(_root).Build();
        GetComponent<Damageable>().Initialize(100);
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
        else if (stateInfo.IsTag("Movement"))
        {
            _context.IsDoneMoving = true;
        }
        else if (stateInfo.IsTag("Hurt"))
        {
            _context.IsHurting = false;
        }
        else if (stateInfo.IsTag("Dead"))
        {
            FlyweightFactory.ReturnToPool(this);
        }
    }

    public void OnTakeDamage(int currentHealth, Vector3 knockBackDirection, float knockBackForce)
    {
        if(_context.IsDead) return;
        if (currentHealth <= 0)
        {
            ApplyIgnoreCollision(true);
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
    public void ApplyIgnoreCollision(bool ignore)
    {
        // Find all active colliders in the scene
        Collider[] allColliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (var col in allColliders)
        {
            // Check if the collider's layer is one of the layers in the mask
            if (((1 << col.gameObject.layer) & _layerIgnoreOnDeath) != 0)
            {
                Physics.IgnoreCollision(_characterController, col, ignore);
            }
        }
    }
    public void ResetStateContext()
    {
        _context.IsDead = false;
        _context.IsHurting = false;
        _context.IsMoreHurt = false;
        _context.IsStunned = false;
        _context.IsDoneAttacking = false;
        _context.IsDoneMoving = false;

        ApplyIgnoreCollision(false);
    }
}
