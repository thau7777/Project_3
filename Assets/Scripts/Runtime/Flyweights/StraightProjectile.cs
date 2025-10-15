using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class StraightProjectile : Flyweight
{
    new StraightProjectileSettings settings => (StraightProjectileSettings)base.settings;

    private Vector3? _direction;
    private Rigidbody _rb;
    private float _speed;

    private Coroutine _despawnRoutine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _despawnRoutine = StartCoroutine(DespawnAfterDelay(settings.DespawnDelay));
    }

    private void OnDisable()
    {
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

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (_direction == null || _rb == null)
            return;

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
