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
        }
        public void ReturnToPool()
        {
            FlyweightFactory_TB.ReturnToPool(this);
        }
    }

}