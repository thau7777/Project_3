using HSM;
using MyRule;
using System;
using System.Collections.Generic;
using Turnbase;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct ClassWeaponData
{
    public CharacterClass Class;
    public List<GameObject> weaponOfThisClass;
}


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerTopDownStateDriver : Singleton<PlayerTopDownStateDriver>
{
    #region References
    [SerializeField, Required]
    [TabGroup("References")]
    private InputReader _inputReader;

    [SerializeField, Required]
    [TabGroup("References")]
    private LocomotionSet _locomotionSet;

    [SerializeField, Required]
    [TabGroup("References")]
    private CharacterController _controller;

    [SerializeField, Required]
    [TabGroup("References")]
    private CCLayerIgnoreController _layerIgnoreController;

    [SerializeField, Required]
    [TabGroup("References")]
    private Animator _animator;

    [SerializeField, Required]
    [TabGroup("References")]
    private Renderer _renderer;

    [SerializeField, Required]
    [TabGroup("References")]
    private SkillExecutor _executor;

    [SerializeField, Required]
    [TabGroup("References")]
    private CharacterStats _characterStats;

    [SerializeField]
    private List<ClassWeaponData> classWeaponDataList;


    [SerializeField]
    List<Transform> _attackVfxSpawnPoints;
    #endregion

    #region Variables
    [SerializeField, TabGroup("Movement Settings")] private float _strafeMoveSpeed = 2f;   // default
    [SerializeField, TabGroup("Movement Settings")] private float _smoothTime = 0.2f;    // higher = slower accel/dec
    [SerializeField, TabGroup("Movement Settings")] private float _rotateSpeed = 20f;

    [SerializeField]
    private PlayerTopdownContext _context;
    StateMachine machine;
    State root;

    Vector3 _rotateDirOnAttack;

    bool _isInSpecialMoveAnim
        => _animator.GetCurrentAnimatorStateInfo(_context.IsUseSkillByUpperBody ? 1 : 0).IsTag("SpecialMove");

    bool _isInAttackAnim
        => _animator.GetCurrentAnimatorStateInfo(_context.IsRangeClass ? 1 : 0).IsTag("Attack");

    bool _isInHurtAnim => _animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt");

    [field: SerializeField] public bool IsParrying { get; set; } = false;

    private EventBinding<TopDownEndGameEvent> _onEndGameEventBinding;
    #endregion

    #region SummonerStuffs
    [SerializeField]
    private GameObject _minionManagerPrefab;

    private Vector3 _savedMousePosition;
    #endregion

    #region Initialization
    protected override void Awake()
    {
        base.Awake();
        if (_locomotionSet.characterClass == CharacterClass.Summoner)
            Instantiate(_minionManagerPrefab);

        _controller = GetComponent<CharacterController>();
        _layerIgnoreController = GetComponent<CCLayerIgnoreController>();
        _animator = GetComponent<Animator>();
        _executor = GetComponent<SkillExecutor>();
        _characterStats = GetComponent<CharacterStats>();
        _animator.runtimeAnimatorController = _locomotionSet.animationController;

        if (!TopDownGameManager.Instance.isTestGameplay)
        {
            CharacterData playerStats = CharacterManager.Instance.GetCharacterStats();
            BaseStatsData baseStatsData = playerStats.CharacterStatsData.BaseStatsData;
            DamageData damageData = playerStats.CharacterStatsData.Damage;
            DefenseData defenseData = playerStats.CharacterStatsData.Defense;
            _characterStats.Setup(ElementalType.Normal, baseStatsData.MaxHealth, 0, baseStatsData.MaxMana, Mathf.RoundToInt(damageData.PhysDmg * 0.6f),
                Mathf.RoundToInt(damageData.MagDmg * 0.6f), Mathf.RoundToInt(damageData.FireDmg * 0.6f), Mathf.RoundToInt(damageData.WaterDmg * 0.6f), Mathf.RoundToInt(damageData.FrostDmg * 0.6f), Mathf.RoundToInt(damageData.LightningDmg * 0.6f), Mathf.RoundToInt(damageData.HolyDmg * 0.6f),
                Mathf.RoundToInt(damageData.DarkDmg * 0.6f), Mathf.RoundToInt(damageData.PoisonDmg * 0.6f), defenseData.PhysDef, defenseData.MagDef, defenseData.FireDef, defenseData.WaterDef,
                defenseData.FrostDef, defenseData.LightningDef, defenseData.HolyDef, defenseData.DarkDef, defenseData.PoisonDef, baseStatsData.Speed,
                baseStatsData.CritChance, baseStatsData.CritMult);

            GetComponent<Damageable>().Initialize(baseStatsData.CurrentHealth, baseStatsData.MaxHealth, 0);
            _executor.InitializeMana(baseStatsData.MaxMana);
        }
        else
        {
            _characterStats.Setup(ElementalType.Normal, _characterStats.InitialHealth, 0, _characterStats.InitialMana, _characterStats.AttackDamage,
                _characterStats.MagicAttackDamage, _characterStats.FireDamage, _characterStats.WaterDamage, _characterStats.FrostDamage, _characterStats.LightningDamage, _characterStats.HolyDamage,
                _characterStats.DarkDamage, _characterStats.PoisonDamage, _characterStats.PhysicalDefense, _characterStats.MagicDefense, _characterStats.FireDefense, _characterStats.WaterDefense,
                _characterStats.FrostDefense, _characterStats.LightningDefense, _characterStats.HolyDefense, _characterStats.DarkDefense, _characterStats.PoisonDefense, _characterStats.Speed,
                _characterStats.CriticalRate, _characterStats.CriticalMultiplier);

            GetComponent<Damageable>().Initialize(_characterStats.InitialHealth, _characterStats.InitialHealth, 0);
            _executor.InitializeMana(_characterStats.InitialMana);
        }

        _context = new PlayerTopdownContext.Builder()
        .SetBaseMoveSpeed(_characterStats.Speed / 2)
        .SetStrafeMoveSpeed(_strafeMoveSpeed)
        .SetMoveSpeedSmoothTime(_smoothTime)
        .SetRotateSpeed(_rotateSpeed)
        .SetAnimator(_animator)
        .SetCharacterController(_controller)
        .SetRenderer(_renderer)
        .SetMainCameraTransform(Camera.main.transform)
        .SetRootTransform(transform)
        .SetLocomotionSet(_locomotionSet)
        .Build();


        root = new PlayerTopdownRoot(null, _context);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();

        InitializeClassWeapon(_locomotionSet.characterClass);

        _context.IsSpawning = true;

    }
    private void OnEnable()
    {
        _inputReader.playerTopDownActions.onMove += OnMove;
        _inputReader.playerTopDownActions.onLeftClick += OnLeftClick;
        _inputReader.playerTopDownActions.onSkillUse += OnSkillUse;

        _onEndGameEventBinding = new(OnEndGame);
        EventBus<TopDownEndGameEvent>.Register(_onEndGameEventBinding);
    }
    private void OnDisable()
    {
        _inputReader.playerTopDownActions.onMove -= OnMove;
        _inputReader.playerTopDownActions.onLeftClick -= OnLeftClick;
        _inputReader.playerTopDownActions.onSkillUse -= OnSkillUse;

        EventBus<TopDownEndGameEvent>.Deregister(_onEndGameEventBinding);

    }


    private void InitializeClassWeapon(CharacterClass characterClass)
    {
        foreach (var classWeaponData in classWeaponDataList)
        {
            bool isActiveClass = classWeaponData.Class == characterClass;
            foreach (var weapon in classWeaponData.weaponOfThisClass)
            {
                if (weapon != null)
                    weapon.SetActive(isActiveClass);
            }
        }
    }
    #endregion

    #region Input Handlers
    private void OnSkillUse(bool value, int skillIndex)
    {
        if (_context.IsSpawning || _context.IsDespawning || _context.IsVictory || _context.IsHurting || _isInHurtAnim || _isInSpecialMoveAnim || _context.IsInSpecialMove) return;
        UseSKill(skillIndex, value, SaveDirToAttack);
    }

    private void UseSKill(int skillIndex, bool isPressed, Action onCastInstantly = null)
    {
        if ( _context.CastingSkill != -1 && _context.CastingSkill != skillIndex) return;

        if (isPressed)
        {
            _executor.UseSkill(skillIndex, _locomotionSet.characterClass, _context, onCastInstantly);
        }
        else if (_context.CastingSkill != -1) // currently charging skill or aiming
        {
            _executor.CastSkill(_context);
            onCastInstantly?.Invoke();
        }

    }
    private void OnMove(Vector2 vector)
    {
        if (_context.IsSpawning || _context.IsDespawning || _context.IsVictory) return;
        _context.MoveInput = vector;
    }
    public void OnLeftClick()
    {
        if (_context.IsSpawning || _context.IsDespawning || _context.IsVictory || _context.IsHurting || _isInHurtAnim || _isInSpecialMoveAnim || _context.IsInSpecialMove) return;
        SaveDirToAttack();
        if (_context.IsNextAttackQueued ||
            !_context.IsAttacking && _isInAttackAnim) return;
        if (_context.CastingSkill != -1)
        {
            _executor.CastSkill(_context);
            return;
        }

        if (_locomotionSet.characterClass == CharacterClass.Summoner)
            TrySaveMousePosition();

        if (!_context.IsAttacking)
        {
            _context.IsAttacking = true;
            _context.FirstAttackAnimName = _locomotionSet.FirstComboAttack.animName;
        }
        else
        {
            _context.IsNextAttackQueued = true;
            _locomotionSet.QueueNextComboAttack();
        }
    }

    private void SaveDirToAttack()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            Vector3 hitPoint = ray.GetPoint(hitDist);

            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f; // keep it flat on ground

            if (lookDir.sqrMagnitude > 0.001f)
            {
                _rotateDirOnAttack = lookDir.normalized;
            }
        }
    }

    #endregion

    #region Loop
    private void Update()
    {
        machine.Tick(Time.deltaTime);
        if (_context.IsVictory)
        {
            _context.MoveInput = Vector2.zero;
            _context.CurrentMoveSpeed = 0;

        }
    }

    #endregion

    #region Animation Events
    public void ApplyRotation()
    {
        _context.RotateDir = _context.IsDashing ? (_context.DesiredMoveDir == Vector3.zero ? transform.forward : _context.DesiredMoveDir) : _rotateDirOnAttack;
    }
    public void ApplyDash()
    {
        _context.CurrentMoveSpeed = 0; // reset current speed
        _context.MoveSpeedSmoothTime = _context.IsDashing ? 0.01f : 0.05f; // quick accel
        bool isDashForward;
        float dashForce;
        if (_context.IsInSpecialMove)
        {
            isDashForward = _executor.StoredSkillDataForClass.Value.isDashForward;
            dashForce = _executor.StoredSkillDataForClass.Value.dashForce;

        }
        else
        {
            isDashForward = _locomotionSet.CurrentAttackData.isDashForward;
            dashForce = _locomotionSet.CurrentAttackData.dashForce;
        }
        _context.TargetMoveSpeed = dashForce;
        _context.MoveDir = _context.IsDashing ? (_context.DesiredMoveDir == Vector3.zero ? transform.forward : _context.DesiredMoveDir) : _rotateDirOnAttack * (isDashForward ? 1 : -1);

    }
    public void StopMoving(int isDashTrigerredThis = 0) // 0 means not the dash anim triggered this
    {
        if (_context.IsDashing && isDashTrigerredThis == 0) return;
        _context.TargetMoveSpeed = 0; // stop moving when attacking
        _context.MoveSpeedSmoothTime = 0.1f;

    }

    public void OnAttackTrigger()
    {
        SpawnVFX(_locomotionSet.CurrentAttackData.flyweightSettings, _locomotionSet.CurrentAttackData.spawnLocation);
    }
    private void SpawnVFX(FlyweightSettings flyweightSettings, VFXSpawnLocation location)
    {
        if (location.ToString() == "Mouse")
        {
            Flyweight vfx = FlyweightFactory.Spawn(flyweightSettings); // do the onGet stuff

            vfx.FlyweightInitialize(_savedMousePosition);
            if (vfx is OneShotVFX)
            {
                var oneshotVFX = vfx as OneShotVFX;
                var oneshotSettings = oneshotVFX.settings as OneShotVFXSettings;
                if (oneshotVFX.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
                {
                    hitBoxHandler.Setup(
                        gameObject,
                        _locomotionSet.CurrentAttackData.dodgeLayers,
                        oneshotSettings.hitboxOnOffTime,
                        oneshotSettings.useTriggerStays,
                        oneshotSettings.triggerStayTickInterval,
                        false);
                }
                if (oneshotVFX.TryGetComponent<DamageDealer>(out var damageDealer))
                {
                    int finalDamage = Mathf.RoundToInt(_locomotionSet.CurrentAttackData.damageScale * _characterStats.AttackDamage);
                    damageDealer.Setup(
                        oneshotSettings.isMagicAttack,
                        finalDamage,
                        false,
                        _locomotionSet.CurrentAttackData.knockbackForce,
                        false);

                    if (oneshotSettings.UseParticleCollision)
                        damageDealer.SetupParicleDamageDealer(gameObject);
                }
                if (oneshotVFX.TryGetComponent<EffectApplier>(out var effectApplier))
                {
                    effectApplier.SetUpForParticle(gameObject);
                }
                oneshotVFX.InitializeVFX(oneshotSettings.DefaultSize, oneshotSettings.DefaultLifeTime);
            }
            if (flyweightSettings.name == "SummonerBasicAttack")
            {
                MinionsManager.Instance.RemoveAllTargetedEnemies();
            }
            return;
        }

        foreach (var spawnPoint in _attackVfxSpawnPoints)
        {
            
            if (location.ToString() == spawnPoint.name)
            {
                bool dealTrueDmg = false;
                if (TryGetComponent<EffectsManager>(out var effectsManager))
                {
                    if (effectsManager.HasEffect("Show Your True Form"))
                    {
                        dealTrueDmg = true;
                    }
                }
                Flyweight vfx = FlyweightFactory.Spawn(flyweightSettings); // do the onGet stuff
                vfx.FlyweightInitialize(spawnPoint.position, transform.rotation); // set position
                int finalDamage = Mathf.RoundToInt(_locomotionSet.CurrentAttackData.damageScale * _characterStats.AttackDamage);
                if (vfx is StraightProjectile)
                {
                    var straightProjectile = vfx as StraightProjectile;


                    straightProjectile.InitializeProjectile(
                        gameObject,
                        spawnPoint.forward,
                        _locomotionSet.CurrentAttackData.projectileSpeed,
                        _locomotionSet.CurrentAttackData.projectileDuration,
                        _locomotionSet.CurrentAttackData.size * _characterStats.AttackSizeScale,
                        finalDamage,
                        _locomotionSet.CurrentAttackData.knockbackForce,
                        dealTrueDmg,
                        _locomotionSet.CurrentAttackData.dodgeLayers);
                }
                else if (vfx is OneShotVFX)
                {
                    var oneshotVFX = vfx as OneShotVFX;
                    var oneshotSettings = oneshotVFX.settings as OneShotVFXSettings;

                    if (oneshotVFX.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
                    {
                        hitBoxHandler.Setup(
                            gameObject,
                            _locomotionSet.CurrentAttackData.dodgeLayers,
                            oneshotSettings.hitboxOnOffTime,
                            oneshotSettings.useTriggerStays,
                            oneshotSettings.triggerStayTickInterval,
                            false);
                    }

                    if (oneshotVFX.TryGetComponent<DamageDealer>(out var damageDealer))
                    {
                        damageDealer.Setup(
                            oneshotSettings.isMagicAttack,
                            finalDamage,
                            dealTrueDmg,
                            _locomotionSet.CurrentAttackData.knockbackForce,
                            false);

                        if (oneshotSettings.UseParticleCollision)
                            damageDealer.SetupParicleDamageDealer(gameObject);
                    }
                    if (oneshotVFX.TryGetComponent<EffectApplier>(out var effectApplier))
                    {
                        effectApplier.SetUpForParticle(gameObject);
                    }

                    oneshotVFX.InitializeVFX(_locomotionSet.CurrentAttackData.size, oneshotSettings.DefaultLifeTime);
                }
                break;
            }
        }
    }
    private void TrySaveMousePosition()
    {
        if (!TryGetMouseWorldPosition(out Vector3 mouseWorld)) return;
        _savedMousePosition = GetGroundPosition(mouseWorld);
    }
    private Vector3 GetGroundPosition(Vector3 worldPos)
    {
        if (Physics.Raycast(worldPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("Ground")))
            return hit.point + Vector3.up * 0.01f;
        return worldPos + Vector3.up * 0.01f;
    }

    private bool TryGetMouseWorldPosition(out Vector3 worldPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            worldPos = hit.point;
            return true;
        }
        worldPos = Vector3.zero;
        return false;
    }


    public void OnSkillDone()
    {
        _context.IsInSpecialMove = false;
        _context.IsAiming = false;
        _context.CastingSkill = -1;
        _context.IsDashing = false; 
        _layerIgnoreController.ResetLayerIgnore();
    }
    public void OnAttackDone()
    {
        if (!_context.IsNextAttackQueued)
        {
            _context.IsAttacking = false;
            return;
        }
        // execute cai queued combo
        _context.IsNextAttackQueued = false;
        _animator.Play(_locomotionSet.QueuedAttackData.animName, _context.IsRangeClass ? _context.UpperBodyLayerIndex : 0, 0);
    }
    public void OnHurtDone()
    {
        _context.IsHurting = false;
    }
    public bool GetIsHurting()
    {
        return _context.IsHurting;
    }
    // must have to reset physicalAttack cycle when physicalAttack animation exits maybe by other state entering
    public void OnAttackAnimExit()
    {
        // if marked for exit state
        if (_context.IsAttacking)
            return;
        _locomotionSet.ResetAttackAnimCycle();
    }
    public void OnSpecialMoveAnimExit()
    {
        _executor.ClearSkillData();
    }

    public void OnParryTrigger()
    {
        IsParrying = true;
    }
    public void OnParryEnd()
    {
        IsParrying = false;
    }
    #endregion

    #region Outside Calls
    public void OnTakeDamage(GameObject sender, float currentHealth, Vector3 knockBackDirection, float knockBackForce)
    {
        _context.IsHurting = true;
        _context.KnockBackDirection = knockBackDirection;
        _context.KnockbackForce = knockBackForce;
        CameraShaker.Instance.ShakeRandomDirection(force: 0.5f);

        OnSkillDone();
        IsParrying = false;

        _locomotionSet.ResetAttackAnimCycle();
    }
    public void OnDeath()
    {
        _context.IsDead = true;
        EventBus<TopDownEndGameEvent>.Raise(new TopDownEndGameEvent(UIEndGameExecuteState.Lose));
    }
    public void OnEndGame(TopDownEndGameEvent topDownEndGameEvent)
    {
        if (topDownEndGameEvent.endGameExecuteState == UIEndGameExecuteState.Win)
        {
            _context.IsVictory = true;
            _layerIgnoreController.IgnoreAllExceptGround();
        }
    }
    public void Despawn()
    {
        _context.IsDespawning = true;
    }
    public void SetBaseSpeed(float newSpeed)
    {
        _context.BaseMoveSpeed = newSpeed;
    }
    #endregion
}



