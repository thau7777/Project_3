using UnityEngine;
using System.Collections;

public class ImpactVFX : Flyweight
{
    public void Initialize(Vector3 position, Quaternion rotation, float duration)
    {
        base.FlyweightInitialize(position, rotation);

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

        FlyweightFactory.ReturnToPool(this);

    }
}