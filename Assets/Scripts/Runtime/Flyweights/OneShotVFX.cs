using System.Collections;
using UnityEngine;

public class OneShotVFX : Flyweight
{
    new OneShotVFXSettings settings => (OneShotVFXSettings)base.settings;

    private Coroutine _despawnCoroutine;
    [SerializeField] private Collider _collider;

    private float _elapsedTime = 0f;
    private float _hitboxOnTime;
    private float _hitboxOffTime;

    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>(true); // find collider automatically
        if (_collider != null)
            _collider.enabled = false; // always start disabled

    }

    private void OnEnable()
    {
        _elapsedTime = 0f;

        // Convert normalized 0–1 values into actual time thresholds
        _hitboxOnTime = settings.hitboxOnOffTime.x * settings.DespawnDelay;
        _hitboxOffTime = settings.hitboxOnOffTime.y * settings.DespawnDelay;

        if (_collider != null)
            _collider.enabled = false;

        if (_despawnCoroutine != null)
            StopCoroutine(_despawnCoroutine);

        _despawnCoroutine = StartCoroutine(LifetimeRoutine());
    }

    private void OnDisable()
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
            _despawnCoroutine = null;
        }

        if (_collider != null)
            _collider.enabled = false;
    }

    private IEnumerator LifetimeRoutine()
    {
        while (_elapsedTime < settings.DespawnDelay)
        {
            _elapsedTime += Time.deltaTime;

            if (settings.HasHitBox && _collider != null)
            {
                // enable collider when time >= hitboxOnTime
                if (!_collider.enabled && _elapsedTime >= _hitboxOnTime && _elapsedTime < _hitboxOffTime)
                    _collider.enabled = true;

                // disable collider when time >= hitboxOffTime
                else if (_collider.enabled && _elapsedTime >= _hitboxOffTime)
                    _collider.enabled = false;
            }

            yield return null;
        }

        FlyweightFactory.ReturnToPool(this);
    }
}
