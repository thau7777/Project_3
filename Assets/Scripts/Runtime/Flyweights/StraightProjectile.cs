using System.Collections;
using UnityEngine;

public class StraightProjectile : Flyweight
{
    new StraightProjectileSettings settings => (StraightProjectileSettings)base.settings;

    private Vector3? _direction = null;
    private Rigidbody _rb;
    private float _speed;
    private float _range;
    private float _traveledDistance = 0f;
    private Vector3 _startPosition;
    private Vector3 _ogScale;

    public Vector3? projectileImpactScale;

    private const float MaxHeight = 1.35f;
    private const float DescentSpeed = 2f; // how fast it moves down when above height

    private void Awake()
    {
        _rb = gameObject.GetOrAdd<Rigidbody>();
        _direction = null;
        _rb.useGravity = false;
        _ogScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = _ogScale;
        projectileImpactScale = null;
        _traveledDistance = 0f;
    }

    private void OnDisable()
    {
        _direction = null;
    }

    public void InitializeProjectile(Vector3 direction, float speed, float range)
    {

        _direction = direction.normalized;
        _speed = speed;
        _range = range;
        _startPosition = transform.position;
        _traveledDistance = 0f;
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

        if (projectileImpactScale != null)
            projectileImpactFlyweight.transform.localScale = projectileImpactScale.Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((settings.DodgeLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            if (other.TryGetComponent<Damageable>(out var damageable) && (damageable.CurrentHealth == 0)) return;
            DespawnFlyweight();
        }
    }
}
