using UnityEngine;

namespace Turnbase
{
    public abstract class Flyweight_TB : MonoBehaviour
    {
        public FlyweightSettings_TB settings;
        public virtual void Initialize(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;

            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in particles)
            {
                p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                p.Play(true);
            }
        }
        public void ReturnToPool()
        {
            FlyweightFactory_TB.ReturnToPool(this);
        }
    }

}