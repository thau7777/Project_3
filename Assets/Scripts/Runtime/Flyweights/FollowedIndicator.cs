using UnityEngine;
public class FollowedIndicator : SkillIndicator
{
    [SerializeField]
    private Transform _user;
    private bool rotateTowardMouse = false; // toggle between target or mouse

    public void Initialize(Transform user, float width, float length)
    {
        bool isPlayerUser = user.gameObject.layer == LayerMask.NameToLayer("Player");
        if (!isPlayerUser)
            transform.SetParent(user);
        _user = user;
        rotateTowardMouse = isPlayerUser;
        if (_vfx.HasFloat("Width") && _vfx.HasFloat("Length"))
        {
            _vfx.SetFloat("Width", width);
            _vfx.SetFloat("Length", length);
        }else
            transform.localScale = new Vector3(width, 1f, width);
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