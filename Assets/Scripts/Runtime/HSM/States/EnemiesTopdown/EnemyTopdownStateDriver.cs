using HSM;
using PixPlays.ElementalVFX;
using System.Collections;
using System.Collections.Generic;
using Turnbase;
using UnityEngine;
using UnityEngine.Analytics;

public class EnemyTopdownStateDriver : Flyweight
{
    #region References
    [TabGroup("References")]
    private Animator _animator;
    [TabGroup("References")]
    private CharacterController _characterController;
    [TabGroup("References")] 
    private NavMeshSteering _navMeshSteering;
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
    [ShowIfEnumValue("_movementType", EnemyTopdownMovementType.Slime, EnemyTopdownMovementType.Range)]
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
        _navMeshSteering = gameObject.GetOrAdd<NavMeshSteering>();

        _context = new EnemyTopdownContext.Builder()
            .SetAnimator(_animator)
            .SetCharacterController(_characterController)
            .SetNavMeshSteering(_navMeshSteering)
            .SetPlayerTransform(PlayerTopDownStateDriver.Instance.transform)
            .SetRootTransform(transform)
            .SetMoveSpeed(_characterStats.Speed * 1.25f)
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
        TopDownEnemyManager.Instance.OnEnemyDied(gameObject);
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
        if (_context.CurrentEnemyAttackData.skillVFX == null) return;


        Transform spawnTransform = null;
        switch (_context.CurrentEnemyAttackData.spawnType)
        {
            case EnemyAttackData.SpawnType.AtCustomSpawnTransform:
                spawnTransform = _context.CurrentEnemyAttackData.skillSpawnTransform;
                break;
            case EnemyAttackData.SpawnType.AtSelf:
                spawnTransform = _context.RootTransform;
                break;
            case EnemyAttackData.SpawnType.AtTarget:
                spawnTransform = _context.CurrentTargetTransform;
                break;
        }

        if (_skillIndicator && _context.CurrentEnemyAttackData.spawnType == EnemyAttackData.SpawnType.AtTarget)
        {
            spawnTransform = _skillIndicator.transform;
            _skillIndicator = null;
        }
        Flyweight vfx = FlyweightFactory.Spawn(_context.CurrentEnemyAttackData.skillVFX);
        if (vfx.name == "StingRay_WaterBeam" || vfx.name == "StingRay_WaterBullet" || vfx.name == "StingRay_WaterBlast")
            spawnTransform = transform;

        var rotationOffset = Quaternion.Euler(_context.CurrentEnemyAttackData.rotationOffset);
        var positionOffset = _context.CurrentEnemyAttackData.positionOffset;
        vfx.FlyweightInitialize(
            spawnTransform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z),
            spawnTransform.rotation * rotationOffset,
            setParentForVFX == 1 ? spawnTransform : null); // Apply the rotation offset here

        if (vfx is OneShotVFX)
        {
            OneShotVFX oneShotVFX = (OneShotVFX)vfx;
            OneShotVFXSettings oneShotVFXSettings = (OneShotVFXSettings)_context.CurrentEnemyAttackData.skillVFX;

            // SpecialVfx Handling
            if (oneShotVFX.TryGetComponent<BaseVfx>(out var baseVfx))
            {
                baseVfx.gameObject.SetActive(true);
                baseVfx.transform.localScale = Vector3.one * _context.CurrentEnemyAttackData.skillSize;
                Transform target = null;
                switch (_context.CurrentEnemyAttackData.spawnType)
                {
                    case EnemyAttackData.SpawnType.AtCustomSpawnTransform:
                        target = _context.CurrentEnemyAttackData.skillSpawnTransform;
                        break;
                    case EnemyAttackData.SpawnType.AtSelf:
                        target = _context.RootTransform;
                        break;
                    case EnemyAttackData.SpawnType.AtTarget:
                        target = _context.CurrentTargetTransform;
                        break;
                }


                SetupVfxHitBox(baseVfx.transform, oneShotVFXSettings);
                Vector3 targetPosOffset = Vector3.zero;
                switch (vfx.name)
                {
                    case "StingRay_WaterBeam":
                        {
                            targetPosOffset = Vector3.zero;
                            break;
                        }
                    case "StingRay_WaterBullet":
                        {
                            targetPosOffset = new Vector3(0,0.5f,0);
                            break;
                        }
                }

                VfxData vfxData = new(baseVfx.transform, target, oneShotVFXSettings.DefaultLifeTime, _context.CurrentEnemyAttackData.skillSize, targetPosOffset);
                baseVfx.Play(vfxData);
                return;
            }
            

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
                    oneShotVFXSettings.isMagicAttack,
                    _characterStats.AttackDamage,
                    _context.CurrentEnemyAttackData.dealTrueDamage,
                    _context.CurrentEnemyAttackData.knockBackForce,
                    _context.CurrentEnemyAttackData.reverseKnockbackDirection,
                    oneShotVFXSettings.elementalType,
                    oneShotVFXSettings.hitImpactVFXSetting);

                if (oneShotVFXSettings.UseParticleCollision)
                    damageDealer.SetupParicleDamageDealer(gameObject);
            }
            if (vfx.TryGetComponent<EffectApplier>(out var effectApplier))
            {
                effectApplier.SetUpForParticle(gameObject);
            }
            (vfx as OneShotVFX).InitializeVFX(_context.CurrentEnemyAttackData.skillSize,
                oneShotVFXSettings.DefaultLifeTime);
            
        }
        else if (vfx is StraightProjectile)
        {
            
            (vfx as StraightProjectile).InitializeProjectile(

                gameObject,
                transform.forward,
                _context.CurrentEnemyAttackData.projectileSpeed,
                _context.CurrentEnemyAttackData.projectileRange,
                _context.CurrentEnemyAttackData.skillSize,
                _context.CurrentEnemyAttackData.damage,
                _context.CurrentEnemyAttackData.knockBackForce,
                _context.CurrentEnemyAttackData.dealTrueDamage,
                _context.CurrentEnemyAttackData.dodgeLayers);
        }

        if (_chargeEffect)
        {
            _chargeEffect.ReturnToPool();
            _chargeEffect = null;
        }
            
    }
    private void SetupVfxHitBox(Transform parent, OneShotVFXSettings settings)
    {
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent<HitBoxHandler>(out var specialVfxHitBoxHandler))
            {
                specialVfxHitBoxHandler.Setup(
                    gameObject,
                    _context.CurrentEnemyAttackData.dodgeLayers,
                    settings.hitboxOnOffTime,
                    settings.useTriggerStays,
                    settings.triggerStayTickInterval,
                    _context.CurrentEnemyAttackData.Parryable);

                if (child.TryGetComponent<DamageDealer>(out var specialVfxDamageDealer))
                {
                    specialVfxDamageDealer.Setup(
                        settings.isMagicAttack,
                        _characterStats.AttackDamage,
                        _context.CurrentEnemyAttackData.dealTrueDamage,
                        _context.CurrentEnemyAttackData.knockBackForce,
                        _context.CurrentEnemyAttackData.reverseKnockbackDirection,
                        settings.elementalType,
                        settings.hitImpactVFXSetting,
                        false);

                    if (settings.UseParticleCollision)
                        specialVfxDamageDealer.SetupParicleDamageDealer(gameObject);
                }

                if (child.TryGetComponent<EffectApplier>(out var specialVfxEffectApplier))
                {
                    specialVfxEffectApplier.SetEffects(settings.effectsToApplyList);
                    specialVfxEffectApplier.SetUpForParticle(gameObject);
                }

                return; // Found it — stop traversal
            }

            SetupVfxHitBox(child, settings); // Recurse into children
        }
    }
    public void SpawnChargeEffect()
    {
        OneShotVFXSettings chargeSettings = _context.CurrentEnemyAttackData.chargeEffect;
        _chargeEffect = FlyweightFactory.Spawn(chargeSettings);
        OneShotVFX vfx = _chargeEffect as OneShotVFX;
        vfx.FlyweightInitialize(_context.CurrentEnemyAttackData.chargeSpawnTransform.position.Add(y: 0.01f));
        vfx.InitializeVFX(_context.CurrentEnemyAttackData.chargeEffectSize, chargeSettings.DefaultLifeTime);
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
                    target = _context.CurrentTargetTransform;
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
    public void StartSpawnAnim(float groundSurfaceY)
    {
        StartCoroutine(SpawnAnimationCoroutine(((EnemyTopDownSettings)settings).spawnAnimationDuration, groundSurfaceY));
    }

    public IEnumerator SpawnAnimationCoroutine(float duration, float groundSurfaceY)
    {
        _characterController.enabled = false;

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, groundSurfaceY, startPos.z);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float curve = Mathf.SmoothStep(0, 1, elapsedTime / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, curve);
            yield return null;
        }

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
        _context.MoveSpeedAnimScale = _context.BaseMoveSpeed / _context.OgMoveSpeed;
    }
    #endregion
}
