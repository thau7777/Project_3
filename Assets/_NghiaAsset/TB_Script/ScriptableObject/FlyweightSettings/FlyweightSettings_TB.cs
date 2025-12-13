using UnityEngine;

namespace Turnbase
{
    public abstract class FlyweightSettings_TB : ScriptableObject
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

        public abstract Flyweight_TB Create();

        public virtual void OnGet(Flyweight_TB f) => f.gameObject.SetActive(true);
        public virtual void OnRelease(Flyweight_TB f) => f.gameObject.SetActive(false);
        public virtual void OnDestroyPoolObject(Flyweight_TB f) => Destroy(f.gameObject);
    }

}