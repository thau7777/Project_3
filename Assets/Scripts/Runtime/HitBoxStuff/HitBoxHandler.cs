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
    public Vector2 HitboxOnOffTime { get; set; }
    public bool UseTriggerStays { get; set; }
    public float TriggerStayTickInterval { get; set; } = 0.2f;
    public UnityEvent<GameObject, GameObject> OnColliderHit = new();
    public bool Parryable { get; set; } = true;

    private const float PulseOnDuration = 0.1f;

    private Collider _collider;
    private Coroutine _hitboxTimingRoutine;
    private Coroutine _pulseRoutine;

    private void Awake()
    {
        Origin = transform.root.gameObject;
        _collider = GetComponent<Collider>();
        if (_collider != null)
            _collider.enabled = false;
    }

    private void OnEnable()
    {
        if (_collider != null)
            _collider.enabled = false;
        StopAllHitboxCoroutines();
    }

    private void OnDisable()
    {
        StopAllHitboxCoroutines();
        if (_collider != null)
            _collider.enabled = false;
    }

    private void StopAllHitboxCoroutines()
    {
        if (_hitboxTimingRoutine != null)
        {
            StopCoroutine(_hitboxTimingRoutine);
            _hitboxTimingRoutine = null;
        }
        StopPulse();
    }

    private void StopPulse()
    {
        if (_pulseRoutine != null)
        {
            StopCoroutine(_pulseRoutine);
            _pulseRoutine = null;
        }
    }

    public void StartHitBoxCoroutine(float lifeTime)
    {
        _hitboxTimingRoutine = StartCoroutine(HitboxTimingRoutine(lifeTime));
    }

    private IEnumerator HitboxTimingRoutine(float lifeTime)
    {
        float hitboxOnTime = HitboxOnOffTime.x * lifeTime;
        float hitboxOffTime = HitboxOnOffTime.y * lifeTime;
        float activeDuration = hitboxOffTime - hitboxOnTime;

        if (hitboxOnTime > 0f)
            yield return new WaitForSeconds(hitboxOnTime);

        if (UseTriggerStays)
        {
            // Pulse the collider on/off for the entire active duration
            _pulseRoutine = StartCoroutine(PulseRoutine(activeDuration));
            yield return _pulseRoutine;
        }
        else
        {
            if (_collider != null)
                _collider.enabled = true;

            if (activeDuration > 0f)
                yield return new WaitForSeconds(activeDuration);
        }

        if (_collider != null)
            _collider.enabled = false;
    }

    /// <summary>
    /// Repeatedly enables the collider for PulseOnDuration, then disables it,
    /// waiting TriggerStayTickInterval between each pulse start.
    /// </summary>
    private IEnumerator PulseRoutine(float totalDuration)
    {
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            // Turn on
            if (_collider != null)
                _collider.enabled = true;

            float onTime = Mathf.Min(PulseOnDuration, totalDuration - elapsed);
            yield return new WaitForSeconds(onTime);
            elapsed += onTime;

            // Turn off
            if (_collider != null)
                _collider.enabled = false;

            // Wait for the remainder of the tick interval before next pulse
            float offTime = TriggerStayTickInterval - PulseOnDuration;
            if (offTime > 0f && elapsed < totalDuration)
            {
                float waitTime = Mathf.Min(offTime, totalDuration - elapsed);
                yield return new WaitForSeconds(waitTime);
                elapsed += waitTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != Origin && (DodgeLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            OnColliderHit?.Invoke(Origin, other.gameObject);
        }
    }

}