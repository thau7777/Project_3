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
    private float _damage;
    private float _knockBackForce;
    private bool _dealTrueDamage;
    private float _currentSize;
    private ElementalType _elementalType;

    private const float MaxHeight = 1.35f;
    private const float DescentSpeed = 2f; // how fast it moves down when above height

    private void Awake()
    {
        _sender = transform.root.gameObject;
        _rb = gameObject.GetOrAdd<Rigidbody>();
        _direction = null;
        _rb.useGravity = false;
    }

    private void OnEnable()
    {
        _traveledDistance = 0f;
    }

    private void OnDisable()
    {
        _direction = null;
    }

    public void InitializeProjectile(GameObject sender, Vector3 direction, float speed, float range, float size,float damage, float knockBackForce, bool dealTrueDamage, LayerMask dodgeLayers, ElementalType elementalType = ElementalType.Normal)
    {
        _sender = sender;
        _direction = direction.normalized;
        _speed = speed;
        _range = range;
        _startPosition = transform.position;
        _traveledDistance = 0f;
        _currentSize = size;

        transform.localScale = new Vector3(size, size, size);

        _damage = damage;
        _knockBackForce = knockBackForce;
        _dealTrueDamage = dealTrueDamage;
        _dodgeLayers = dodgeLayers;
        _elementalType = elementalType;
    }

    private void FixedUpdate()
    {
        if (_direction == null || _rb == null)
        {
            _rb.Stop();
            return;
        }

        // Base velocity
        Vector3 velocity = _direction.Value * _speed;

        // Adjust height if needed
        float currentY = transform.position.y;
        if (currentY > MaxHeight)
        {
            float descent = Mathf.Min((currentY - MaxHeight), DescentSpeed * Time.fixedDeltaTime);
            velocity.y -= descent * (1f / Time.fixedDeltaTime);
        }

        // Apply velocity
        _rb.linearVelocity = velocity;

        // Track distance traveled
        _traveledDistance = Vector3.Distance(_startPosition, transform.position);

        // Check if reached max range
        if (_traveledDistance >= _range)
        {
            DespawnFlyweight();
        }
    }

    public void DespawnFlyweight()
    {
        SpawnHitVFX();
        FlyweightFactory.ReturnToPool(this);
    }

    private void SpawnHitVFX()
    {
        var projectileImpactFlyweight = FlyweightFactory.Spawn(settings.ProjectileImpactVFX);
        projectileImpactFlyweight.FlyweightInitialize(transform.position, Quaternion.identity);

        var impactVFX = projectileImpactFlyweight as OneShotVFX;
        OneShotVFXSettings impactVFXSettings = impactVFX.settings as OneShotVFXSettings;

        if (impactVFX.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.Setup(
                _sender, 
                _dodgeLayers,
                impactVFXSettings.hitboxOnOffTime,
                impactVFXSettings.useTriggerStays, 
                impactVFXSettings.triggerStayTickInterval, 
                false);
        }
        if (impactVFX.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            damageDealer.Setup(_damage, _dealTrueDamage, _knockBackForce, false, _elementalType);
        }

        impactVFX.InitializeVFX(_currentSize, impactVFXSettings.DefaultLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_dodgeLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            if (other.TryGetComponent<Damageable>(out var damageable) && (damageable.CurrentHealth == 0)) return;
            DespawnFlyweight();
        }
    }
}
