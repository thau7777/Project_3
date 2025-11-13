using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public class StraightProjectile2 : Flyweight2
    {
        new StraightProjectileSettings2 settings => (StraightProjectileSettings2)base.settings;

        private Vector3? _direction = null;
        private Rigidbody _rb;
        private float _speed;
        private Vector3 _ogScale;
        private Coroutine _despawnRoutine;

        public Vector3? projectileImpactScale;

        private const float MaxHeight = 1.35f;
        private const float DescentSpeed = 2f; // how fast it moves down when above height

        HitBoxHandler _hitboxHandler;
        private void Awake()
        {
            _rb = gameObject.GetOrAdd<Rigidbody>();
            _hitboxHandler = GetComponent<HitBoxHandler>();
            _direction = null;
            _rb.useGravity = false;
            _ogScale = transform.localScale;
        }

        private void OnEnable()
        {
            transform.localScale = _ogScale;
            projectileImpactScale = null;
        }

        private void OnDisable()
        {
            _direction = null;

            if (_despawnRoutine != null)
            {
                StopCoroutine(_despawnRoutine);
                _despawnRoutine = null;
            }
        }

        public void InitializeMovement(Vector3 direction, float speed)
        {
            _direction = direction.normalized;
            _speed = speed;
            _despawnRoutine = StartCoroutine(DespawnAfterDelay(settings.DespawnDelay));
        }

        private void FixedUpdate()
        {
            if (_direction == null || _rb == null)
            {
                _rb.Stop();
                return;
            }

            // Get base movement velocity
            Vector3 velocity = _direction.Value * _speed;

            // Adjust height if needed
            float currentY = transform.position.y;
            if (currentY > MaxHeight)
            {
                float descent = Mathf.Min((currentY - MaxHeight), DescentSpeed * Time.fixedDeltaTime);
                velocity.y -= descent * (1f / Time.fixedDeltaTime);
            }

            _rb.linearVelocity = velocity;
        }

        private IEnumerator DespawnAfterDelay(float delay)
        {
            yield return Helpers.GetWaitForSeconds(delay);
            DespawnFlyweight();
        }
        public void DespawnFlyweight()
        {
            SpawnHitVFX();
            FlyweightFactory2.ReturnToPool(this);
        }
        private void SpawnHitVFX()
        {
            var projectileImpactFlyweight = FlyweightFactory2.Spawn(settings);
            projectileImpactFlyweight.Initialize(transform.position, Quaternion.identity);
            if (projectileImpactScale != null)
                projectileImpactFlyweight.transform.localScale = projectileImpactScale.Value;
        }
    }

}