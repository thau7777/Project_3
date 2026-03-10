using MyRule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownEnemyManager : Singleton<TopDownEnemyManager>
{
    [System.Serializable]
    public struct TopdownEnemyWithIds
    {
        public EnemyId id;
        public EnemyTopDownSettings settings;
    }

    [Header("Enemy Registry")]
    [SerializeField] private List<TopdownEnemyWithIds> _topDownEnemyWithIdsList;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private float _waveDuration = 60f; // 1 minute per wave

    // Wave state
    private GroupWave _groupWave;
    private int _currentWaveIndex = 0;
    private int _remainingEnemiesInWave = 0;
    private bool _isSpawning = false;
    private bool _gameOver = false;
    private Coroutine _waveTimerCoroutine;

    // Tracked live enemies
    private List<GameObject> _activeEnemies = new List<GameObject>();

    private Transform _player;

    // Events
    private EventBinding<TopdownStartGameEvent> _startGameEventBinding;

    private void OnEnable()
    {
        _startGameEventBinding = new EventBinding<TopdownStartGameEvent>(LoadCurrentWave);
        _startGameEventBinding.Add(SpawnEnemies);
        EventBus<TopdownStartGameEvent>.Register(_startGameEventBinding);
    }

    private void OnDisable()
    {
        EventBus<TopdownStartGameEvent>.Deregister(_startGameEventBinding);
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_player == null)
        {
            Debug.LogError("EnemyManager: Player not found! Make sure your Player has the 'Player' tag.");
        }
    }

    private void LoadCurrentWave()
    {
        _groupWave = WaveManager.Instance.GetCurrentWave();

        if (_groupWave == null)
        {
            Debug.LogError("TopDownEnemyManager: No GroupWave returned from WaveManager!");
        }
    }

    // Called when the game starts via event
    private void SpawnEnemies()
    {
        if (_groupWave == null)
        {
            Debug.LogError("TopDownEnemyManager: GroupWave is null, cannot spawn enemies.");
            return;
        }

        _currentWaveIndex = 0;
        _gameOver = false;
        StartCoroutine(StartWave(_currentWaveIndex));
    }

    // ─── Wave Lifecycle ───────────────────────────────────────────────────────

    private IEnumerator StartWave(int waveIndex)
    {
        WaveData[] waves = _groupWave.WaveDatas;

        if (waveIndex >= waves.Length)
        {
            WinGame();
            yield break;
        }

        WaveData wave = waves[waveIndex];
        if (wave == null)
        {
            Debug.LogWarning($"Wave {waveIndex} is null — skipping.");
            yield return StartCoroutine(AdvanceToNextWave());
            yield break;
        }

        Debug.Log($"[WaveManager] Starting Wave {waveIndex + 1} / {waves.Length}");

        // Count valid enemies in this wave
        _remainingEnemiesInWave = 0;
        foreach (var enemy in wave.Enemies)
        {
            if (enemy != null) _remainingEnemiesInWave++;
        }

        _isSpawning = true;
        _activeEnemies.Clear();

        // Spawn all enemies in the wave
        for (int i = 0; i < wave.Enemies.Length; i++)
        {
            EnemyData enemyData = wave.Enemies[i];
            if (enemyData == null) continue;

            SpawnEnemy(enemyData);
            yield return new WaitForSeconds(0.3f); // slight stagger per spawn
        }

        _isSpawning = false;

        // Start the 1-minute wave timer
        if (_waveTimerCoroutine != null)
            StopCoroutine(_waveTimerCoroutine);

        _waveTimerCoroutine = StartCoroutine(WaveTimer());
    }

    private IEnumerator WaveTimer()
    {
        yield return new WaitForSeconds(_waveDuration);

        if (_gameOver) yield break;

        // Timer expired — kill remaining enemies and advance
        Debug.Log($"[WaveManager] Wave {_currentWaveIndex + 1} time expired! Moving to next wave.");
        ClearRemainingEnemies();
        yield return StartCoroutine(AdvanceToNextWave());
    }

    private IEnumerator AdvanceToNextWave()
    {
        if (_gameOver) yield break;

        yield return new WaitForSeconds(2f); // brief pause between waves

        _currentWaveIndex++;
        yield return StartCoroutine(StartWave(_currentWaveIndex));
    }

    // ─── Enemy Spawning ───────────────────────────────────────────────────────

    private void SpawnEnemy(EnemyData enemyData)
    {
        EnemyTopDownSettings settings = GetSettingsForId(enemyData.EnemyId);
        if (settings == null)
        {
            Debug.LogWarning($"No settings found for EnemyId: {enemyData.EnemyId}");
            _remainingEnemiesInWave--;
            return;
        }

        Vector3 spawnPos = GetRandomSpawnPosition();

        var enemy = FlyweightFactory.Spawn(settings);
        if(enemy.TryGetComponent<CharacterStats>(out var enemyStats))
        {
            enemyStats.Setup(settings.elementalType, enemyData.Health, 0, enemyData.Phys, enemyData.Mag, enemyData.Fire, enemyData.Water, enemyData.Frost,
                enemyData.Lightning, enemyData.Holy, enemyData.Dark, enemyData.Poison, enemyData.PhyDef, enemyData.MagDef, enemyData.FireDef, enemyData.WaterDef,
                enemyData.FrostDef, enemyData.LightningDef, enemyData.HolyDef, enemyData.DarkDef, enemyData.PoisonDef, enemyData.AttackSpeed, enemyData.CritChance, enemyData.CritMult);
        }
        _activeEnemies.Add(enemy.gameObject);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * _spawnRadius;
        return _player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    private EnemyTopDownSettings GetSettingsForId(EnemyId id)
    {
        foreach (var entry in _topDownEnemyWithIdsList)
        {
            if (entry.id == id) return entry.settings;
        }
        return null;
    }

    // ─── Enemy Death Callback ─────────────────────────────────────────────────

    // Called by each enemy when they die
    public void OnEnemyDied(GameObject enemyGO)
    {
        if (_activeEnemies.Contains(enemyGO))
            _activeEnemies.Remove(enemyGO);

        _remainingEnemiesInWave--;
        _remainingEnemiesInWave = Mathf.Max(0, _remainingEnemiesInWave);

        Debug.Log($"[WaveManager] Enemy died. Remaining in wave: {_remainingEnemiesInWave}");

        if (_remainingEnemiesInWave == 0)
        {
            Debug.Log($"[WaveManager] Wave {_currentWaveIndex + 1} cleared!");
            if (_waveTimerCoroutine != null)
                StopCoroutine(_waveTimerCoroutine);

            StartCoroutine(AdvanceToNextWave());
        }
    }

    // ─── Cleanup ──────────────────────────────────────────────────────────────

    private void ClearRemainingEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        _activeEnemies.Clear();
        _remainingEnemiesInWave = 0;
    }

    // ─── Win / Lose ───────────────────────────────────────────────────────────

    private void WinGame()
    {
        _gameOver = true;
        Debug.Log("[WaveManager] All waves complete — YOU WIN!");
        //EventBus<TopdownWinGameEvent>.Raise(new TopdownWinGameEvent());
    }

    // ─── Public Helpers (UI / HUD) ────────────────────────────────────────────

    public int GetRemainingEnemies() => _remainingEnemiesInWave;
    public int GetCurrentWaveIndex() => _currentWaveIndex;
    public int GetTotalWaves() => _groupWave?.WaveDatas?.Length ?? 0;
}