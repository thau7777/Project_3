using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class StraightProjectile : Flyweight
{
    new StraightProjectileSettings settings => (StraightProjectileSettings)base.settings;

    private Vector3? _direction = null;
    private Rigidbody _rb;
    private float _speed;
    private Vector3 _ogScale;
    private Coroutine _despawnRoutine;

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
    }

    private void OnDisable()
    {
        _direction = null;

        if (_despawnRoutine != null)
        {
            StopCoroutine(_despawnRoutine);
            _despawnRoutine = null;
        }

        
    }

    public void InitializeMovement(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        _speed = speed;
        _despawnRoutine = StartCoroutine(DespawnAfterDelay(settings.DespawnDelay));
    }

    private void FixedUpdate()
    {
        if (_direction == null || _rb == null)
        {
            _rb.Stop();
            return;
        }

        // Get base movement velocity
        Vector3 velocity = _direction.Value * _speed;

        // Adjust height if needed
        float currentY = transform.position.y;
        if (currentY > MaxHeight)
        {
            float descent = Mathf.Min((currentY - MaxHeight), DescentSpeed * Time.fixedDeltaTime);
            velocity.y -= descent * (1f / Time.fixedDeltaTime);
        }

        _rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((settings.DodgeLayers.value & (1 << other.gameObject.layer)) != 0)
            return;

        FlyweightFactory.ReturnToPool(this);
        SpawnHitVFX();
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return Helpers.GetWaitForSeconds(delay);
        FlyweightFactory.ReturnToPool(this);
        SpawnHitVFX();
    }
    private void SpawnHitVFX()
    {
        var projectileImpactFlyweight = FlyweightFactory.Spawn(settings.HitVFXSettings);
        projectileImpactFlyweight.Initialize(transform.position, Quaternion.identity);
        if(projectileImpactScale != null)
            projectileImpactFlyweight.transform.localScale = projectileImpactScale.Value;
    }
}
