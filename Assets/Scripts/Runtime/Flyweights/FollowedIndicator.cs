using UnityEngine;
public class FollowedIndicator : SkillIndicator
{
    [SerializeField]
    private Transform _user;
    private bool rotateTowardMouse = false; // toggle between target or mouse
    private bool isPlayerUser;
    public void Initialize(Transform user, float width, float length)
    {
        isPlayerUser = user.gameObject.layer == LayerMask.NameToLayer("Player");
        if (!isPlayerUser)
            transform.SetParent(user);
        _user = user;
        rotateTowardMouse = isPlayerUser;
        if (_vfx.HasFloat("Width") && _vfx.HasFloat("Length")) 
        {
            _vfx.SetFloat("Width", width);
            _vfx.SetFloat("Length", length);
        }else if(name == "Indicator_Cone_Blue" || name == "Indicator_Cone_Red")
            _vfx.SetFloat("Size", width);
        else
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
        if (isPlayerUser)
            transform.position = _user.position.Add(y:(settings as SkillIndicatorSettings).groundOffset);

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