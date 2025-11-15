using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = " NewEnemyTopDownSettings", menuName = "Scriptable Objects/Flyweight Turnbase/Enemy TopDown Settings")]
    public class EnemyTopDownSettings2 : FlyweightSettings
    {
        [SerializeField] private float _spawnRadius = 10f;
        [SerializeField] private int _initialHealth = 100;
        private Transform _player;

        public override Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.name = prefab.name;

            var flyweight = go.GetComponent<EnemyTopdownStateDriver>();
            flyweight.settings = this;

            return flyweight;
        }
        public void SetupSpawnSettings(Transform player, float spawnRadius)
        {
            _player = player;
            _spawnRadius = spawnRadius;
        }
        public void SetInitialHealthOnSpawn(int health)
        {
            _initialHealth = Mathf.Max(1, health);
        }
        private Vector3 PickRandomLocationAroundPlayer()
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            Vector3 spawnPosition = _player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            spawnPosition.y = 0f;
            return spawnPosition;
        }
        public override void OnGet(Flyweight f)
        {
            f.GetComponent<Damageable>().Initialize(_initialHealth);
            f.transform.position = PickRandomLocationAroundPlayer();
            base.OnGet(f);
        }
        public override void OnRelease(Flyweight f)
        {
            base.OnRelease(f);
            f.GetComponent<EnemyTopdownStateDriver>().ResetStateContext();
        }
    }

}