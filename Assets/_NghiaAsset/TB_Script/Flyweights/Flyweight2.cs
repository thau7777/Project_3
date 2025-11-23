using UnityEngine;

namespace Turnbase
{
    public abstract class Flyweight2 : MonoBehaviour
    {
        public FlyweightSettings2 settings;
        public void Initialize(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
        public void ReturnToPool()
        {
            FlyweightFactory2.ReturnToPool(this);
        }
    }

}