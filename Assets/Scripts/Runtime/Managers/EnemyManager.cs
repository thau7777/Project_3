using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Enemy Settings")]
    [SerializeField] private List<EnemyTopDownSettings> _enemiesList;


    private Transform _player;

 

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

    private void SpawnEnemies(List<EnemyTopDownSettings> enemies)
    {
        if (_enemiesList == null || _enemiesList.Count == 0)
        {
            Debug.LogWarning("EnemyManager: No enemy settings assigned!");
            return;
        }

    }
}