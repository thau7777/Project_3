using UnityEngine;
using HSM;
using System.Collections;
using UnityEngine.VFX;
using System.Collections.Generic;
public class EnemyTopdownStateDriver : Flyweight
{
    #region References
    private Animator _animator;
    private CharacterController _characterController;
    #endregion

    #region Variables
    [SerializeField]
    EnemyTopdownType _enemyType = EnemyTopdownType.Normal;

    [SerializeField, FoldoutGroup("Movements")]
    float _moveSpeed = 2f;
    [SerializeField, FoldoutGroup("Movements")]
    [ShowIfEnumValue("_enemyType", EnemyTopdownType.Slime)]
    float _movePauseDuration = 1f;
    [SerializeField, FoldoutGroup("Movements")]
    float _rotateSpeed = 10f;

    [SerializeField]
    float _attackRange = 1f;

    // Boss stuff
    [SerializeField]
    private bool _isBoss;
    [ShowIf("_isBoss")]
    [SerializeField]
    private List<EnemySpecialMoveData> _bossSkillList = new();


    // Hide in inspector stuffs
    private SkillIndicator _skillIndicator;
    private AdvanceOneShotVFX _chargeEffect;

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
            .SetPlayerTransform(GameObject.FindWithTag("Player").transform)
            .SetRootTransform(transform)
            .SetMoveSpeed(_moveSpeed)
            .SetMovePauseDuration(_movePauseDuration)
            .SetRotateSpeed(_rotateSpeed)
            .SetAttackRange(_attackRange)
            .SetEnemyType(_enemyType)
            .SetIsBoss(_isBoss)
            .SetSpecialMoveList(_bossSkillList)
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
    public void OnTakeDamage(GameObject sender,int currentHealth, Vector3 knockBackDirection, float knockBackForce)
    {
        if(_context.IsDead) return;

        if (!_context.IsHurting)
            _context.IsHurting = true;
        else 
            _context.IsMoreHurt = true;

        _context.KnockbackDirection = knockBackDirection;
        _context.KnockbackForce = knockBackForce;

        if(sender.layer == LayerMask.NameToLayer("MinionTopDown"))
            _context.SetCurrentTarget(sender.transform);
        else 
            _context.SetCurrentTarget(_context.PlayerTransform);
    }
    public void OnDeath()
    {
        _context.IsDead = true;
        GetComponent<EffectsManager>().RemoveAllActiveEffects();
    }
    public void OnHealed()
    {

    }
    public void OnAttackTrigger(FlyweightSettings vfxSettings)
    {
        var vfxFlyweight = FlyweightFactory.Spawn(vfxSettings);
        vfxFlyweight.FlyweightInitialize(transform.position.With(y: transform.position.y + 0.35f, z: transform.position.z + 0.35f), transform.rotation);
    }

    public void SpawnChargeEffect(float duration)
    {
        _chargeEffect = FlyweightFactory.Spawn(_context.EnemySpecialMoveData.chargeEffect) as AdvanceOneShotVFX;
        _chargeEffect.FlyweightInitialize(transform.position);
        _chargeEffect.PlayEffect(duration,_context.EnemySpecialMoveData.chargeEffectSize);
    }
    public void ShowSkillIndicator()
    {
        _skillIndicator = FlyweightFactory.Spawn(_context.EnemySpecialMoveData.indicator) as SkillIndicator;
        if (_skillIndicator is FollowedIndicator)
        {
            _skillIndicator.FlyweightInitialize(_context.RootTransform.position);
            FollowedIndicator indicator = (FollowedIndicator)_skillIndicator;
            indicator.Initialize(_context.RootTransform, _context.EnemySpecialMoveData.indicatorWidth, _context.EnemySpecialMoveData.indicatorLength, _context.CurrentTargetTransform);
        }
        else
        {
            Transform spawnTransform = _context.EnemySpecialMoveData.followSelf ? _context.RootTransform : _context.CurrentTargetTransform;
            
            _skillIndicator.FlyweightInitialize(spawnTransform.position);
            CircleIndicator indicator = (CircleIndicator)_skillIndicator;
            indicator.Initialize(_context.EnemySpecialMoveData.indicatorWidth, spawnTransform);
        }
    }
    public void LockIndicator(float duration)
    {
        _skillIndicator.LockIndicator(duration);
        //after the lock we can use OnAttackTrigger to spawn the vfx hitbox stuff
    }
    public void StartSpawnAnim()
    {
        StartCoroutine(SpawnAnimationCoroutine(((EnemyTopDownSettings)settings).spawnAnimationTime));
    }

    public void StopMoving()
    {
        _context.CurrentSpeed = 0;
    }
    public void StartMoving()
    {
        _context.CurrentSpeed = _context.IsInSpecialMove ? _context.EnemySpecialMoveData.movementSpeed : _context.BaseMoveSpeed;
    }
    public IEnumerator SpawnAnimationCoroutine(float duration)
    {
        _characterController.enabled = false;
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, 0f, startPos.z);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;

            // Lerp from y=-1 to y=0
            transform.position = Vector3.Lerp(startPos, targetPos, normalizedTime);

            yield return null;
        }

        // Ensure final position is exact
        transform.position = targetPos;
        _characterController.enabled = true;
    }
    public void ResetStateContext()
    {
        _context.IsDead = false;
        _context.IsHurting = false;
        _context.IsMoreHurt = false;
        _context.IsStunned = false;
        _context.IsDoneAttacking = false;
        _context.IsDoneMoving = false;

    }
}
