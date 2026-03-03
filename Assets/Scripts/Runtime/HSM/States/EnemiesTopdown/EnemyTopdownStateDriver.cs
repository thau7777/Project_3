using HSM;
using System.Collections;
using System.Collections.Generic;
using Turnbase;
using UnityEngine;

public class EnemyTopdownStateDriver : Flyweight
{
    #region References
    [TabGroup("References")]
    private Animator _animator;
    [TabGroup("References")]
    private CharacterController _characterController;
    [TabGroup("References")]
    private CharacterStats _characterStats;
    #endregion

    #region Variables
    //public EnemyTopDownSettings.ElementalType elementType;
    [TabGroup("Movement Settings")]
    [SerializeField]
    EnemyTopdownMovementType _movementType = EnemyTopdownMovementType.Normal;

    [SerializeField, TabGroup("Movement Settings"),ShowIfEnumValue("_movementType", EnemyTopdownMovementType.Range)]
    private float _minRangeDistance = 5f;
    [SerializeField, TabGroup("Movement Settings"), ShowIfEnumValue("_movementType", EnemyTopdownMovementType.Range)]
    private float _maxRangeDistance = 10f;

    [SerializeField, TabGroup("Movement Settings")]
    [ShowIfEnumValue("_movementType", EnemyTopdownMovementType.Slime)]
    float _movePauseDuration = 1f;
    [SerializeField, TabGroup("Movement Settings")]
    float _rotateSpeed = 10f;

    [TabGroup("Attack Settings")]
    [SerializeField]
    private List<EnemyAttackData> _attackList = new();


    // Hide in inspector stuffs
    private SkillIndicator _skillIndicator;
    private Flyweight _chargeEffect;
    //private Flyweight _chargeEffect;
    #endregion

    [TabGroup("State Machine")]
    [SerializeField]
    private EnemyTopdownContext _context;
    StateMachine _machine;
    State _root;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _characterController.slopeLimit = 0.01f;
        _characterController.stepOffset = 0.01f;
        _characterStats = GetComponent<CharacterStats>();

        _context = new EnemyTopdownContext.Builder()
            .SetAnimator(_animator)
            .SetCharacterController(_characterController)
            .SetPlayerTransform(GameObject.FindWithTag("Player").transform)
            .SetRootTransform(transform)
            .SetMoveSpeed(_characterStats.Agility)
            .SetMovePauseDuration(_movePauseDuration)
            .SetRotateSpeed(_rotateSpeed)
            .SetEnemyType(_movementType)
            .SetEnemyAttackList(_attackList)
            .Build();
        if(_movementType == EnemyTopdownMovementType.Range)
        {
            _context.MinRangeDistance = _minRangeDistance;
            _context.MaxRangeDistance = _maxRangeDistance;
        }
        _root = new EnemyTopdownRoot(null, _context);
        _machine = new StateMachineBuilder(_root).Build();
    }
    private void Update()
    {
        _machine.Tick(Time.deltaTime);
    }
    public void OnEndOfAnimation()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        _context.ForceStopFacingTarget = false;
        if (stateInfo.IsTag("Charge"))
        {
            _context.IsCharging = false;
            _context.CurrentSpeed = _context.BaseMoveSpeed;
            _context.RotateSpeed = _context.BaseRotateSpeed;
        }
        else if (stateInfo.IsTag("Attack"))
        {
            _context.IsAttacking = false;
            _context.CurrentSpeed = _context.BaseMoveSpeed;
            _context.RotateSpeed = _context.BaseRotateSpeed;
        }
        else if (stateInfo.IsTag("Movement"))
        {
            _context.IsMoving = false;
        }
        else if (stateInfo.IsTag("Hurt"))
        {
            if(_context.IsStunned)
                _animator.CrossFade(_context.StunnedHash, 0.1f);
            _context.IsHurting = false;
        }
        else if (stateInfo.IsTag("Dead"))
        {
            FlyweightFactory.ReturnToPool(this);
        }
    }
    public void OnTakeDamage(GameObject sender,float currentHealth, Vector3 knockBackDirection, float knockBackForce)
    {
        if(_context.IsDead) return;

        _context.KnockbackDirection = knockBackDirection;
        _context.KnockbackForce = knockBackForce;

        if (_context.IsStunned)
        {
            _animator.Play(_context.HurtHash);
            return;
        }
        
        if(knockBackForce > 0)
        {
            if (_skillIndicator)
                _skillIndicator.ReturnToPool();
            if (!_context.IsHurting)
                _context.IsHurting = true;
            else
                _context.IsMoreHurt = true;
        }
        



        //if(sender.layer == LayerMask.NameToLayer("MinionTopDown"))
        //    _context.SetCurrentTarget(sender.transform);
        //else 
        //    _context.SetCurrentTarget(_context.PlayerTransform);
    }
    public void OnStunned(float duration)
    {
        _context.IsStunned = true;
        _context.StunDuration = duration;
    }
    public void OnDeath()
    {
        _context.IsDead = true;
        GetComponent<EffectsManager>().RemoveAllActiveEffects();
    }
    public void OnHealed()
    {

    }

    public void ForceStopRotate()
    {
        _context.ForceStopFacingTarget = true;
    }
    public void OnAttackTrigger(int setParentForVFX = 0)
    {
        if (_context.CurrentEnemyAttackData.skillEffect == null) return;
        Transform spawnTransform = _context.CurrentEnemyAttackData.skillSpawnTransform ?? transform;
        if (_skillIndicator && _context.CurrentEnemyAttackData.spawnType == EnemyAttackData.SpawnType.AtTarget)
        {
            spawnTransform = _skillIndicator.transform;
        }
        _skillIndicator = null;
        Flyweight vfx = FlyweightFactory.Spawn(_context.CurrentEnemyAttackData.skillEffect);
        
        var rotationOffset = Quaternion.Euler(_context.CurrentEnemyAttackData.rotationOffset);
        var positionOffset = _context.CurrentEnemyAttackData.positionOffset;
        vfx.FlyweightInitialize(
            spawnTransform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z),
            spawnTransform.rotation * rotationOffset,
            setParentForVFX == 1 ? spawnTransform : null); // Apply the rotation offset here
        if (vfx is OneShotVFX)
        {
            OneShotVFX oneShotVFX = (OneShotVFX) vfx;
            OneShotVFXSettings oneShotVFXSettings = (OneShotVFXSettings)_context.CurrentEnemyAttackData.skillEffect;
            if (vfx.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
            {
                hitBoxHandler.Setup(
                    gameObject,
                    _context.CurrentEnemyAttackData.dodgeLayers,
                    oneShotVFXSettings.hitboxOnOffTime,
                    oneShotVFXSettings.useTriggerStays,
                    oneShotVFXSettings.triggerStayTickInterval,
                    _context.CurrentEnemyAttackData.Parryable);
            }
            if(vfx.TryGetComponent<DamageDealer>(out var damageDealer))
            {
                damageDealer.Setup(
                    _context.CurrentEnemyAttackData.damage,
                    _context.CurrentEnemyAttackData.dealTrueDamage,
                    _context.CurrentEnemyAttackData.knockBackForce,
                    _context.CurrentEnemyAttackData.reverseKnockbackDirection,
                    (settings as EnemyTopDownSettings).elementalType,
                    oneShotVFXSettings.hitImpactVFXSetting);
            }
            (vfx as OneShotVFX).InitializeVFX(_context.CurrentEnemyAttackData.skillSize,
                _context.CurrentEnemyAttackData.skillDuration);
            
        }
            
        else if (vfx is StraightProjectile)
        {
            (vfx as StraightProjectile).InitializeProjectile(
                gameObject,
                transform.forward,
                _context.CurrentEnemyAttackData.projectileSpeed,
                _context.CurrentEnemyAttackData.skillDuration,
                _context.CurrentEnemyAttackData.skillSize,
                _context.CurrentEnemyAttackData.damage,
                _context.CurrentEnemyAttackData.knockBackForce,
                _context.CurrentEnemyAttackData.dealTrueDamage,
                _context.CurrentEnemyAttackData.dodgeLayers,
                (settings as EnemyTopDownSettings).elementalType);
        }

        if (_chargeEffect)
        {
            _chargeEffect.ReturnToPool();
            _chargeEffect = null;
        }
            
    }

    public void SpawnChargeEffect()
    {
        var chargeSettings = _context.CurrentEnemyAttackData.chargeEffect;
        _chargeEffect = FlyweightFactory.Spawn(chargeSettings);
        OneShotVFX vfx = _chargeEffect as OneShotVFX;
        vfx.FlyweightInitialize(_context.CurrentEnemyAttackData.chargeSpawnTransform.position.Add(y: 0.01f));
        vfx.InitializeVFX(_context.CurrentEnemyAttackData.chargeEffectSize, _context.CurrentEnemyAttackData.chargeDuration);
    }
    public void ShowSkillIndicator()
    {
        var choseVFXSetting = _context.CurrentEnemyAttackData.indicator;
        var indicatorFlyweight = FlyweightFactory.Spawn(_context.CurrentEnemyAttackData.indicator);

        _skillIndicator = indicatorFlyweight as SkillIndicator;

        if (indicatorFlyweight is FollowedIndicator)
        {
            _skillIndicator.FlyweightInitialize(_context.RootTransform.position.Add(y:0.1f), _context.RootTransform.rotation);
            FollowedIndicator indicator = (FollowedIndicator)_skillIndicator;
            indicator.Initialize(_context.RootTransform, _context.CurrentEnemyAttackData.indicatorWidth, _context.CurrentEnemyAttackData.indicatorLength);
        }
        else
        {
            Transform target = null;
            switch(_context.CurrentEnemyAttackData.spawnType)
            {
                case EnemyAttackData.SpawnType.AtCustomSpawnTransform:
                    target = _context.CurrentEnemyAttackData.skillSpawnTransform;
                    break;
                case EnemyAttackData.SpawnType.AtSelf:
                    target = _context.RootTransform;
                    break;
                case EnemyAttackData.SpawnType.AtTarget:
                    target = _context.PlayerTransform;
                    break;
            }
            _skillIndicator.FlyweightInitialize(target.position.Add(y: 0.1f));
            CircleIndicator indicator = (CircleIndicator)_skillIndicator;
            indicator.Initialize(_context.CurrentEnemyAttackData.indicatorWidth, target);
        }
    }
    public void LockIndicator(float duration)
    {
        if (!_skillIndicator) return;
        _skillIndicator.LockIndicator(duration);
    }
    public void StopIndicator()
    {
        if(!_skillIndicator) return;
        _skillIndicator.Stop();
    }

    public void StopMoving()
    {
        _context.CurrentSpeed = 0;
    }
    public void StartMoving()
    {
        _context.CurrentSpeed = _context.IsAttacking || _context.IsCharging ? _context.CurrentEnemyAttackData.movementSpeed : _context.BaseMoveSpeed;
    }

    #region Outside Calls
    public void StartSpawnAnim()
    {
        StartCoroutine(SpawnAnimationCoroutine(((EnemyTopDownSettings)settings).spawnAnimationTime));
    }
    public IEnumerator SpawnAnimationCoroutine(float duration)
    {
        // 1. Disable Controller so we can move the transform manually
        _characterController.enabled = false;

        float elapsedTime = 0f;
        Vector3 startPos = transform.position; // Assumes enemy is already spawned underground
        Vector3 rayStartPos = transform.position.Add(y: 10f);
        // --- FIX START ---
        // Raycast upward from current position to find the ground surface
        Vector3 targetPos = startPos;

        if (Physics.Raycast(rayStartPos, Vector3.down, out RaycastHit hit, 15, LayerMask.GetMask("Ground")))
        {
            // Found ground above, set target to surface level
            targetPos = new Vector3(startPos.x, hit.point.y, startPos.z);
        }
        else
        {
            Debug.LogWarning("Could not find ground surface for spawn animation!");
            targetPos = new Vector3(startPos.x, startPos.y + 2f, startPos.z); // Emergency fallback
        }
        // --- FIX END ---

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;

            // Optional: Use SmoothStep for a more natural "Emerge" movement (Start slow, end slow)
            // If you want linear movement, just use 'normalizedTime' instead of 'curve'
            float curve = Mathf.SmoothStep(0, 1, normalizedTime);

            transform.position = Vector3.Lerp(startPos, targetPos, curve);

            yield return null;
        }

        // 2. Snap to exact target and re-enable controller
        transform.position = targetPos;
        _characterController.enabled = true;
        _context.IsSpawning = false;
    }
    public void ResetStateContext()
    {
        _context.IsDead = false;
        _context.IsHurting = false;
        _context.IsMoreHurt = false;
        _context.IsStunned = false;
        _context.IsAttacking = false;
        _context.IsMoving = false;

    }

    public void SetMoveSpeed(float newSpeed)
    {
        _context.BaseMoveSpeed = newSpeed;
    }
    #endregion
}
