using HSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTopdownStateDriver : Flyweight
{
    #region References
    [TabGroup("References")]
    private Animator _animator;
    [TabGroup("References")]
    private CharacterController _characterController;

    [TabGroup("References")]
    [SerializeField]
    private OneShotVFXSettings _stunVfxSettings;
    [TabGroup("References")]
    [SerializeField]
    private Transform _stunVfxSpawnTransform;
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
    float _moveSpeed = 2f;
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

        _context = new EnemyTopdownContext.Builder()
            .SetAnimator(_animator)
            .SetCharacterController(_characterController)
            .SetPlayerTransform(GameObject.FindWithTag("Player").transform)
            .SetRootTransform(transform)
            .SetMoveSpeed(_moveSpeed)
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
        if(_context.IsDead || _context.IsAttacking) return;

        _context.KnockbackDirection = knockBackDirection;
        _context.KnockbackForce = knockBackForce;

        if (_context.IsStunned)
        {
            _animator.Play(_context.HurtHash);
            return;
        }
            
        if (!_context.IsHurting)
            _context.IsHurting = true;
        else 
            _context.IsMoreHurt = true;



        //if(sender.layer == LayerMask.NameToLayer("MinionTopDown"))
        //    _context.SetCurrentTarget(sender.transform);
        //else 
        //    _context.SetCurrentTarget(_context.PlayerTransform);
    }
    public void OnStunned(float duration)
    {
        // spawn stun VFX here
        if (_stunVfxSettings)
        {
            OneShotVFX stunVfx = FlyweightFactory.Spawn(_stunVfxSettings) as OneShotVFX;
            OneShotVFXSettings oneShotVFXSettings = stunVfx.settings as OneShotVFXSettings;
            stunVfx.FlyweightInitialize(_stunVfxSpawnTransform.position);
            stunVfx.InitializeVFX(oneShotVFXSettings.DefaultSize, duration);
        }

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
        Transform spawnTransform = _context.CurrentEnemyAttackData.skillSpawnTransform;
        if (_skillIndicator && _skillIndicator.isMovementLocked)
        {
            spawnTransform = _skillIndicator.transform;
            _skillIndicator = null;
        }
        Flyweight vfx = FlyweightFactory.Spawn(_context.CurrentEnemyAttackData.skillEffect);
        
        var rotationOffset = Quaternion.Euler(_context.CurrentEnemyAttackData.rotationOffset);
        var positionOffset = _context.CurrentEnemyAttackData.positionOffset;
        vfx.FlyweightInitialize(
            spawnTransform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z),
            spawnTransform.rotation * rotationOffset); // Apply the rotation offset here
        if (vfx is OneShotVFX)
        {
            if (vfx.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
            {
                hitBoxHandler.DodgeLayers = _context.CurrentEnemyAttackData.dodgeLayers;
            }
            if(vfx.TryGetComponent<DamageDealer>(out var damageDealer))
            {
                damageDealer.Damage = _context.CurrentEnemyAttackData.damage;
            }
            (vfx as OneShotVFX).InitializeVFX(_context.CurrentEnemyAttackData.skillSize,
                _context.CurrentEnemyAttackData.skillDuration,
                setParentForVFX == 1 ? _context.CurrentEnemyAttackData.skillSpawnTransform : null);
            
        }
            
        else if (vfx is StraightProjectile)
        {
            (vfx as StraightProjectile).InitializeProjectile(
                transform.forward, 
                _context.CurrentEnemyAttackData.projectileSpeed, 
                _context.CurrentEnemyAttackData.skillDuration, 
                _context.CurrentEnemyAttackData.skillSize,
                _context.CurrentEnemyAttackData.damage,
                _context.CurrentEnemyAttackData.dodgeLayers);
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
            _skillIndicator.FlyweightInitialize(_context.RootTransform.position, _context.RootTransform.rotation);
            FollowedIndicator indicator = (FollowedIndicator)_skillIndicator;
            indicator.Initialize(_context.RootTransform, _context.CurrentEnemyAttackData.indicatorWidth, _context.CurrentEnemyAttackData.indicatorLength);
        }
        else
        {
            _skillIndicator.FlyweightInitialize(_context.CurrentEnemyAttackData.skillSpawnTransform.position);
            CircleIndicator indicator = (CircleIndicator)_skillIndicator;
            indicator.Initialize(_context.CurrentEnemyAttackData.indicatorWidth,Mathf.Infinity, _context.CurrentEnemyAttackData.skillSpawnTransform);
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
    public void StartSpawnAnim()
    {
        StartCoroutine(SpawnAnimationCoroutine(((EnemyTopDownSettings)settings).spawnAnimationTime));
    }
    public IEnumerator SpawnAnimationCoroutine(float duration)
    {
        _context.IsSpawning = true;
        // 1. Disable Controller so we can move the transform manually
        _characterController.enabled = false;

        float elapsedTime = 0f;
        Vector3 startPos = transform.position; // Assumes enemy is already spawned underground

        // --- FIX START ---
        // Calculate the actual surface height at this specific X, Z coordinate
        // We add terrain.transform.position.y just in case the terrain object isn't at Y=0
        float surfaceY = Terrain.activeTerrain.SampleHeight(startPos) + Terrain.activeTerrain.transform.position.y;

        Vector3 targetPos = new Vector3(startPos.x, surfaceY, startPos.z);
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
}
