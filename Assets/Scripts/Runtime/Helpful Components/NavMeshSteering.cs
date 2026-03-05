using UnityEngine;
using UnityEngine.AI;

public class NavMeshSteering : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float pathRefreshRate = 0.3f;
    public float waypointReachedDistance = 0.4f;

    [Header("Off-Mesh Recovery")]
    public float offMeshSearchRadius = 5f;

    [Header("Separation")]
    public float separationRadius = 2f;
    public float separationStrength = 0.5f;
    public float separationOverrideDuration = 0.5f;
    public LayerMask enemyLayer;

    NavMeshPath _path;
    int _waypointIndex;
    float _pathTimer;

    Vector3 _separationOverrideDirection;
    float _separationOverrideTimer;
    bool _isSeparating => _separationOverrideTimer > 0f;

    void Awake()
    {
        _path = new NavMeshPath();
        enemyLayer = LayerMask.GetMask("EnemyTopDown");
    }

    void Update()
    {
        if (_separationOverrideTimer > 0f)
        {
            _separationOverrideTimer -= Time.deltaTime;
        }

        _pathTimer -= Time.deltaTime;
        if (_pathTimer <= 0f && target != null)
        {
            _pathTimer = pathRefreshRate;

            Vector3 destination = target.position;
            if (!IsOnNavMesh(target.position))
            {
                if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, offMeshSearchRadius, NavMesh.AllAreas))
                    destination = hit.position;
            }

            NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, _path);
            _waypointIndex = 1;
        }
    }

    bool IsOnNavMesh(Vector3 position)
    {
        return NavMesh.SamplePosition(position, out NavMeshHit hit, 0.3f, NavMesh.AllAreas)
               && Vector3.Distance(position, hit.position) < 0.3f;
    }

    void CheckSeparationTrigger()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius, enemyLayer);
        Vector3 push = Vector3.zero;
        int count = 0;

        foreach (Collider col in nearby)
        {
            if (col.gameObject == gameObject) continue;

            Vector3 away = transform.position - col.transform.position;
            away.y = 0f;

            float distance = away.magnitude;
            if (distance == 0f)
            {
                away = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                distance = 0.01f;
            }

            float weight = 1f / (distance * distance);
            push += away.normalized * weight;
            count++;
        }

        if (count == 0) return;

        // Trigger the override — lock in the opposite direction for the duration
        _separationOverrideDirection = push.normalized;
        _separationOverrideTimer = separationOverrideDuration;
    }

    public Vector3 GetDirection()
    {
        if (!IsOnNavMesh(transform.position))
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, offMeshSearchRadius, NavMesh.AllAreas))
            {
                Vector3 recovery = hit.position - transform.position;
                recovery.y = 0f;
                return recovery.normalized;
            }
            return Vector3.zero;
        }

        // Check every frame if we should trigger a new separation
        if (!_isSeparating)
            CheckSeparationTrigger();

        // If separating, blend separation with nav direction
        if (_isSeparating)
        {
            if (_path.status == NavMeshPathStatus.PathInvalid || _path.corners.Length == 0)
                return _separationOverrideDirection;

            while (_waypointIndex < _path.corners.Length)
            {
                Vector3 toWaypoint = _path.corners[_waypointIndex] - transform.position;
                toWaypoint.y = 0f;
                if (toWaypoint.magnitude <= waypointReachedDistance)
                    _waypointIndex++;
                else
                    break;
            }

            if (_waypointIndex >= _path.corners.Length)
                return _separationOverrideDirection;

            Vector3 navDir = _path.corners[_waypointIndex] - transform.position;
            navDir.y = 0f;
            return (navDir.normalized + _separationOverrideDirection * separationStrength).normalized;
        }

        // Normal pathfinding
        if (_path.status == NavMeshPathStatus.PathInvalid || _path.corners.Length == 0)
            return Vector3.zero;

        while (_waypointIndex < _path.corners.Length)
        {
            Vector3 toWaypoint = _path.corners[_waypointIndex] - transform.position;
            toWaypoint.y = 0f;
            if (toWaypoint.magnitude <= waypointReachedDistance)
                _waypointIndex++;
            else
                break;
        }

        if (_waypointIndex >= _path.corners.Length)
            return Vector3.zero;

        Vector3 dir = _path.corners[_waypointIndex] - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }

    public Vector3 GetTargetPosition()
    {
        if (!IsOnNavMesh(transform.position))
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, offMeshSearchRadius, NavMesh.AllAreas))
                return hit.position;

            return transform.position;
        }

        if (_path.status == NavMeshPathStatus.PathInvalid || _path.corners.Length == 0)
            return transform.position;

        if (_waypointIndex >= _path.corners.Length)
            return transform.position;

        return _path.corners[_waypointIndex];
    }

    void OnDrawGizmosSelected()
    {
        if (_path == null || _path.corners.Length == 0) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < _path.corners.Length - 1; i++)
            Gizmos.DrawLine(_path.corners[i], _path.corners[i + 1]);
        Gizmos.color = Color.yellow;
        if (_waypointIndex < _path.corners.Length)
            Gizmos.DrawSphere(_path.corners[_waypointIndex], 0.15f);

        // Visualize separation override
        if (_isSeparating)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _separationOverrideDirection * 2f);
        }
    }
}