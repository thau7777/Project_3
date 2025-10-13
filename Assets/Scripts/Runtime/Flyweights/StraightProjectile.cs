using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class StraightProjectile : Flyweight
{
    new SlashVFXSetting settings => (SlashVFXSetting)base.settings;

    private Vector3? _direction;
    private Rigidbody _rb;
    public float Speed {  get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(DespawnAfterDelay(settings.despawnDelay));
    }

    public void Initialize(Vector3 direction)
    {
        _direction = direction.normalized;

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

        // Constant velocity movement
        _rb.linearVelocity = _direction.Value * Speed;
    }
     
    private void OnCollisionEnter(Collision collision)
    {
        // Optionally handle hit effects here
        FlyweightFactory.ReturnToPool(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        FlyweightFactory.ReturnToPool(this);
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return Helpers.GetWaitForSeconds(delay);
        FlyweightFactory.ReturnToPool(this);
    }
}
