using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(CapsuleCollider))]
public class HitBoxHandler : MonoBehaviour
{
    Transform _origin;
    
    [field: SerializeField]
    public LayerMask DodgeLayers { get; set; }

    public UnityEvent<GameObject> OnColliderHit;

    private void Awake()
    {
        _origin = transform.root;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != _origin && (DodgeLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            OnColliderHit?.Invoke(other.gameObject);
        }
    }
}
