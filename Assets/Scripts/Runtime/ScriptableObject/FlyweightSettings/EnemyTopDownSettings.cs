using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyTopDownSettings", menuName = "Scriptable Objects/Flyweight/Enemy TopDown Settings")]
public class EnemyTopDownSettings : FlyweightSettings
{
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _initialHealth = 100;

    [SerializeField] private float _raycastHeight = 50f;
    [SerializeField] private float _spawnOffsetBelowGround = 2f;

    public ElementalType elementalType = ElementalType.Normal;
    public float spawnAnimationDuration = 0.5f;
    [SerializeField] private OneShotVFXSettings _spawnVFXSettings;

    [HideInInspector] public LayerMask groundLayerMask;
    [HideInInspector] public LayerMask obstacleLayerMask;
    private float _spawnClearanceRadius = 2.5f;
    private Transform _player;

    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var flyweight = go.GetComponent<EnemyTopdownStateDriver>();
        flyweight.settings = this;
        go.GetComponent<Damageable>().hasShieldBreakingMechanic = true;

        var characterStats = go.GetOrAdd<CharacterStats>();
        go.GetOrAdd<NavMeshSteering>().target = PlayerTopDownStateDriver.Instance.transform;
        return flyweight;
    }

    public void SetupSpawnSettings(Transform player, float spawnRadius, LayerMask groundLayerMask, LayerMask obstacleLayerMask)
    {
        _player = player;
        _spawnRadius = spawnRadius;
        this.groundLayerMask = groundLayerMask;
        this.obstacleLayerMask = obstacleLayerMask;
    }

    private bool TryGetGroundHeight(float x, float z, out float height)
    {
        height = 0f;
        Vector3 rayOrigin = new Vector3(x, _raycastHeight, z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _raycastHeight * 2f, groundLayerMask))
        {
            height = hit.point.y;
            return true;
        }

        return false;
    }

    private Vector3 PickRandomLocationAroundPlayer()
    {
        const int maxAttempts = 30;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            float potentialX = _player.position.x + randomCircle.x;
            float potentialZ = _player.position.z + randomCircle.y;

            if (!TryGetGroundHeight(potentialX, potentialZ, out float groundY))
                continue;

            Vector3 boxCenter = new Vector3(potentialX, groundY + _spawnClearanceRadius, potentialZ);
            Vector3 halfExtents = Vector3.one * _spawnClearanceRadius;

            bool blocked = Physics.CheckBox(
                boxCenter,
                halfExtents,
                Quaternion.identity,
                obstacleLayerMask
            );

            if (!blocked)
                return new Vector3(potentialX, groundY - _spawnOffsetBelowGround, potentialZ);
        }

        Debug.LogWarning($"Could not find valid spawn position after {maxAttempts} attempts. Falling back to player position.");
        if (TryGetGroundHeight(_player.position.x, _player.position.z, out float fallbackY))
            return new Vector3(_player.position.x, fallbackY - _spawnOffsetBelowGround, _player.position.z);

        return _player.position;
    }

    public override void OnGet(Flyweight f)
    {
        base.OnGet(f);
        Vector3 spawnPosition = PickRandomLocationAroundPlayer();
        f.GetComponent<CharacterController>().enabled = false;
        f.transform.position = spawnPosition;

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
        f.transform.position = spawnPosition;

        float groundSurfaceY = spawnPosition.y + _spawnOffsetBelowGround;
        f.GetComponent<EnemyTopdownStateDriver>().StartSpawnAnim(groundSurfaceY);
    }

    public override void OnRelease(Flyweight f)
    {
        base.OnRelease(f);
        f.GetComponent<EnemyTopdownStateDriver>().ResetStateContext();
    }
}