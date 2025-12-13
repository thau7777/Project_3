using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HitBoxHandler : MonoBehaviour
{
    [field: SerializeField]
    public GameObject Origin { get; set; }

    [field: SerializeField]
    public LayerMask DodgeLayers { get; set; }

    public float VFXLifeTime { get; set; }
    public Vector2 HitboxOnOffTime { get; set; } // normalized 0-1 values

    public UnityEvent<GameObject, GameObject> OnColliderHit = new();

    private Collider _collider;
    private Coroutine _hitboxTimingRoutine;

    private void Awake()
    {
        Origin = transform.gameObject;
        _collider = GetComponent<Collider>();

        if (_collider != null)
            _collider.enabled = false; // Start disabled
    }

    private void OnEnable()
    {
        if (_collider != null)
            _collider.enabled = false;

        if (_hitboxTimingRoutine != null)
            StopCoroutine(_hitboxTimingRoutine);

        _hitboxTimingRoutine = StartCoroutine(HitboxTimingRoutine());
    }

    private void OnDisable()
    {
        if (_hitboxTimingRoutine != null)
        {
            StopCoroutine(_hitboxTimingRoutine);
            _hitboxTimingRoutine = null;
        }

        if (_collider != null)
            _collider.enabled = false;
    }

    private IEnumerator HitboxTimingRoutine()
    {
        // Calculate actual times from normalized values
        float hitboxOnTime = HitboxOnOffTime.x * VFXLifeTime;
        float hitboxOffTime = HitboxOnOffTime.y * VFXLifeTime;

        // Wait until it's time to enable
        if (hitboxOnTime > 0f)
            yield return new WaitForSeconds(hitboxOnTime);

        // Enable hitbox
        if (_collider != null)
            _collider.enabled = true;

        // Wait for the duration it should be active
        float activeDuration = hitboxOffTime - hitboxOnTime;
        if (activeDuration > 0f)
            yield return new WaitForSeconds(activeDuration);

        // Disable hitbox
        if (_collider != null)
            _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != Origin && (DodgeLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            OnColliderHit?.Invoke(Origin, other.gameObject);
        }
    }
}