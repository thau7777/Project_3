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
    private Transform _player;

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

    private Vector3 PickRandomLocationAroundPlayer()
    {
        int groundLayer = LayerMask.GetMask("Ground");
        int allLayers = Physics.AllLayers;

        const int maxAttempts = 30;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            float potentialX = _player.position.x + randomCircle.x;
            float potentialZ = _player.position.z + randomCircle.y;

            Vector3 rayOrigin = new Vector3(potentialX, _raycastHeight, potentialZ);

            // Cast against ALL layers to find whatever is hit first
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, allLayers))
            {
                // Only valid if the FIRST thing hit is on the Ground layer
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    Vector3 spawnPosition = new Vector3(potentialX, hit.point.y - _spawnOffsetBelowGround, potentialZ);
                    return spawnPosition;
                }
                // If something else (building, object, etc.) was hit first, skip this point
            }
        }

        // Fallback: spawn directly at player position using a raycast straight down
        Debug.LogWarning($"Could not find valid ground-only position after {maxAttempts} attempts. Falling back to player position.");
        Vector3 fallbackOrigin = new Vector3(_player.position.x, _raycastHeight, _player.position.z);
        if (Physics.Raycast(fallbackOrigin, Vector3.down, out RaycastHit fallbackHit, _raycastHeight * 2f, groundLayer))
        {
            return new Vector3(_player.position.x, fallbackHit.point.y - _spawnOffsetBelowGround, _player.position.z);
        }

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