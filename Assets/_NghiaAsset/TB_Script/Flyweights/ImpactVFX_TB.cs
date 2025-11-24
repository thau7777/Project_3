using UnityEngine;
using System.Collections;

namespace Turnbase
{
    public class ImpactVFX_TB : Flyweight_TB
    {
        public void Initialize(Vector3 position, Quaternion rotation, float duration)
        {
            base.Initialize(position, rotation);

            transform.position = position;
            transform.rotation = rotation;


            float finalDuration = duration;

            ParticleSystem particle = GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Play(true);

                finalDuration = Mathf.Max(duration, particle.main.duration);
            }

            StartCoroutine(DespawnAfterDelay(finalDuration));
        }

        IEnumerator DespawnAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Debug.Log("ImpactVFX returned to pool after " + delay + " seconds.");

            FlyweightFactory_TB.ReturnToPool(this);

        }
    }
}