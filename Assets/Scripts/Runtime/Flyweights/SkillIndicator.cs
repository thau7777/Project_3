using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public abstract class SkillIndicator : Flyweight
{
    new SkillIndicatorSettings settings => (SkillIndicatorSettings)base.settings;

    [Header("Indicator Settings")]
    [SerializeField] protected float groundOffset = 0.1f;

    [Header("Debug / Control")]
    public bool isMovementLocked = false; // <--- NEW: Stops all movement & rotation when true

    protected VisualEffect _vfx;
    private Coroutine _lockInCoroutine;

    private void Awake()
    {
        _vfx = GetComponent<VisualEffect>();
    }

    private void OnEnable()
    {
        if(_vfx == null)
            _vfx = GetComponent<VisualEffect>();
        isMovementLocked = false;

        if (_vfx.HasFloat("ChargeAmount"))
            _vfx.SetFloat("ChargeAmount", 0);
    }

    private void OnDisable()
    {
        if (_lockInCoroutine != null)
            StopCoroutine(_lockInCoroutine);

    }

    // -------------------------------------------------------------------------
    // INITIALIZE
    // -------------------------------------------------------------------------
   
    
    public void Stop()
    {
        isMovementLocked = true; // <--- freeze during lock-in
    }
    protected Vector3 GetGroundPosition(Vector3 worldPos)
    {
        if (Physics.Raycast(worldPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, settings.groundMask))
            return hit.point + Vector3.up * groundOffset;
        return worldPos + Vector3.up * groundOffset;
    }

    protected bool TryGetMouseWorldPosition(out Vector3 worldPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, settings.groundMask))
        {
            worldPos = hit.point;
            return true;
        }
        worldPos = Vector3.zero;
        return false;
    }
    // -------------------------------------------------------------------------
    // SKILL LOCK-IN
    // -------------------------------------------------------------------------
    public void LockIndicator(float lockDuration)
    {
        if(!_vfx.HasFloat("ChargeAmount") || lockDuration == 0)
        {
            ReturnToPool();
            return;
        }
        if (_lockInCoroutine != null)
            StopCoroutine(_lockInCoroutine);
        _lockInCoroutine = StartCoroutine(StartLockIn(lockDuration));
    }

    private IEnumerator StartLockIn(float lockDuration)
    {
        float elapsed = 0f;
        while (elapsed < lockDuration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Clamp01(elapsed / lockDuration);
            _vfx.SetFloat("ChargeAmount", value);
            yield return null;
        }

        _vfx.SetFloat("ChargeAmount", 1);
        ReturnToPool();
    }
}
