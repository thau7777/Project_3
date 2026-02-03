using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyTopDownSettings", menuName = "Scriptable Objects/Flyweight/Enemy TopDown Settings")]
public class EnemyTopDownSettings : FlyweightSettings
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _initialHealth = 100;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _raycastHeight = 50f;
    [SerializeField] private float _spawnOffsetBelowGround = 2f;

    public ElementalType elementalType = ElementalType.Normal;
    public float spawnAnimationTime = 0.5f;
    [SerializeField] private FlyweightSettings _spawnVFX;
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
        const int maxAttempts = 30;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // 1. Calculate random X and Z around player (flat circle, ignore player Y)
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            float potentialX = _player.position.x + randomCircle.x;
            float potentialZ = _player.position.z + randomCircle.y;

            // 2. Raycast downward from high above to find the ground
            Vector3 rayStart = new Vector3(potentialX, _raycastHeight, potentialZ);

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, _groundLayer))
            {
                // Valid ground found! Set spawn point below the surface
                Vector3 spawnPosition = new Vector3(potentialX, hit.point.y - _spawnOffsetBelowGround, potentialZ);

                if (_spawnVFX != null)
                {
                    var vfx = FlyweightFactory.Spawn(_spawnVFX);
                    vfx.FlyweightInitialize(new Vector3(potentialX, hit.point.y+0.1f, potentialZ));
                    OneShotVFXSettings vfxSettings = vfx.settings as OneShotVFXSettings;
                    (vfx as OneShotVFX).InitializeVFX(vfxSettings.DefaultSize, vfxSettings.DefaultLifeTime);
                }

                return spawnPosition;
            }
        }

        // Fallback: If we can't find valid ground after max attempts, spawn at player position
        Debug.LogWarning($"Could not find valid ground position after {maxAttempts} attempts. Spawning at player position.");
        return _player.position + Vector3.down * _spawnOffsetBelowGround;
    }

    public override void OnGet(Flyweight f)
    {
        Damageable enemyDamageable = f.GetComponent<Damageable>();
        enemyDamageable.Initialize(_initialHealth);
        f.transform.position = PickRandomLocationAroundPlayer();
        base.OnGet(f);
        f.GetComponent<EnemyTopdownStateDriver>().StartSpawnAnim();
    }

    public override void OnRelease(Flyweight f)
    {
        base.OnRelease(f);
        f.GetComponent<EnemyTopdownStateDriver>().ResetStateContext();
    }
}