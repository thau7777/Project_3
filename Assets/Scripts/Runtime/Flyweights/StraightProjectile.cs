using Ami.BroAudio;
using System.Collections;
using UnityEngine;

public class StraightProjectile : Flyweight
{
    new StraightProjectileSettings settings => (StraightProjectileSettings)base.settings;

    private GameObject _sender;
    private LayerMask _dodgeLayers;
    private Vector3? _direction = null;
    private Rigidbody _rb;

    private float _speed;
    private float _range;
    private float _traveledDistance = 0f;
    private Vector3 _startPosition;
    private int _damage;
    private float _knockBackForce;
    private bool _dealTrueDamage;
    private float _currentSize;

    private DamageDealer _damageDealer;
    private EffectApplier _effectApplier;

    private const float MaxHeight = 1.35f;
    private const float DescentSpeed = 2f;
    private const float StoppedSpeedThreshold = 0.01f;

    private float _lifeTimeElapsed = 0f;
    private bool _despawnScheduled = false;
    private bool _canCurrentlyDealDamage = true;

    private void Awake()
    {
        _sender = transform.root.gameObject;
        _rb = gameObject.GetOrAdd<Rigidbody>();
        _rb.useGravity = false;
    }

    private void OnEnable()
    {
        _traveledDistance = 0f;
        _lifeTimeElapsed = 0f;
        _despawnScheduled = false;
        _canCurrentlyDealDamage = true;
        _direction = null;
    }

    private void OnDisable()
    {
        _direction = null;
        StopAllCoroutines();
    }

    public void InitializeProjectile(GameObject sender,
        Vector3 direction, float speed, float range,
        float size, int damage, float knockBackForce,
        bool dealTrueDamage, LayerMask dodgeLayers)
    {
        _sender = sender;
        _direction = direction.normalized;
        _speed = speed;
        _range = range;
        _startPosition = transform.position;
        _currentSize = size;

        transform.localScale = new Vector3(size, size, size);
        _damage = damage;
        _knockBackForce = knockBackForce;
        _dealTrueDamage = dealTrueDamage;
        _dodgeLayers = dodgeLayers;

        if (settings.canDealDamageByProjectile)
        {
            _damageDealer = gameObject.GetOrAdd<DamageDealer>();
            _damageDealer.Setup(true, _damage, _dealTrueDamage, _knockBackForce, false, settings.projectileDamageElementType);

            if (settings.canApplyEffects && settings.effectsToApplyList.Count > 0)
            {
                _effectApplier = gameObject.GetOrAdd<EffectApplier>();
                _effectApplier.SetEffects(settings.effectsToApplyList);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_direction == null || _rb == null)
        {
            _rb.Stop();
            return;
        }

        _lifeTimeElapsed += Time.fixedDeltaTime;

        float currentSpeed = ComputeCurrentSpeed();

        // Gate damage when curve brings speed to zero
        if (settings.useSpeedCurve && !settings.canDealDamageWhileStopping)
            _canCurrentlyDealDamage = currentSpeed > StoppedSpeedThreshold;

        Vector3 velocity = _direction.Value * currentSpeed;
        AdjustHeightVelocity(ref velocity);
        _rb.linearVelocity = velocity;

        if (settings.useLifeTime)
        {
            // Lifetime path — ignore distance, despawn when time is up
            if (_lifeTimeElapsed >= settings.lifeTime && !_despawnScheduled)
            {
                _despawnScheduled = true;
                _rb.linearVelocity = Vector3.zero;
                StartCoroutine(DespawnAfterDelay(settings.delayDurationToDespawn));
            }
        }
        else
        {
            // Distance path — despawn when range is reached
            _traveledDistance = Vector3.Distance(_startPosition, transform.position);
            if (_traveledDistance >= _range)
                DespawnProjectile();
        }
    }

    private float ComputeCurrentSpeed()
    {
        if (!settings.useLifeTime || !settings.useSpeedCurve)
            return _speed;

        float normalizedTime = Mathf.Clamp01(_lifeTimeElapsed / settings.lifeTime);
        float curveValue = settings.speedCurve.Evaluate(normalizedTime);
        return _speed * curveValue;
    }

    private void AdjustHeightVelocity(ref Vector3 velocity)
    {
        float currentY = transform.position.y;
        if (currentY > MaxHeight)
        {
            float descent = Mathf.Min((currentY - MaxHeight), DescentSpeed * Time.fixedDeltaTime);
            velocity.y -= descent * (1f / Time.fixedDeltaTime);
        }
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        DespawnProjectile();
    }

    public void DespawnProjectile()
    {
        SpawnHitVFX();
        ReturnToPool();
    }

    private void SpawnHitVFX()
    {
        if (!settings.ProjectileImpactVFX) return;

        var projectileImpactFlyweight = FlyweightFactory.Spawn(settings.ProjectileImpactVFX);
        projectileImpactFlyweight.FlyweightInitialize(transform.position, Quaternion.identity);

        var impactVFX = projectileImpactFlyweight as OneShotVFX;
        OneShotVFXSettings impactVFXSettings = impactVFX.settings as OneShotVFXSettings;

        if (impactVFX.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.Setup(_sender, _dodgeLayers,
                impactVFXSettings.hitboxOnOffTime,
                impactVFXSettings.useTriggerStays,
                impactVFXSettings.triggerStayTickInterval,
                false);
        }
        if (impactVFX.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            damageDealer.Setup(impactVFXSettings.isMagicAttack, _damage, _dealTrueDamage,
                _knockBackForce, false, impactVFXSettings.elementalType);
            if (impactVFXSettings.UseParticleCollision)
                damageDealer.SetupParicleDamageDealer(_sender);
        }
        if (impactVFX.TryGetComponent<EffectApplier>(out var effectApplier))
            effectApplier.SetUpForParticle(_sender);

        impactVFX.InitializeVFX(_currentSize, impactVFXSettings.DefaultLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_dodgeLayers.value & (1 << other.gameObject.layer)) != 0) return;
        if (settings.despawnOnHit) DespawnProjectile();

        if (!other.TryGetComponent<Damageable>(out var damageable)) return;
        if (damageable.CurrentHealth == 0) return;
        if (!_canCurrentlyDealDamage) return;

        if (settings.canDealDamageByProjectile)
            _damageDealer.DealDamage(_sender, gameObject, damageable.gameObject);

        if (settings.canApplyEffects && settings.effectsToApplyList.Count > 0)
            _effectApplier.ApplyEffect(_sender, gameObject, damageable.gameObject);

    }
}