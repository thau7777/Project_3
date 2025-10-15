using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class StraightProjectile : Flyweight
{
    new StraightProjectileSettings settings => (StraightProjectileSettings)base.settings;

    private Vector3? _direction = null;
    private Rigidbody _rb;
    private float _speed;

    private Coroutine _despawnRoutine;

    private void Awake()
    {
        _rb = gameObject.GetOrAdd<Rigidbody>();
        _direction = null;
        _rb.useGravity = false;
    }

    private void OnDisable()
    {
        _direction = null;
        // Safety — stop any running coroutine

        if (_despawnRoutine != null)
        {
            StopCoroutine(_despawnRoutine);
            _despawnRoutine = null;
        }
    }

    public void Initialize(Vector3 direction, float speed)
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

        _rb.linearVelocity = _direction.Value * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((settings.DodgeLayers.value & (1 << other.gameObject.layer)) != 0)
            return;

        FlyweightFactory.ReturnToPool(this);
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return Helpers.GetWaitForSeconds(delay);
        FlyweightFactory.ReturnToPool(this);
    }
}
