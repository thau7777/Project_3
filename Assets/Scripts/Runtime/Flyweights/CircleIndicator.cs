using UnityEngine;
public class CircleIndicator : SkillIndicator
{
    [Header("Indicator Settings")]
    private Transform _followTarget;
    bool _followMouse = false;

    public void Initialize(float radius, Transform followTarget = null)
    {
        _vfx.SetFloat("Size", radius);
        _followTarget = followTarget;
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
        transform.position = GetGroundPosition(mouseWorld);
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