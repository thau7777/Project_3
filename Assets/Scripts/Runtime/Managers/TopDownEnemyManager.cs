using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownEnemyManager : Singleton<TopDownEnemyManager>
{
    [Header("Enemy Settings")]
    [SerializeField] private List<EnemyTopDownSettings> _enemiesList;

    [SerializeField]
    private int _testSpawnCount = 5;

    private EventBinding<TopDownStartGameEvent> _startGameEventBinding;

    private Transform _player;

    private void OnEnable()
    {
        _startGameEventBinding = new EventBinding<TopDownStartGameEvent>(SpawnEnemies);
        EventBus<TopDownStartGameEvent>.Register(_startGameEventBinding);
    }

    private void OnDisable()
    {
        EventBus<TopDownStartGameEvent>.Deregister(_startGameEventBinding);
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
    }
    public List<EnemyTopDownSettings> GetAllEnemies()
    {
        return _enemiesList;
    }
    private void SetupSettings()
    {
        foreach (var setting in _enemiesList)
        {
            setting.SetupSpawnSettings(_player, 10f);
        }
    }

    public void SetNewEnemyInitialHealth(float newHealth)
    {
        foreach (var setting in _enemiesList)
        {
            setting.SetInitialHealthOnSpawn(newHealth);
        }
    }

    private void SpawnEnemies()
    {
        if (_enemiesList == null || _enemiesList.Count == 0)
        {
            Debug.LogWarning("EnemyManager: No enemy settings assigned!");
            return;
        }
        for(int i = 0; i < _testSpawnCount; i++)
        {
            FlyweightFactory.Spawn(_enemiesList[i]);
        }
    }
}