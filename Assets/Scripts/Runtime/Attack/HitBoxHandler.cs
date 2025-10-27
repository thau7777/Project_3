using UnityEngine;

public class HitBoxHandler : MonoBehaviour
{
    Transform _origin;
    [SerializeField]
    float _knockbackForce = 20f;
    private void Start()
    {
        _origin = transform.root;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Damageable>(out var damageable) && other != _origin)
        {
            Vector3 hitDirection = other.transform.position - _origin.position;
            damageable.TakeDamage(40, hitDirection.normalized,_knockbackForce); // Example damage value
        }
    }
}
