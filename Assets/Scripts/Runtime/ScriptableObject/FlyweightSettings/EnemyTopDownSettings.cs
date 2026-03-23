using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyTopDownSettings", menuName = "Scriptable Objects/Flyweight/Enemy TopDown Settings")]
public class EnemyTopDownSettings : FlyweightSettings
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _initialHealth = 100;

    private GameObject _targetGroundObject;
    [SerializeField] private float _raycastHeight = 50f;
    [SerializeField] private float _spawnOffsetBelowGround = 2f;

    public ElementalType elementalType = ElementalType.Normal;
    public float spawnAnimationDuration = 0.5f;
    [SerializeField] private OneShotVFXSettings _spawnVFXSettings;
    [SerializeField] private LayerMask _obstacleLayerMask;
    [SerializeField] private float _spawnClearanceRadius = 0.5f;
    private Transform _player;

    private Collider _groundCollider;
    private Terrain _groundTerrain; 
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var flyweight = go.GetComponent<EnemyTopdownStateDriver>();
        flyweight.settings = this;
        go.GetComponent<Damageable>().hasShieldBreakingMechanic = true;

        // temporary: add CharacterStats component and set it up with default values (can be overridden on spawn)
        var characterStats = go.GetOrAdd<CharacterStats>();
        //characterStats.Setup(elementalType, _initialHealth, 0, 10, 10, 40, 40, 5, 5, 1.5f, 1);

        go.GetOrAdd<NavMeshSteering>().target = PlayerTopDownStateDriver.Instance.transform;
        return flyweight;
    }

    public void SetupSpawnSettings(Transform player, float spawnRadius)
    {
        _player = player;
        _spawnRadius = spawnRadius;
    }


    private void CacheGroundReference()
    {
        if (_groundCollider != null || _groundTerrain != null) return;

        var groundObj = GameObject.FindWithTag("Ground");
        if (groundObj == null)
        {
            Debug.LogWarning("No GameObject with tag 'Ground' found.");
            return;
        }

        // Prefer Terrain for accurate height sampling
        _groundTerrain = groundObj.GetComponent<Terrain>();
        if (_groundTerrain == null)
            _groundCollider = groundObj.GetComponent<Collider>();
    }

    private bool TryGetGroundHeight(float x, float z, out float height)
    {
        height = 0f;

        // Terrain path — most accurate, zero physics cost
        if (_groundTerrain != null)
        {
            height = _groundTerrain.SampleHeight(new Vector3(x, 0f, z));
            // Make sure the XZ point is actually within terrain bounds
            var bounds = _groundTerrain.GetComponent<Collider>().bounds;
            return bounds.Contains(new Vector3(x, height, z));
        }

        // Collider path (MeshCollider, BoxCollider, etc.)
        if (_groundCollider != null)
        {
            Vector3 samplePoint = new Vector3(x, _raycastHeight, z);
            Vector3 closest = _groundCollider.ClosestPoint(samplePoint);

            // If closest point snapped far away in XZ, this spot is off the ground mesh
            bool isOnGround = Vector2.Distance(new Vector2(closest.x, closest.z), new Vector2(x, z)) < 0.5f;
            if (!isOnGround) return false;

            height = closest.y;
            return true;
        }

        return false;
    }

    private Vector3 PickRandomLocationAroundPlayer()
    {
        CacheGroundReference();

        const int maxAttempts = 30;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            float potentialX = _player.position.x + randomCircle.x;
            float potentialZ = _player.position.z + randomCircle.y;

            if (TryGetGroundHeight(potentialX, potentialZ, out float groundY))
            {
                Vector3 candidate = new Vector3(potentialX, groundY - _spawnOffsetBelowGround, potentialZ);

                // Reject if an obstacle is at this position
                bool blocked = Physics.CheckSphere(
                    candidate + Vector3.up * _spawnClearanceRadius,
                    _spawnClearanceRadius,
                    _obstacleLayerMask
                );

                if (!blocked)
                    return candidate;
            }
        }

        // Fallback to directly under player
        Debug.LogWarning($"Could not find valid ground position after {maxAttempts} attempts. Falling back to player position.");
        if (TryGetGroundHeight(_player.position.x, _player.position.z, out float fallbackY))
            return new Vector3(_player.position.x, fallbackY - _spawnOffsetBelowGround, _player.position.z);

        return _player.position;
    }

    public override void OnGet(Flyweight f)
    {
        base.OnGet(f);
        // Cache the spawn position
        Vector3 spawnPosition = PickRandomLocationAroundPlayer();
        f.GetComponent<CharacterController>().enabled = false;
        f.transform.position = spawnPosition; // Set position AFTER activating
        // Spawn VFX at ground level
        if (_spawnVFXSettings != null)
        {
            var vfx = FlyweightFactory.Spawn(_spawnVFXSettings);
            vfx.FlyweightInitialize(new Vector3(spawnPosition.x, spawnPosition.y + _spawnOffsetBelowGround + 0.1f, spawnPosition.z));
            (vfx as OneShotVFX).InitializeVFX(_spawnVFXSettings.DefaultSize, _spawnVFXSettings.DefaultLifeTime);
        }

        DelayEnemySpawn(f, spawnPosition).Forget();
    }

    private async UniTask DelayEnemySpawn(Flyweight f, Vector3 spawnPosition)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.6f));

        f.gameObject.SetActive(true);
        f.transform.position = spawnPosition; // Set position AFTER activating
        f.GetComponent<EnemyTopdownStateDriver>().StartSpawnAnim();
    }
    public override void OnRelease(Flyweight f)
    {
        base.OnRelease(f);
        f.GetComponent<EnemyTopdownStateDriver>().ResetStateContext();
    }
}