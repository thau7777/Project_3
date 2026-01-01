using UnityEngine;

[CreateAssetMenu(fileName = " NewEnemyTopDownSettings", menuName = "Scriptable Objects/Flyweight/Enemy TopDown Settings")]
public class EnemyTopDownSettings : FlyweightSettings
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _initialHealth = 100;
    public ElementalType elementalType = ElementalType.Normal;
    public float spawnAnimationTime = 0.5f;
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
        // 1. Calculate random X and Z around player
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
        Vector3 spawnPosition = _player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // 2. Find the exact height of the terrain surface at this X, Z
        if (Terrain.activeTerrain != null)
        {
            float surfaceY = Terrain.activeTerrain.SampleHeight(spawnPosition) + Terrain.activeTerrain.transform.position.y;

            // 3. Set the spawn point to be 2 units BELOW that surface
            spawnPosition.y = surfaceY - 2f;
        }
        else
        {
            // Fallback if no terrain is found
            spawnPosition.y = -2f;
        }

        return spawnPosition;
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
