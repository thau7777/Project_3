using UnityEngine;

public class BeholderMultiProjectile : StraightProjectile
{
    [SerializeField] private float minSplitDistance = 3f;
    public int duplicateCount = 6;

    private float _nextSplitDistance;

    public void InitializeMultiProjectile(GameObject sender,
        Vector3 direction, float speed, float range,
        float size, int damage, float knockBackForce,
        bool dealTrueDamage, LayerMask dodgeLayers, int duplicates)
    {
        duplicateCount = duplicates;
        InitializeProjectile(sender, direction, speed, range, size, damage, knockBackForce, dealTrueDamage, dodgeLayers);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _nextSplitDistance = minSplitDistance;
    }

    protected override void OnTraveledDistance(float traveledDistance)
    {
        if (duplicateCount <= 0) return;

        if (traveledDistance >= _nextSplitDistance)
        {
            _nextSplitDistance += minSplitDistance;
            SpawnSplitProjectiles(despawnSelf: false);
        }
    }

    protected override void OnHitDespawn()
    {
        //if (duplicateCount > 0)
        //    SpawnSplitProjectiles(despawnSelf: true);
        //else
            base.OnHitDespawn();
    }

    private void SpawnSplitProjectiles(bool despawnSelf)
    {
        if (duplicateCount <= 0) return;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = new Vector3(-forward.z, 0f, forward.x);

        SpawnChild(right);
        SpawnChild(-right);

        if (despawnSelf)
            DespawnProjectile();
    }

    private void SpawnChild(Vector3 direction)
    {
        var flyweight = FlyweightFactory.Spawn(settings);
        flyweight.FlyweightInitialize(transform.position);
        if (flyweight is BeholderMultiProjectile child)
        {
            child.InitializeMultiProjectile(
                _sender,
                direction,
                _speed,
                _range,
                _currentSize,
                _damage,
                _knockBackForce,
                _dealTrueDamage,
                _dodgeLayers,
                duplicateCount - 1);
        }
    }
}