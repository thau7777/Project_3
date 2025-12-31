using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Enemy Settings")]
    [SerializeField] private List<EnemyTopDownSettings> _enemyTopDownSettings;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnInterval = 3f; // seconds between spawns
    [SerializeField] private int _minSpawnCountPerWave = 2; // minimum enemies per wave
    [SerializeField] private int _maxSpawnCountPerWave = 5; // maximum enemies per wave

    private Transform _player;
    private Coroutine _spawnRoutine;

    // Allow runtime modification
    public int MinSpawnCountPerWave
    {
        get => _minSpawnCountPerWave;
        set => _minSpawnCountPerWave = Mathf.Max(0, value);
    }

    public int MaxSpawnCountPerWave
    {
        get => _maxSpawnCountPerWave;
        set => _maxSpawnCountPerWave = Mathf.Max(_minSpawnCountPerWave, value); // Max must be >= Min
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_player == null)
        {
            Debug.LogError("EnemyManager: Player not found! Make sure your Player has the 'Player' tag.");
            return;
        }

        SetupSettings();
        _spawnRoutine = StartCoroutine(SpawnEnemiesLoop());
    }

    private void SetupSettings()
    {
        foreach (var setting in _enemyTopDownSettings)
        {
            setting.SetupSpawnSettings(_player, 10f);
        }
    }

    private IEnumerator SpawnEnemiesLoop()
    {
        while (true)
        {
            SpawnMultipleEnemies();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    public void SetNewEnemyInitialHealth(float newHealth)
    {
        foreach (var setting in _enemyTopDownSettings)
        {
            setting.SetInitialHealthOnSpawn(newHealth);
        }
    }

    private void SpawnMultipleEnemies()
    {
        if (_enemyTopDownSettings == null || _enemyTopDownSettings.Count == 0)
        {
            Debug.LogWarning("EnemyManager: No enemy settings assigned!");
            return;
        }

        // Generate random count between min and max (inclusive)
        int spawnCount = Random.Range(_minSpawnCountPerWave, _maxSpawnCountPerWave + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRandomly();
        }
    }

    private void SpawnRandomly()
    {
        var enemySettings = _enemyTopDownSettings[Random.Range(0, _enemyTopDownSettings.Count)];
        Flyweight enemy = FlyweightFactory.Spawn(enemySettings);

        if (enemy == null)
        {
            Debug.LogWarning("EnemyManager: Failed to spawn enemy from FlyweightFactory.");
            return;
        }
    }
}