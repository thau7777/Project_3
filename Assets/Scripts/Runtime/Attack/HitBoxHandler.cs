using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]
public class HitBoxHandler : MonoBehaviour
{
    Transform _origin;
    [SerializeField]
    float _knockbackForce = 20f;

    [field: SerializeField]
    public LayerMask DodgeLayers { get; private set; }


    [field: SerializeField]
    public OneShotVFXSettings HitImpactEffect { get; private set; }
    private void Awake()
    {
        _origin = transform.root;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != _origin && (DodgeLayers.value & (1 << other.gameObject.layer)) == 0)
        {

            if (other.TryGetComponent<Damageable>(out var damageable))
            {
                Vector3 hitDirection = other.transform.position - _origin.position;
                damageable.TakeDamage(40, hitDirection.normalized, _knockbackForce); // Example damage value

                if (HitImpactEffect)
                    FlyweightFactory.Spawn(HitImpactEffect).transform.position = other.transform.position.Add(y: 1);
            }

        }
    }
}
