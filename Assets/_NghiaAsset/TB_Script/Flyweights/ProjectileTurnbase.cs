using System;
using System.Collections;
using Ami.BroAudio;
using Turnbase;
using UnityEngine;

namespace Turnbase
{
    public class ProjectileTurnBase : Flyweight_TB
    {
        private Character owner;
        private Character target;
        private Action onHitCallback;
        private Skill skillData;
        private int damageAmount;

        public SoundID sound;

        private ElementType projectileElement;

        private const float SPEED = 30f;

        public void Setup(Character attacker, Character target, Skill skill, int damage, ElementType element, Action hitCallback)
        {
            this.owner = attacker;
            this.target = target;
            this.skillData = skill;
            this.damageAmount = damage;
            this.projectileElement = element;
            this.onHitCallback = hitCallback;

            StartCoroutine(MoveToTarget());
        }

        private IEnumerator MoveToTarget()
        {
            Vector3 GetTargetPos()
            {
                if (target == null) return transform.position;
                return target.buffEffectSpawnPoint != null
                    ? target.buffEffectSpawnPoint.position
                    : target.transform.position + Vector3.up * 1f;
            }

            Vector3 endPos = GetTargetPos();

            while (target != null && Vector3.Distance(transform.position, endPos) > 0.1f)
            {
                endPos = GetTargetPos(); 

                transform.position = Vector3.MoveTowards(transform.position, endPos, SPEED * Time.deltaTime);

                transform.LookAt(endPos);

                yield return null;
            }

            if (target != null)
            {
                ApplyImpact();
            }

            float releaseDelay = skillData.impactVFXDuration > 0 ? skillData.impactVFXDuration : 0f;
            if (releaseDelay > 0) yield return new WaitForSeconds(releaseDelay);

            FlyweightFactory_TB.ReturnToPool(this);
        }

        private void ApplyImpact()
        {
            ElementType element = projectileElement;

            if (target != null && target.isAttackBlocked && target.isParrySuccessful)
            {
                onHitCallback?.Invoke();
                return;
            }

            target.TakeDamage(owner, damageAmount, element);

            SpawnImpactEffect(target.transform.position, skillData);

            onHitCallback?.Invoke();

            BroAudio.Play(sound);
        }

        private void SpawnImpactEffect(Vector3 position, Skill skill)
        {
            FlyweightSettings_TB effectToSpawn = skill.impactVFXPrefab;


            if (effectToSpawn != null)
            {
                Flyweight_TB effectInstance = FlyweightFactory_TB.Spawn(effectToSpawn);

                if (effectInstance != null)
                {
                    effectInstance.Initialize(position, Quaternion.identity);
                }
            }
        }
    }
}