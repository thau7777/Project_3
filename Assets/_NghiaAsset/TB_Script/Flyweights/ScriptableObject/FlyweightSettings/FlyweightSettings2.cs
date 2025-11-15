using UnityEngine;

namespace Turnbase
{
    public abstract class FlyweightSettings2 : ScriptableObject
    {
        [TabGroup("Pool Settings")]
        public bool collectionCheck = true;

        [TabGroup("Pool Settings")]
        public int defaultCapacity = 10;

        [TabGroup("Pool Settings")]
        public int maxPoolSize = 100;

        [TabGroup("Flyweight Info")]
        public FlyweightType type;

        [TabGroup("Flyweight Info")]
        public GameObject prefab;

        public abstract Flyweight2 Create();

        public virtual void OnGet(Flyweight2 f) => f.gameObject.SetActive(true);
        public virtual void OnRelease(Flyweight2 f) => f.gameObject.SetActive(false);
        public virtual void OnDestroyPoolObject(Flyweight2 f) => Destroy(f.gameObject);
    }

}