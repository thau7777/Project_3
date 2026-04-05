using UnityEngine;

public class FollowedIndicator : SkillIndicator
{
    [SerializeField]
    private Transform _user;
    private bool rotateTowardMouse = false;
    private bool isPlayerUser;
    private float _rotationYOffset = 0f; // stored offset for enemy users

    public void Initialize(Transform user, float width, float length, float rotationYOffset = 0f)
    {
        isPlayerUser = user.gameObject.layer == LayerMask.NameToLayer("Player");
        if (!isPlayerUser)
        {
            transform.SetParent(user);
            _rotationYOffset = rotationYOffset;
            // Apply offset immediately as local rotation — parent handles the rest
            transform.localRotation = Quaternion.Euler(0f, _rotationYOffset, 0f);
        }

        _user = user;
        rotateTowardMouse = isPlayerUser;

        if (_vfx.HasFloat("Width") && _vfx.HasFloat("Length"))
        {
            _vfx.SetFloat("Width", width);
            _vfx.SetFloat("Length", length);
        }
        else if (name == "Indicator_Cone_Blue" || name == "Indicator_Cone_Red")
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

        if (isPlayerUser)
            transform.position = _user.position.Add(y: (settings as SkillIndicatorSettings).groundOffset);

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
        transform.rotation = Quaternion.LookRotation(direction);
    }
}