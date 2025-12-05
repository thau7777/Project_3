using System.Collections.Generic;
using Turnbase;
using UnityEngine;
using UnityEngine.Events;

public class MinionsManager : Singleton<MinionsManager>
{
    public enum MinionKind 
    { 
        Knight,
        Wizard,
        Golem
    }

    [SerializeField] private MinionData[] minionsDataArray = new MinionData[3];
    private MinionTopDownStateDriver[] _minionsSpawnedArray = new MinionTopDownStateDriver[3];
    [SerializeField] private float _spawnRadius = 5f;
    private Transform _player;

    private List<GameObject> _targetedEnemies = new();
    private GameObject _prioritizedEnemy;


    protected override void Awake()
    {
        base.Awake();
        _player = GameObject.FindWithTag("Player").transform;
        
        SpawnMinions();
    }
    private Vector3 PickRandomLocationAroundPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
        Vector3 spawnPosition = _player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        spawnPosition.y = 0f;
        return spawnPosition;
    }
    private void SpawnMinions()
    {
        for(int i=0; i<minionsDataArray.Length; i++)
        {
            if (minionsDataArray[i] == null) continue;
            var obj = Instantiate(minionsDataArray[i].MinionPrefab);
            obj.transform.position = PickRandomLocationAroundPlayer();
            obj.GetOrAdd<MinionTopDownStateDriver>().InitializeMinion(minionsDataArray[i]);

            _minionsSpawnedArray[i] = obj.GetComponent<MinionTopDownStateDriver>();
        }

    }

    public void AddTargetedEnemies(GameObject enemy)
    {
        _targetedEnemies.Add(enemy);
        CheckAndReorderPriority();
    }
    public void RemoveTargetedEnemy(GameObject enemy)
    {
        _targetedEnemies.Remove(enemy);
        CheckAndReorderPriority();
    }
    public void RemoveAllTargetedEnemies()
    {
        for(int i = _targetedEnemies.Count-1; i >= 0; i--)
            _targetedEnemies[i].GetComponent<EffectsManager>().RemoveEffectByName("SetSummonerTargetEffect");

    }
    public void CheckAndReorderPriority()
    {
        if (_targetedEnemies.Count == 0)
        {
            EventBus<SummonerTargetEvent>.Raise(new SummonerTargetEvent(null));
            return;
        }
        _prioritizedEnemy = _targetedEnemies[0];
        foreach(var enemy in _targetedEnemies)
        {
            var distance = Vector3.Distance(_player.transform.position, enemy.transform.position);
            var nearestDistance = Vector3.Distance(_player.transform.position, _prioritizedEnemy.transform.position);
            if (distance < nearestDistance)
                _prioritizedEnemy = enemy;
        }
        EventBus<SummonerTargetEvent>.Raise(new SummonerTargetEvent(_prioritizedEnemy.transform));
    }
}
