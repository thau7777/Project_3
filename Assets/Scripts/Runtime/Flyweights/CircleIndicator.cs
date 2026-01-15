using UnityEngine;

public class CircleIndicator : SkillIndicator
{
    [Header("Indicator Settings")]
    private Transform _followTarget;
    private Transform _originPoint; // The point to measure max range from (usually the player)
    bool _followMouse = false;
    private float _maxRange = Mathf.Infinity;

    private void Awake()
    {
        _originPoint = GameObject.FindWithTag("Player").transform;
    }
    public void Initialize(float radius,float range, Transform followTarget = null)
    {
        _vfx.SetFloat("Size", radius);
        _followTarget = followTarget;
        _maxRange = range;
        // if follow target = null then follow mouse position
        _followMouse = followTarget == null;
    }

    private void Update()
    {
        if (isMovementLocked) return;

        if (_followMouse)
            MoveToMouse();
        else
            MoveTowardTarget();
    }

    protected void MoveToMouse()
    {
        if (!TryGetMouseWorldPosition(out Vector3 mouseWorld)) return;

        Vector3 groundMousePos = GetGroundPosition(mouseWorld);

        // If no origin point set, just move to mouse (no clamping)
        if (_originPoint == null || _maxRange == Mathf.Infinity)
        {
            transform.position = groundMousePos;
            return;
        }

        // Get origin position on ground (ignore Y for top-down)
        Vector3 originPos = _originPoint.position;
        originPos.y = groundMousePos.y; // Match Y level for proper distance calculation

        // Calculate direction and distance from origin to mouse
        Vector3 directionToMouse = groundMousePos - originPos;
        float distanceToMouse = directionToMouse.magnitude;

        // If mouse is within range, move indicator to mouse position
        if (distanceToMouse <= _maxRange)
        {
            transform.position = groundMousePos;
        }
        else
        {
            // Clamp indicator to max range in the direction of mouse
            Vector3 clampedPosition = originPos + directionToMouse.normalized * _maxRange;
            transform.position = clampedPosition;
        }
    }

    private void MoveTowardTarget()
    {
        if (!_followTarget && !_followMouse)
        {
            ReturnToPool();
            return;
        }

        // Instant snap to target position - no lerp/interpolation
        transform.position = _followTarget.position;
    }
}