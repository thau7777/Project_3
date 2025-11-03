using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SkillIndicator : Flyweight
{
    new SkillIndicatorSettings settings => (SkillIndicatorSettings)base.settings;

    [SerializeField] private float moveSpeed = 5f; // Only used for circle indicator
    [SerializeField] private float groundOffset = 0.01f; // small lift to avoid z-fighting

    private Transform target;
    private bool followMouse;
    private VisualEffect _vfx;
    private Coroutine _lockInCoroutine;

    public FlyweightSettings Skill { get; set; }

    private void Awake()
    {
        _vfx = GetComponent<VisualEffect>();
    }

    private void OnEnable()
    {
        _vfx.SetFloat("LockInTime", 0);
    }

    private void OnDisable()
    {
        if (_lockInCoroutine != null)
            StopCoroutine(_lockInCoroutine);

        transform.SetParent(null);
        moveSpeed = 5f;
        Stop();
    }

    public void Initialize(float speed, Transform target = null)
    {
        if (settings.isCircle)
        {
            moveSpeed = speed;
            if (target) SetFollowTarget(target);
            else SetFollowMouse();
            return;
        }

        if (target)
        {
            transform.SetParent(target);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        this.target = target;
        followMouse = false;
    }

    public void SetFollowMouse()
    {
        target = null;
        followMouse = true;
    }

    private void Update()
    {
        if (!settings.isCircle)
        {
            RotateToMouse();
            return;
        }

        MoveTowardTargetOrMouse();
    }

    private void RotateToMouse()
    {
        // Get mouse world position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Calculate direction ignoring Y (only rotate on horizontal plane)
        Vector3 lookDir = worldMousePos - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    private void MoveTowardTargetOrMouse()
    {
        Vector3 desiredPos = transform.position;

        if (target != null)
        {
            desiredPos = target.position;
        }
        else if (followMouse)
        {
            // Ray from camera to mouse position
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseRay, out RaycastHit mouseHit, Mathf.Infinity, settings.groundMask))
            {
                desiredPos = mouseHit.point + Vector3.up * groundOffset;
            }
            else
            {
                // fallback: don't move if nothing hit
                return;
            }
        }
        else
        {
            return;
        }

        // Raycast down to stick precisely to ground (useful even if target is above)
        if (Physics.Raycast(desiredPos + Vector3.up * 5f, Vector3.down, out RaycastHit groundHit, 10f, settings.groundMask))
        {
            desiredPos = groundHit.point + Vector3.up * groundOffset;
        }

        // Move toward target smoothly and accurately along the ground plane
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, moveSpeed * Time.deltaTime);
    }

    private void Stop()
    {
        target = null;
        followMouse = false;
    }
    public void OnSkillUse(float lockDuration, Action skillCastMethod = null)
    {
        Stop();
        if (skillCastMethod == null)
        {
            ReturnToPool();
            return;
        }

        _vfx.SendEvent("Lock In");

        if (_lockInCoroutine != null)
            StopCoroutine(_lockInCoroutine);
        _lockInCoroutine = StartCoroutine(StartLockIn(skillCastMethod, lockDuration));
    }

    private IEnumerator StartLockIn(Action skillCastMethod, float lockDuration)
    {
        float elapsed = 0f;
        while (elapsed < lockDuration)
        {
            elapsed += Time.deltaTime;
            _vfx.SetFloat("LockInTime", elapsed);
            yield return null;
        }
        _vfx.SetFloat("LockInTime", 1);
        skillCastMethod?.Invoke();
        ReturnToPool();
    }

}
