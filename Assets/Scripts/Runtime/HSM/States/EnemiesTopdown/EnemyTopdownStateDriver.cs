using Ami.BroAudio;
using HSM;
using PixPlays.ElementalVFX;
using System;
using System.Collections;
using System.Collections.Generic;
using Turnbase;
using UnityEngine;
using UnityEngine.Analytics;

public class EnemyTopdownStateDriver : Flyweight
{
    [Serializable]
    private struct PhaseTransitionVFXData
    {
        public Transform SpawnTransform;
        public OneShotVFXSettings VFXSettings;
        public float Size;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

    }

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
    public bool isBoss = false;
    [ShowIf("isBoss"), SerializeField, TabGroup("BossSettings")]
    private List<PhaseTransitionVFXData> _phaseTransitionVFXDatasList = new();
    [ShowIf("isBoss"), SerializeField, TabGroup("BossSettings")]
    private SoundID _phaseTransitionSound;
    [ShowIf("isBoss"), SerializeField, TabGroup("BossSettings")]
    private SoundID _deathSound;
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
    private List<SkillIndicator> _skillIndicatorList = new();
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
            .SetIsBoss(isBoss)
            .SetAnimator(_animator)
            .SetCharacterController(_characterController)
            .SetNavMeshSteering(_navMeshSteering)
            .SetPlayerTransform(PlayerTopDownStateDriver.Instance.transform)
            .SetRootTransform(transform)
            .SetMoveSpeed(_characterStats.Speed * 1.1f)
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
        else if (stateInfo.IsName("PhaseTransition"))
        {
            _context.IsChangingPhase = false;
        }
    }
    public void OnTakeDamage(GameObject sender,float currentHealth, Vector3 knockBackDirection, float knockBackForce)
    {
        if(_context.IsDead || _context.IsChangingPhase) return;

        _context.KnockbackDirection = knockBackDirection;
        _context.KnockbackForce = knockBackForce;

        if (_context.IsStunned)
        {
            _animator.Play(_context.HurtHash); 
            return;
        }
        
        if(knockBackForce > 0 && !isBoss)
        {
            if (_skillIndicatorList.Count > 0)
            {
                foreach (var indicator in _skillIndicatorList)
                    indicator.ReturnToPool();
                _skillIndicatorList.Clear();
            }
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
        if (_skillIndicatorList.Count > 0)
        {
            foreach (var indicator in _skillIndicatorList)
                indicator.ReturnToPool();
            _skillIndicatorList.Clear();
        }
    }
    public void OnDeath()
    {
        _context.IsDead = true;
        GetComponent<EffectsManager>().RemoveAllActiveEffects();
        TopDownEnemyManager.Instance.OnEnemyDied(gameObject);

        if (isBoss)
        {
            BroAudio.Play(_deathSound).AsDominator();
            BroAudio.Stop(BroAudioType.Music);
        }
    }
    public void OnHealed()
    {

    }
    public void PlayPhaseTransitionSound()
    {
        if(_phaseTransitionSound.ToString() != "None")
            BroAudio.Play(_phaseTransitionSound).AsDominator();
    }
    public void StartPhaseTransition()
    {
        _context.IsHurting = true;
        _context.IsChangingPhase = true;
        if (_skillIndicatorList.Count > 0)
        {
            foreach (var indicator in _skillIndicatorList)
                indicator.ReturnToPool();
            _skillIndicatorList.Clear();
        }
    }

    public void ForceStopRotate()
    {
        _context.ForceStopFacingTarget = true;
    }
    public void OnAttackTrigger(int setParentForVFX = 0)
    {
        if (_context.CurrentEnemyAttackData.skillVFX == null) return;

        var rotationOffset = Quaternion.Euler(_context.CurrentEnemyAttackData.rotationOffset);
        var positionOffset = _context.CurrentEnemyAttackData.positionOffset;
        bool hasIndicators = _skillIndicatorList != null && _skillIndicatorList.Count > 0;
        int spawnCount = hasIndicators ? _skillIndicatorList.Count : 1;

        for (int i = 0; i < spawnCount; i++)
        {
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
                    // Use each indicator's transform if available, else fallback to target
                    spawnTransform = hasIndicators
                        ? _skillIndicatorList[i].transform
                        : _context.CurrentTargetTransform;
                    break;
            }

            Flyweight vfx = FlyweightFactory.Spawn(_context.CurrentEnemyAttackData.skillVFX);
            if (vfx.name == "StingRay_WaterBeam" || vfx.name == "StingRay_WaterBullet" || vfx.name == "StingRay_WaterBlast")
                spawnTransform = transform;

            vfx.FlyweightInitialize(
                spawnTransform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z),
                spawnTransform.rotation * rotationOffset,
                setParentForVFX == 1 ? spawnTransform : null);

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
                                targetPosOffset = new Vector3(0, 0.5f, 0);
                                break;
                            }
                    }

                    VfxData vfxData = new(baseVfx.transform, target, oneShotVFXSettings.DefaultLifeTime, _context.CurrentEnemyAttackData.skillSize, targetPosOffset);

                    baseVfx.Play(vfxData);
                    return;
                }

                if (oneShotVFX.TryGetComponent<BeholderHeavenStrike>(out var beholderHeavenStrike))
                {
                    beholderHeavenStrike.SetUp(gameObject, _context.CurrentEnemyAttackData.dodgeLayers, _characterStats.AttackDamage, _context.CurrentEnemyAttackData.skillSize);
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
                if (vfx.TryGetComponent<DamageDealer>(out var damageDealer))
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
                // Each projectile uses its indicator's forward, fallback to transform.forward
                Vector3 indicatorForward = hasIndicators
                    ? _skillIndicatorList[i].transform.forward
                    : transform.forward;

                if (vfx is BeholderMultiProjectile)
                {
                    (vfx as BeholderMultiProjectile).InitializeMultiProjectile(
                        gameObject,
                        rotationOffset * indicatorForward,
                        _context.CurrentEnemyAttackData.projectileSpeed,
                        _context.CurrentEnemyAttackData.projectileRange,
                        _context.CurrentEnemyAttackData.skillSize,
                        _context.CurrentEnemyAttackData.damage,
                        _context.CurrentEnemyAttackData.knockBackForce,
                        _context.CurrentEnemyAttackData.dealTrueDamage,
                        _context.CurrentEnemyAttackData.dodgeLayers,
                        2);

                    return;
                }
                (vfx as StraightProjectile).InitializeProjectile(
                    gameObject,
                    rotationOffset * indicatorForward,
                    _context.CurrentEnemyAttackData.projectileSpeed,
                    _context.CurrentEnemyAttackData.projectileRange,
                    _context.CurrentEnemyAttackData.skillSize,
                    _context.CurrentEnemyAttackData.damage,
                    _context.CurrentEnemyAttackData.knockBackForce,
                    _context.CurrentEnemyAttackData.dealTrueDamage,
                    _context.CurrentEnemyAttackData.dodgeLayers);

            }
        }

        // Clear indicators after all spawns are done
        if (hasIndicators)
        {
            _skillIndicatorList.Clear();
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
    public void ShowSkillIndicator(float rotationOffset = 0)
    {
        var choseVFXSetting = _context.CurrentEnemyAttackData.indicator;
        var indicatorFlyweight = FlyweightFactory.Spawn(_context.CurrentEnemyAttackData.indicator);

        SkillIndicator skillIndicator = indicatorFlyweight as SkillIndicator;
        _skillIndicatorList.Add(skillIndicator);

        Debug.Log("Indicator added: " + _skillIndicatorList.Count);
        if (indicatorFlyweight is FollowedIndicator)
        {
            skillIndicator.FlyweightInitialize(_context.RootTransform.position.Add(y:0.1f), _context.RootTransform.rotation);
            FollowedIndicator indicator = (FollowedIndicator)skillIndicator;
            indicator.Initialize(_context.RootTransform, _context.CurrentEnemyAttackData.indicatorWidth, _context.CurrentEnemyAttackData.indicatorLength, rotationOffset);
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
            skillIndicator.FlyweightInitialize(target.position.Add(y: 0.1f));
            CircleIndicator indicator = (CircleIndicator)skillIndicator;
            indicator.Initialize(_context.CurrentEnemyAttackData.indicatorWidth, target);
        }
    }
    public void LockIndicator(float duration)
    {
        if (_skillIndicatorList.Count == 0) return;
        foreach (var indicator in _skillIndicatorList)
            indicator.LockIndicator(duration);
    }
    public void StopIndicator()
    {
        if (_skillIndicatorList.Count == 0) return;
        foreach (var indicator in _skillIndicatorList)
            indicator.Stop();
    }

    public void StopMoving()
    {
        _context.CurrentSpeed = 0;
    }
    public void StartMoving()
    {
        _context.CurrentSpeed = _context.IsAttacking || _context.IsCharging ? _context.CurrentEnemyAttackData.movementSpeed : _context.BaseMoveSpeed;
    }
    public void SpawnPhaseTransitionVfx(int index)
    {
        OneShotVFX vfx = FlyweightFactory.Spawn(_phaseTransitionVFXDatasList[index].VFXSettings) as OneShotVFX;
        var rotationOffset = Quaternion.Euler(_phaseTransitionVFXDatasList[index].rotationOffset);
        var positionOffset = _phaseTransitionVFXDatasList[index].positionOffset;
        vfx.FlyweightInitialize(_phaseTransitionVFXDatasList[index].SpawnTransform.AddLocal(positionOffset.x, positionOffset.y, positionOffset.z) , _phaseTransitionVFXDatasList[index].SpawnTransform.rotation * rotationOffset);
        vfx.InitializeVFX(_phaseTransitionVFXDatasList[index].Size, _phaseTransitionVFXDatasList[index].VFXSettings.DefaultLifeTime);
    }

    public void TurnOffLight()
    {
        EnviromentManager.Instance.LerpLightIntensity(2,2).Forget();
    }

    public void StartShakeCamera(float duration)
    {
        CinemachineCameraController.Instance.ShakeSequence(duration);
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
    public bool GetIsChangingPhase()
    {
        return _context.IsChangingPhase;
    }
    #endregion
}
