using UnityEngine;

public class CircleIndicator : SkillIndicator
{
    [Header("Indicator Settings")]
    [SerializeField] private float moveSpeed = 5f;   // For circle indicator smooth follow
    private Transform _followTarget;
    bool _followMouse = false;

    public void Initialize(Transform followTarget = null)
    {
        _followTarget = followTarget;
        // if follow target = null then follow mouse position
        _followMouse = followTarget == null;
    }

    private void Update()
    {
        if (isMovementLocked) return;
        if(_followMouse)
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
        if(!_followTarget && !_followMouse)
        {
            ReturnToPool();
            return;
        }
        Vector3 targetPosition = _followTarget.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

}
