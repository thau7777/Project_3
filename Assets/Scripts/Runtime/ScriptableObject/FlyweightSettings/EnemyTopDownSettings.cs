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
    public float spawnAnimationTime = 0.5f;
    [SerializeField] private OneShotVFXSettings _spawnVFXSettings;
    private Transform _player;

    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var flyweight = go.GetComponent<EnemyTopdownStateDriver>();
        flyweight.settings = this;
        go.GetComponent<Damageable>().hasShieldBreakingMechanic = true;
        return flyweight;
    }

    public void SetupSpawnSettings(Transform player, float spawnRadius)
    {
        _player = player;
        _spawnRadius = spawnRadius;
    }

    public void SetInitialHealthOnSpawn(float health)
    {
        _initialHealth = Mathf.Max(1, health);
    }


    private Vector3 PickRandomLocationAroundPlayer()
    {
        _targetGroundObject = GameObject.FindWithTag("Ground");
        Terrain terrain = _targetGroundObject.GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("Ground object doesn't have a Terrain component!");
            return _player.position;
        }

        const int maxAttempts = 30;
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Calculate random X and Z around player
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            float potentialX = _player.position.x + randomCircle.x;
            float potentialZ = _player.position.z + randomCircle.y;

            // Check if position is within terrain bounds
            if (potentialX >= terrainPosition.x &&
                potentialX <= terrainPosition.x + terrainData.size.x &&
                potentialZ >= terrainPosition.z &&
                potentialZ <= terrainPosition.z + terrainData.size.z)
            {
                // Sample the actual terrain height at this position
                float terrainHeight = terrain.SampleHeight(new Vector3(potentialX, 0, potentialZ));
                float groundY = terrainPosition.y + terrainHeight;

                // Set spawn point below the surface
                Vector3 spawnPosition = new Vector3(potentialX, groundY - _spawnOffsetBelowGround, potentialZ);

                return spawnPosition;
            }
        }

        // Fallback: spawn at player position at terrain height
        Debug.LogWarning($"Could not find valid position within terrain bounds after {maxAttempts} attempts.");
        float playerTerrainHeight = terrain.SampleHeight(_player.position);
        float playerGroundY = terrainPosition.y + playerTerrainHeight;
        return new Vector3(_player.position.x, playerGroundY - _spawnOffsetBelowGround, _player.position.z);
    }

    public override void OnGet(Flyweight f)
    {
        Damageable enemyDamageable = f.GetComponent<Damageable>();
        enemyDamageable.Initialize(_initialHealth);

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