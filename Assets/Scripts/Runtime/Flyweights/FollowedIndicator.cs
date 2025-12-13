using UnityEngine;
public class FollowedIndicator : SkillIndicator
{
    [SerializeField]
    private Transform _user;
    private Transform _target;
    private bool rotateTowardMouse = false; // toggle between target or mouse

    public void Initialize(Transform user, float width, float length, Transform target = null)
    {
        _user = user;
        _target = target;
        _vfx.SetFloat("Width", width);
        _vfx.SetFloat("Length", length);
        rotateTowardMouse = target == null;
    }

    private void Update()
    {
        if (!_user)
        {
            ReturnToPool();
            return;
        }
        if (isMovementLocked)
            return;

        // Follow user position
        transform.position = _user.position;

        // Rotate toward target or mouse
        if (rotateTowardMouse)
            RotateTowardMouse();
        else if (_target)
            RotateTowardTarget();
    }

    private void RotateTowardTarget()
    {
        Vector3 direction = _target.position - _user.position;
        direction.y = 0f; // keep horizontal rotation only

        if (direction.sqrMagnitude < 0.0001f) return;

        // Instant rotation - no lerp/slerp
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void RotateTowardMouse()
    {
        if (!TryGetMouseWorldPosition(out Vector3 mousePos))
            return;

        Vector3 direction = mousePos - _user.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f) return;

        // Instant rotation - no lerp/slerp
        transform.rotation = Quaternion.LookRotation(direction);
    }
}