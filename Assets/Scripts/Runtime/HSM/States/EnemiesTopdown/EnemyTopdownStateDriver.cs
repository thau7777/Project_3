using HSM;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Turnbase;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public struct EnemyAttackData
{
    public int index;
    public OneShotVFXSettings vfx;
    public float size;
    public float duration;
    public Vector3 offset;
}
public class EnemyTopdownStateDriver : Flyweight
{
    #region References
    private Animator _animator;
    private CharacterController _characterController;

    [SerializeField]
    private OneShotVFXSettings _stunVfxSettings;
    [SerializeField]
    private Transform _stunVfxSpawnTransform;
    #endregion

    #region Variables
    //public EnemyTopDownSettings.ElementalType elementType;
    [SerializeField]
    EnemyTopdownKind _enemyKind = EnemyTopdownKind.Normal;

    [SerializeField, FoldoutGroup("Movements")]
    float _moveSpeed = 2f;
    [SerializeField, FoldoutGroup("Movements")]
    [ShowIfEnumValue("_enemyType", EnemyTopdownKind.Slime)]
    float _movePauseDuration = 1f;
    [SerializeField, FoldoutGroup("Movements")]
    float _rotateSpeed = 10f;

    [SerializeField]
    float _attackRange = 1f;

    // Boss stuff
    [SerializeField]
    private bool _isBoss;
    public bool IsBoss => _isBoss;
    [ShowIf("_isBoss")]
    [SerializeField]
    private List<EnemySpecialMoveData> _bossSkillList = new();


    // Hide in inspector stuffs
    private SkillIndicator _skillIndicator;
    private Flyweight _chargeEffect;

    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }

    // Normal Attack Stuff
    [SerializeField]
    [ShowIf("_isBoss", true)]
    private List<EnemyAttackData> _attackList = new();
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
            .SetEnemyType(_enemyKind)
            .SetIsBoss(_isBoss)
            .SetSpecialMoveList(_bossSkillList)
            .Build();

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
    public void OnSkillTrigger()
    {
        OneShotVFXSettings vfxSettings = _context.EnemySpecialMoveData.skillEffect;
        OneShotVFX vfx = FlyweightFactory.Spawn(vfxSettings) as OneShotVFX;

        vfx.FlyweightInitialize(transform.position.Add(y:0.01f));
        vfx.InitializeVFX(vfxSettings.DefaultSize, vfxSettings.DefaultLifeTime);
    }
    public void OnAttackTrigger(int index)
    {
        EnemyAttackData? attackData = null;
        foreach (var AD in _attackList)
        {
            if (AD.index == index)
            {
                attackData = AD;
                break;
            }

        }
        if (attackData == null) return;
        OneShotVFX vfx = FlyweightFactory.Spawn(attackData.Value.vfx) as OneShotVFX;

        vfx.FlyweightInitialize(
            transform.AddLocal(attackData.Value.offset.x, attackData.Value.offset.y, attackData.Value.offset.z),
            transform.rotation);
        vfx.InitializeVFX(attackData.Value.size, attackData.Value.duration);
    }

    public void SpawnChargeEffect()
    {
        var chargeSettings = _context.EnemySpecialMoveData.chargeEffect;
        OneShotVFX vfx = FlyweightFactory.Spawn(chargeSettings) as OneShotVFX;

        vfx.FlyweightInitialize(transform.position.Add(y: 0.01f));
        vfx.InitializeVFX(chargeSettings.DefaultSize, _context.EnemySpecialMoveData.chargeDuration);

        //_chargeEffect = FlyweightFactory.Spawn(_context.EnemySpecialMoveData.chargeEffect);
        //_chargeEffect.FlyweightInitialize(transform.position);

    }
    public void ShowSkillIndicator()
    {
        var choseVFXSetting = _context.EnemySpecialMoveData.indicator;
        var indicatorFlyweight = FlyweightFactory.Spawn(_context.EnemySpecialMoveData.indicator);
        _skillIndicator = indicatorFlyweight as SkillIndicator;

        if (indicatorFlyweight is FollowedIndicator)
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
        if (!_skillIndicator) return;
        _skillIndicator.LockIndicator(duration);
    }

    public void StopMoving()
    {
        _context.CurrentSpeed = 0;
    }
    public void StartMoving()
    {
        _context.CurrentSpeed = _context.IsInSpecialMove ? _context.EnemySpecialMoveData.movementSpeed : _context.BaseMoveSpeed;
    }
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
