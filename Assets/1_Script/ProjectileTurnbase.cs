using UnityEngine;
using System;
using System.Collections;
using Turnbase;

namespace Turnbase
{
    public class ProjectileTurnBase : Flyweight
    {
        private Character target;
        private Action onHitCallback;
        private Skill skillData;
        private int damageAmount;

        private const float SPEED = 50f;

        public void Setup(Character target, Skill skill, int damage, Action hitCallback)
        {
            this.target = target;
            this.skillData = skill;
            this.damageAmount = damage;
            this.onHitCallback = hitCallback;

            StartCoroutine(MoveToTarget());
        }

        private IEnumerator MoveToTarget()
        {
            Vector3 endPos = target.transform.position;

            while (target != null && Vector3.Distance(transform.position, endPos) > 0.1f)
            {
                endPos = target.transform.position;

                transform.position = Vector3.MoveTowards(transform.position, endPos, SPEED * Time.deltaTime);

                yield return null;
            }

            if (target != null)
            {
                ApplyImpact();
            }
            float releaseDelay = skillData.impactVFXDuration > 0 ? skillData.impactVFXDuration : 0f;

            if (releaseDelay > 0)
            {
                yield return new WaitForSeconds(releaseDelay);
            }

            FlyweightFactory.ReturnToPool(this);

        }

        private void ApplyImpact()
        {
            target.TakeDamage(damageAmount);
            SpawnImpactEffect(target.transform.position, skillData);

            onHitCallback?.Invoke();
        }

        private void SpawnImpactEffect(Vector3 position, Skill skill)
        {
            GameObject effectToSpawn = skill.impactVFXPrefab;
            float duration = skill.impactVFXDuration;

            if (effectToSpawn != null)
            {
                GameObject effectInstance = GameObject.Instantiate(effectToSpawn, position, Quaternion.identity);

                GameObject.Destroy(effectInstance, duration);
            }
        }
    }
}