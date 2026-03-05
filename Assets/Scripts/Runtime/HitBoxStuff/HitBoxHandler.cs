using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HitBoxHandler : MonoBehaviour
{
    private GameObject _sender;
    private LayerMask _dodgeLayers;
    private Vector2 _hitboxOnOffTime;
    private bool _useTriggerStays;
    private float _triggerStayTickInterval;
    private bool _parryAble;
    public bool ParryAble => _parryAble;
    private const float PulseOnDuration = 0.1f;

    public UnityEvent<GameObject, GameObject, GameObject> OnColliderHit = new();

    private Collider _collider;
    private Coroutine _hitboxTimingRoutine;
    private Coroutine _pulseRoutine;

    private void Awake()
    {
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

    public void Setup(GameObject sender, LayerMask dodgeLayer, Vector2 hitboxOnOffTime, bool useTriggerStays, float triggerStayTickInterval, bool parryAble)
    {
        _sender = sender;
        _dodgeLayers = dodgeLayer;
        _hitboxOnOffTime = hitboxOnOffTime;
        _useTriggerStays = useTriggerStays;
        _triggerStayTickInterval = triggerStayTickInterval;
        _parryAble = parryAble;
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
        float hitboxOnTime = _hitboxOnOffTime.x * lifeTime;
        float hitboxOffTime = _hitboxOnOffTime.y * lifeTime;
        float activeDuration = hitboxOffTime - hitboxOnTime;

        if (hitboxOnTime > 0f)
            yield return new WaitForSeconds(hitboxOnTime);

        if (_useTriggerStays)
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
            float offTime = _triggerStayTickInterval - PulseOnDuration;
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
        if (other.gameObject != _sender && (_dodgeLayers.value & (1 << other.gameObject.layer)) == 0 && other.gameObject == other.transform.root.gameObject)
        {
            OnColliderHit?.Invoke(_sender, gameObject, other.gameObject);
        }
    }

}