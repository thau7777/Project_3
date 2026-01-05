using System.Collections;
using UnityEngine;

public class StraightProjectile : Flyweight
{
    new StraightProjectileSettings settings => (StraightProjectileSettings)base.settings;
    public LayerMask DodgeLayers { get; set; }
    private Vector3? _direction = null;
    private Rigidbody _rb;
    private float _speed;
    private float _range;
    private float _traveledDistance = 0f;
    private Vector3 _startPosition;
    public float Damage { get; set; }
    public float _currentSize;

    private const float MaxHeight = 1.35f;
    private const float DescentSpeed = 2f; // how fast it moves down when above height

    private void Awake()
    {
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

    public void InitializeProjectile(Vector3 direction, float speed, float range, float size,float damage)
    {

        _direction = direction.normalized;
        _speed = speed;
        _range = range;
        _startPosition = transform.position;
        _traveledDistance = 0f;
        _currentSize = size;

        transform.localScale = new Vector3(size, size, size);

        Damage = damage;
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
        if(projectileImpactFlyweight.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.DodgeLayers = DodgeLayers;
        }

        var impactVFX = projectileImpactFlyweight as OneShotVFX;
        OneShotVFXSettings impactVFXSettings = impactVFX.settings as OneShotVFXSettings;

        if(impactVFX.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            damageDealer.Damage = Damage;
        }

        impactVFX.InitializeVFX(_currentSize, impactVFXSettings.DefaultLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((DodgeLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            if (other.TryGetComponent<Damageable>(out var damageable) && (damageable.CurrentHealth == 0)) return;
            DespawnFlyweight();
        }
    }
}
